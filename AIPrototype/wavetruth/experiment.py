import json
import os
import sys
from dataclasses import dataclass

import torch
from torch.utils.data import DataLoader, Dataset, RandomSampler

CURRENT_DIR = os.path.dirname(os.path.abspath(__file__))
ROOT_DIR = os.path.dirname(CURRENT_DIR)
if CURRENT_DIR not in sys.path:
    sys.path.append(CURRENT_DIR)
if ROOT_DIR not in sys.path:
    sys.path.append(ROOT_DIR)

from mingpt.model import GPT
from mingpt.utils import CfgNode as CN
from mingpt.utils import set_seed
from ontology import build_ontology_targets
from wavetruth import WaveTruthGPT


def get_config():
    config = CN()
    config.system = CN()
    config.system.device = "cpu"
    config.system.seeds = [11, 17, 23, 29, 31]

    config.data = CN()
    config.data.block_size = 192

    config.model = CN()
    config.model.model_type = "gpt-nano"

    config.train = CN()
    config.train.max_iters = 500
    config.train.batch_size = 16
    config.train.learning_rate = 5e-4
    config.train.weight_decay = 0.1
    config.train.betas = (0.9, 0.95)
    config.train.grad_norm_clip = 1.0

    config.eval = CN()
    config.eval.abstention_threshold = 0.55
    config.eval.thresholds = [0.45, 0.55, 0.65]
    config.eval.report_path = os.path.join(ROOT_DIR, "out", "wavetruth", "experiment_report.json")
    return config


class SharedVocabDataset(Dataset):
    def __init__(self, text, stoi, block_size, include_ontology):
        self.text = text
        self.stoi = stoi
        self.block_size = block_size
        self.include_ontology = include_ontology
        self.ontology = build_ontology_targets(text) if include_ontology else None

    def __len__(self):
        return len(self.text) - self.block_size

    def __getitem__(self, idx):
        chunk = self.text[idx:idx + self.block_size + 1]
        dix = [self.stoi[symbol] for symbol in chunk]
        x = torch.tensor(dix[:-1], dtype=torch.long)
        y = torch.tensor(dix[1:], dtype=torch.long)
        if not self.include_ontology:
            return x, y
        return (
            x,
            y,
            self.ontology["frequency"][idx:idx + self.block_size],
            self.ontology["amplitude"][idx:idx + self.block_size],
            self.ontology["phase"][idx:idx + self.block_size],
            self.ontology["effect"][idx:idx + self.block_size],
            self.ontology["abstention"][idx:idx + self.block_size],
            self.ontology["mask"][idx:idx + self.block_size],
        )


@dataclass
class ExperimentBundle:
    train_text: str
    eval_cases: list
    stoi: dict
    itos: dict
    vocab_size: int


def load_bundle(project_dir):
    with open(os.path.join(project_dir, "train.txt"), "r", encoding="utf-8") as handle:
        train_text = handle.read()
    with open(os.path.join(project_dir, "eval_cases.json"), "r", encoding="utf-8") as handle:
        eval_cases = json.load(handle)
    vocab_source = train_text + "\n" + json.dumps(eval_cases, ensure_ascii=False)
    chars = sorted(list(set(vocab_source)))
    stoi = {ch: index for index, ch in enumerate(chars)}
    itos = {index: ch for index, ch in enumerate(chars)}
    return ExperimentBundle(train_text, eval_cases, stoi, itos, len(chars))


def configure_optimizer(model, config):
    train_cfg = CN()
    train_cfg.learning_rate = config.train.learning_rate
    train_cfg.weight_decay = config.train.weight_decay
    train_cfg.betas = config.train.betas
    return model.configure_optimizers(train_cfg)


def train_model(model_name, model, dataset, config):
    device = config.system.device
    model = model.to(device)
    optimizer = configure_optimizer(model, config)
    loader = DataLoader(
        dataset,
        sampler=RandomSampler(dataset, replacement=True, num_samples=config.train.max_iters * config.train.batch_size),
        shuffle=False,
        batch_size=config.train.batch_size,
        num_workers=0,
    )
    iterator = iter(loader)
    model.train()
    losses = []
    for _ in range(config.train.max_iters):
        batch = next(iterator)
        if dataset.include_ontology:
            x, y, frequency, amplitude, phase, effect, abstention, mask = [tensor.to(device) for tensor in batch]
            ontology_targets = {"frequency": frequency, "amplitude": amplitude, "phase": phase, "effect": effect, "abstention": abstention, "mask": mask}
            _, loss = model(x, y, ontology_targets=ontology_targets)
        else:
            x, y = [tensor.to(device) for tensor in batch]
            _, loss = model(x, y)
        model.zero_grad(set_to_none=True)
        loss.backward()
        torch.nn.utils.clip_grad_norm_(model.parameters(), config.train.grad_norm_clip)
        optimizer.step()
        losses.append(float(loss.detach().cpu()))
    return {"model_name": model_name, "final_train_loss": losses[-1], "mean_last_50_loss": sum(losses[-50:]) / min(50, len(losses))}


