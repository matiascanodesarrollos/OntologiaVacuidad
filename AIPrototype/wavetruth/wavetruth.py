import math
import os
import sys
import time

CURRENT_DIR = os.path.dirname(os.path.abspath(__file__))
ROOT_DIR = os.path.dirname(CURRENT_DIR)
if CURRENT_DIR not in sys.path:
    sys.path.append(CURRENT_DIR)
if ROOT_DIR not in sys.path:
    sys.path.append(ROOT_DIR)

import torch
import torch.nn as nn
from torch.nn import functional as F
from torch.utils.data import Dataset

from mingpt.model import GPT
from mingpt.trainer import Trainer
from mingpt.utils import CfgNode as CN
from mingpt.utils import set_seed, setup_logging
from ontology import build_ontology_targets


def get_config():
    config = CN()
    config.system = CN()
    config.system.seed = 3407
    config.system.work_dir = os.path.join(ROOT_DIR, "out", "wavetruth")

    config.data = CharDataset.get_default_config()

    config.model = WaveTruthGPT.get_default_config()
    config.model.model_type = "gpt-nano"
    config.model.n_freq = 8
    config.model.wave_window = 16
    config.model.wave_sigma = 6.0
    config.model.lambda_wave = 0.15
    config.model.lambda_ontology = 0.2
    config.model.lambda_abstention = 0.25

    config.trainer = Trainer.get_default_config()
    config.trainer.learning_rate = 5e-4
    config.trainer.max_iters = 200
    config.trainer.batch_size = 32
    config.trainer.num_workers = 0
    return config


class CharDataset(Dataset):
    @staticmethod
    def get_default_config():
        config = CN()
        config.block_size = 96
        return config

    def __init__(self, config, data):
        self.config = config
        chars = sorted(list(set(data)))
        self.stoi = {ch: index for index, ch in enumerate(chars)}
        self.itos = {index: ch for index, ch in enumerate(chars)}
        self.vocab_size = len(chars)
        self.data = data
        self.ontology = build_ontology_targets(data)
        print(f"data has {len(data)} characters, {self.vocab_size} unique.")

    def get_vocab_size(self):
        return self.vocab_size

    def get_block_size(self):
        return self.config.block_size

    def __len__(self):
        return len(self.data) - self.config.block_size

    def __getitem__(self, idx):
        chunk = self.data[idx:idx + self.config.block_size + 1]
        dix = [self.stoi[symbol] for symbol in chunk]
        x = torch.tensor(dix[:-1], dtype=torch.long)
        y = torch.tensor(dix[1:], dtype=torch.long)
        ontology = {
            "frequency": self.ontology["frequency"][idx:idx + self.config.block_size],
            "amplitude": self.ontology["amplitude"][idx:idx + self.config.block_size],
            "phase": self.ontology["phase"][idx:idx + self.config.block_size],
            "effect": self.ontology["effect"][idx:idx + self.config.block_size],
            "abstention": self.ontology["abstention"][idx:idx + self.config.block_size],
            "mask": self.ontology["mask"][idx:idx + self.config.block_size],
        }
        return x, y, ontology["frequency"], ontology["amplitude"], ontology["phase"], ontology["effect"], ontology["abstention"], ontology["mask"]


class WaveTrainer(Trainer):
    def run(self):
        model, config = self.model, self.config
        self.optimizer = model.configure_optimizers(config)
        train_loader = torch.utils.data.DataLoader(
            self.train_dataset,
            sampler=torch.utils.data.RandomSampler(self.train_dataset, replacement=True, num_samples=int(1e10)),
            shuffle=False,
            pin_memory=True,
            batch_size=config.batch_size,
            num_workers=config.num_workers,
        )
        model.train()
        self.iter_num = 0
        self.iter_time = time.time()
        data_iter = iter(train_loader)
        while True:
            try:
                batch = next(data_iter)
            except StopIteration:
                data_iter = iter(train_loader)
                batch = next(data_iter)

            x, y, frequency, amplitude, phase, effect, abstention, mask = [tensor.to(self.device) for tensor in batch]
            ontology_targets = {
                "frequency": frequency,
                "amplitude": amplitude,
                "phase": phase,
                "effect": effect,
                "abstention": abstention,
                "mask": mask,
            }
            _, self.loss = model(x, y, ontology_targets=ontology_targets)
            model.zero_grad(set_to_none=True)
            self.loss.backward()
            torch.nn.utils.clip_grad_norm_(model.parameters(), config.grad_norm_clip)
            self.optimizer.step()
            self.trigger_callbacks("on_batch_end")
            self.iter_num += 1
            current_time = time.time()
            self.iter_dt = current_time - self.iter_time
            self.iter_time = current_time
            if config.max_iters is not None and self.iter_num >= config.max_iters:
                break