def score_candidate(model, prompt, candidate, stoi, device):
    text = prompt + candidate
    indices = torch.tensor([stoi[ch] for ch in text], dtype=torch.long, device=device).unsqueeze(0)
    prompt_len = len(prompt)
    idx = indices[:, :-1]
    targets = indices[:, 1:]
    logits, _ = model(idx)
    log_probs = torch.log_softmax(logits, dim=-1)
    total = 0.0
    count = 0
    for position in range(prompt_len - 1, idx.size(1)):
        target_id = targets[0, position].item()
        total += float(log_probs[0, position, target_id].detach().cpu())
        count += 1
    return total / max(count, 1)


def build_prompt(case):
    return "\n".join(case["evidence"] + [case["query"]]) + " "


def classify_case(model, case, bundle, config, supports_abstention, abstention_threshold=None):
    prompt = build_prompt(case)
    candidates = ["si.", "no.", "abstenerse."]
    scores = {candidate.rstrip("."): score_candidate(model, prompt, candidate, bundle.stoi, config.system.device) for candidate in candidates}
    predicted = max(scores.items(), key=lambda item: item[1])[0]
    abstention_score = None
    if supports_abstention:
        idx = torch.tensor([bundle.stoi[ch] for ch in prompt], dtype=torch.long, device=config.system.device).unsqueeze(0)
        abstention_score = model.estimate_abstention(idx)
        threshold = config.eval.abstention_threshold if abstention_threshold is None else abstention_threshold
        if abstention_score >= threshold:
            predicted = "abstenerse"
    hallucinated = case["gold"] == "abstenerse" and predicted != "abstenerse"
    return {"id": case["id"], "gold": case["gold"], "predicted": predicted, "scores": scores, "abstention_score": abstention_score, "correct": predicted == case["gold"], "hallucinated": hallucinated}


def summarize_results(name, train_summary, case_results):
    total = len(case_results)
    correct = sum(1 for item in case_results if item["correct"])
    hallucinations = sum(1 for item in case_results if item["hallucinated"])
    abstain_cases = sum(1 for item in case_results if item["gold"] == "abstenerse")
    abstain_correct = sum(1 for item in case_results if item["gold"] == "abstenerse" and item["predicted"] == "abstenerse")
    supported_correct = sum(1 for item in case_results if item["gold"] != "abstenerse" and item["correct"])
    supported_total = sum(1 for item in case_results if item["gold"] != "abstenerse")
    return {"model": name, "final_train_loss": train_summary["final_train_loss"], "mean_last_50_loss": train_summary["mean_last_50_loss"], "accuracy": correct / total, "hallucination_rate": hallucinations / max(abstain_cases, 1), "abstention_recall": abstain_correct / max(abstain_cases, 1), "supported_accuracy": supported_correct / max(supported_total, 1), "case_results": case_results}


def make_baseline_model(bundle, config):
    model_config = GPT.get_default_config()
    model_config.model_type = config.model.model_type
    model_config.vocab_size = bundle.vocab_size
    model_config.block_size = config.data.block_size
    return GPT(model_config)


def make_wavetruth_model(bundle, config):
    model_config = WaveTruthGPT.get_default_config()
    model_config.model_type = config.model.model_type
    model_config.vocab_size = bundle.vocab_size
    model_config.block_size = config.data.block_size
    model_config.lambda_wave = 0.15
    model_config.lambda_ontology = 0.2
    model_config.lambda_abstention = 0.25
    return WaveTruthGPT(model_config)