class WaveTruthGPT(GPT):
    @staticmethod
    def get_default_config():
        config = GPT.get_default_config()
        config.n_freq = 8
        config.wave_window = 16
        config.wave_sigma = 6.0
        config.lambda_wave = 0.1
        config.lambda_ontology = 0.1
        config.lambda_abstention = 0.15
        return config

    def __init__(self, config):
        super().__init__(config)
        self.n_freq = config.n_freq
        self.wave_window = config.wave_window
        self.wave_sigma = config.wave_sigma
        self.lambda_wave = config.lambda_wave
        self.lambda_ontology = config.lambda_ontology
        self.lambda_abstention = config.lambda_abstention
        self.wave_proj = nn.Linear(config.n_embd, config.n_freq * 2)
        self.ontology_proj = nn.Linear(config.n_embd, 4)
        self.abstention_head = nn.Linear(config.n_embd, 1)
        self.register_buffer("omega", torch.linspace(0.25, math.pi, steps=config.n_freq))
        self.last_metrics = {}

    def _hidden_states(self, idx):
        device = idx.device
        _, sequence_length = idx.size()
        pos = torch.arange(0, sequence_length, dtype=torch.long, device=device).unsqueeze(0)
        tok_emb = self.transformer.wte(idx)
        pos_emb = self.transformer.wpe(pos)
        hidden = self.transformer.drop(tok_emb + pos_emb)
        for block in self.transformer.h:
            hidden = block(hidden)
        hidden = self.transformer.ln_f(hidden)
        return hidden

    def _wave_coherence_loss(self, hidden_states):
        batch_size, sequence_length, _ = hidden_states.size()
        projected = self.wave_proj(hidden_states).view(batch_size, sequence_length, self.n_freq, 2)
        signal = torch.view_as_complex(projected.contiguous())
        positions = torch.arange(sequence_length, device=hidden_states.device)
        deltas = positions.unsqueeze(1) - positions.unsqueeze(0)
        causal_mask = ((deltas >= 0) & (deltas <= self.wave_window)).to(hidden_states.dtype)
        kernel = torch.exp(-deltas.abs().to(hidden_states.dtype) / self.wave_sigma) * causal_mask
        phase = torch.exp(-1j * self.omega.view(1, 1, self.n_freq) * deltas.to(hidden_states.dtype).unsqueeze(-1))
        weighted = signal.unsqueeze(1) * kernel.view(1, sequence_length, sequence_length, 1)
        aggregated = (weighted * phase.unsqueeze(0)).sum(dim=2)
        numerator = aggregated.abs()
        denominator = (kernel.view(1, sequence_length, sequence_length, 1) * signal.abs().unsqueeze(1)).sum(dim=2).clamp_min(1e-6)
        coherence = numerator / denominator
        wave_loss = 1.0 - coherence.mean()
        self.last_metrics = {"wave_loss": float(wave_loss.detach().cpu()), "coherence": float(coherence.mean().detach().cpu())}
        return wave_loss

    def _ontology_loss(self, hidden_states, ontology_targets):
        predictions = torch.sigmoid(self.ontology_proj(hidden_states))
        targets = torch.stack([
            ontology_targets["frequency"],
            ontology_targets["amplitude"],
            ontology_targets["phase"],
            ontology_targets["effect"],
        ], dim=-1)
        mask = ontology_targets["mask"].unsqueeze(-1)
        squared_error = ((predictions - targets) ** 2) * mask
        loss = squared_error.sum() / mask.sum().clamp_min(1.0)
        self.last_metrics["ontology_loss"] = float(loss.detach().cpu())
        return loss

    def _abstention_loss(self, hidden_states, ontology_targets):
        logits = self.abstention_head(hidden_states).squeeze(-1)
        targets = ontology_targets["abstention"]
        mask = ontology_targets["mask"]
        loss_map = F.binary_cross_entropy_with_logits(logits, targets, reduction="none")
        loss = (loss_map * mask).sum() / mask.sum().clamp_min(1.0)
        probabilities = torch.sigmoid(logits)
        predicted = (probabilities > 0.5).to(mask.dtype)
        accuracy = ((predicted == targets).to(mask.dtype) * mask).sum() / mask.sum().clamp_min(1.0)
        self.last_metrics["abstention_loss"] = float(loss.detach().cpu())
        self.last_metrics["abstention_acc"] = float(accuracy.detach().cpu())
        return loss

    @torch.no_grad()
    def estimate_abstention(self, idx):
        hidden = self._hidden_states(idx)
        logits = self.abstention_head(hidden).squeeze(-1)
        return torch.sigmoid(logits[:, -1]).mean().item()

    def forward(self, idx, targets=None, ontology_targets=None):
        hidden = self._hidden_states(idx)
        logits = self.lm_head(hidden)
        loss = None
        if targets is not None:
            ce_loss = F.cross_entropy(logits.view(-1, logits.size(-1)), targets.view(-1), ignore_index=-1)
            wave_loss = self._wave_coherence_loss(hidden)
            ontology_loss = self._ontology_loss(hidden, ontology_targets) if ontology_targets is not None else 0.0
            abstention_loss = self._abstention_loss(hidden, ontology_targets) if ontology_targets is not None else 0.0
            loss = ce_loss + (self.lambda_wave * wave_loss) + (self.lambda_ontology * ontology_loss) + (self.lambda_abstention * abstention_loss)
            self.last_metrics["ce_loss"] = float(ce_loss.detach().cpu())
            self.last_metrics["total_loss"] = float(loss.detach().cpu())
        return logits, loss


if __name__ == "__main__":
    config = get_config()
    config.merge_from_args(sys.argv[1:])
    print(config)
    setup_logging(config)
    set_seed(config.system.seed)
    with open(os.path.join(CURRENT_DIR, "train.txt"), "r", encoding="utf-8") as handle:
        text = handle.read()
    train_dataset = CharDataset(config.data, text)
    config.model.vocab_size = train_dataset.get_vocab_size()
    config.model.block_size = train_dataset.get_block_size()
    model = WaveTruthGPT(config.model)
    trainer = WaveTrainer(config.trainer, model, train_dataset)

    def batch_end_callback(trainer_instance):
        if trainer_instance.iter_num % 10 == 0:
            metrics = model.last_metrics
            print(
                "iter_dt %.2fms; iter %d: total %.5f | ce %.5f | wave %.5f | coh %.5f | abst %.5f | abst_acc %.5f"
                % (
                    trainer_instance.iter_dt * 1000,
                    trainer_instance.iter_num,
                    metrics.get("total_loss", trainer_instance.loss.item()),
                    metrics.get("ce_loss", float("nan")),
                    metrics.get("wave_loss", float("nan")),
                    metrics.get("coherence", float("nan")),
                    metrics.get("abstention_loss", float("nan")),
                    metrics.get("abstention_acc", float("nan")),
                )
            )
            print("ontology %.5f" % metrics.get("ontology_loss", float("nan")))
        if trainer_instance.iter_num % 100 == 0:
            model.eval()
            with torch.no_grad():
                context = "consulta: cobre conduce electricidad"
                x = torch.tensor([train_dataset.stoi[s] for s in context], dtype=torch.long)[None, ...].to(trainer_instance.device)
                y = model.generate(x, 80, temperature=0.9, do_sample=True, top_k=8)[0]
                completion = "".join([train_dataset.itos[int(i)] for i in y])
                print(completion)
                print("abstention_score %.5f" % model.estimate_abstention(x))
            os.makedirs(config.system.work_dir, exist_ok=True)
            torch.save(model.state_dict(), os.path.join(config.system.work_dir, "model.pt"))
            model.train()

    trainer.set_callback("on_batch_end", batch_end_callback)
    trainer.run()