def run_single_seed(seed, bundle, config):
    set_seed(seed)
    baseline_dataset = SharedVocabDataset(bundle.train_text, bundle.stoi, config.data.block_size, include_ontology=False)
    wave_dataset = SharedVocabDataset(bundle.train_text, bundle.stoi, config.data.block_size, include_ontology=True)
    baseline_model = make_baseline_model(bundle, config)
    baseline_train = train_model("baseline", baseline_model, baseline_dataset, config)
    baseline_cases = [classify_case(baseline_model, case, bundle, config, supports_abstention=False) for case in bundle.eval_cases]
    baseline_summary = summarize_results("baseline", baseline_train, baseline_cases)
    set_seed(seed)
    wave_model = make_wavetruth_model(bundle, config)
    wave_train = train_model("wavetruth", wave_model, wave_dataset, config)
    wave_thresholds = {}
    for threshold in config.eval.thresholds:
        wave_cases = [classify_case(wave_model, case, bundle, config, supports_abstention=True, abstention_threshold=threshold) for case in bundle.eval_cases]
        wave_thresholds[str(threshold)] = summarize_results(f"wavetruth@{threshold}", wave_train, wave_cases)
    return {"seed": seed, "baseline": baseline_summary, "wavetruth": wave_thresholds}


def aggregate(model_key, all_results):
    rows = [result[model_key] for result in all_results]
    metrics = ["accuracy", "hallucination_rate", "abstention_recall", "supported_accuracy", "final_train_loss", "mean_last_50_loss"]
    summary = {}
    for metric in metrics:
        values = [row[metric] for row in rows]
        summary[metric] = sum(values) / len(values)
    return summary


def aggregate_threshold(threshold_key, all_results):
    rows = [result["wavetruth"][threshold_key] for result in all_results]
    metrics = ["accuracy", "hallucination_rate", "abstention_recall", "supported_accuracy", "final_train_loss", "mean_last_50_loss"]
    summary = {}
    for metric in metrics:
        values = [row[metric] for row in rows]
        summary[metric] = sum(values) / len(values)
    return summary


def print_report(all_results):
    print("=== Per-seed results ===")
    for result in all_results:
        print(f"seed={result['seed']}")
        baseline = result["baseline"]
        print(f"  baseline: acc={baseline['accuracy']:.3f} | supported={baseline['supported_accuracy']:.3f} | abst_recall={baseline['abstention_recall']:.3f} | halluc={baseline['hallucination_rate']:.3f}")
        for threshold_key, row in result["wavetruth"].items():
            print(f"  wavetruth@{threshold_key}: acc={row['accuracy']:.3f} | supported={row['supported_accuracy']:.3f} | abst_recall={row['abstention_recall']:.3f} | halluc={row['hallucination_rate']:.3f}")
    print("\n=== Aggregate means ===")
    baseline = aggregate("baseline", all_results)
    print(f"baseline: acc={baseline['accuracy']:.3f} | supported={baseline['supported_accuracy']:.3f} | abst_recall={baseline['abstention_recall']:.3f} | halluc={baseline['hallucination_rate']:.3f} | train_loss={baseline['mean_last_50_loss']:.3f}")
    for threshold in ["0.45", "0.55", "0.65"]:
        row = aggregate_threshold(threshold, all_results)
        print(f"wavetruth@{threshold}: acc={row['accuracy']:.3f} | supported={row['supported_accuracy']:.3f} | abst_recall={row['abstention_recall']:.3f} | halluc={row['hallucination_rate']:.3f} | train_loss={row['mean_last_50_loss']:.3f}")
    print("\n=== Detailed WaveTruth predictions (last seed, threshold 0.55) ===")
    for case in all_results[-1]["wavetruth"]["0.55"]["case_results"]:
        score_text = ", ".join(f"{label}={score:.3f}" for label, score in case["scores"].items())
        abst_text = "n/a" if case["abstention_score"] is None else f"{case['abstention_score']:.3f}"
        print(f"{case['id']}: gold={case['gold']} pred={case['predicted']} abst={abst_text} halluc={case['hallucinated']} scores[{score_text}]")


def write_report(all_results, config):
    os.makedirs(os.path.dirname(config.eval.report_path), exist_ok=True)
    payload = {"baseline": aggregate("baseline", all_results), "wavetruth": {threshold: aggregate_threshold(threshold, all_results) for threshold in ["0.45", "0.55", "0.65"]}, "per_seed": all_results}
    with open(config.eval.report_path, "w", encoding="utf-8") as handle:
        json.dump(payload, handle, ensure_ascii=False, indent=2)


if __name__ == "__main__":
    config = get_config()
    bundle = load_bundle(CURRENT_DIR)
    all_results = [run_single_seed(seed, bundle, config) for seed in config.system.seeds]
    print_report(all_results)
    write_report(all_results, config)