import math

import torch
import torch.nn as nn
from torch.nn import functional as F

from mingpt.utils import CfgNode as CN


class NewGELU(nn.Module):
    def forward(self, x):
        return 0.5 * x * (1.0 + torch.tanh(math.sqrt(2.0 / math.pi) * (x + 0.044715 * torch.pow(x, 3.0))))


class CausalSelfAttention(nn.Module):
    def __init__(self, config):
        super().__init__()
        assert config.n_embd % config.n_head == 0
        self.c_attn = nn.Linear(config.n_embd, 3 * config.n_embd)
        self.c_proj = nn.Linear(config.n_embd, config.n_embd)
        self.attn_dropout = nn.Dropout(config.attn_pdrop)
        self.resid_dropout = nn.Dropout(config.resid_pdrop)
        self.register_buffer(
            "bias",
            torch.tril(torch.ones(config.block_size, config.block_size)).view(1, 1, config.block_size, config.block_size),
        )
        self.n_head = config.n_head
        self.n_embd = config.n_embd

    def forward(self, x):
        batch_size, sequence_length, channels = x.size()
        query, key, value = self.c_attn(x).split(self.n_embd, dim=2)
        key = key.view(batch_size, sequence_length, self.n_head, channels // self.n_head).transpose(1, 2)
        query = query.view(batch_size, sequence_length, self.n_head, channels // self.n_head).transpose(1, 2)
        value = value.view(batch_size, sequence_length, self.n_head, channels // self.n_head).transpose(1, 2)

        attention = (query @ key.transpose(-2, -1)) * (1.0 / math.sqrt(key.size(-1)))
        attention = attention.masked_fill(self.bias[:, :, :sequence_length, :sequence_length] == 0, float("-inf"))
        attention = F.softmax(attention, dim=-1)
        attention = self.attn_dropout(attention)
        y = attention @ value
        y = y.transpose(1, 2).contiguous().view(batch_size, sequence_length, channels)
        y = self.resid_dropout(self.c_proj(y))
        return y


class Block(nn.Module):
    def __init__(self, config):
        super().__init__()
        self.ln_1 = nn.LayerNorm(config.n_embd)
        self.attn = CausalSelfAttention(config)
        self.ln_2 = nn.LayerNorm(config.n_embd)
        self.mlp = nn.ModuleDict(
            dict(
                c_fc=nn.Linear(config.n_embd, 4 * config.n_embd),
                c_proj=nn.Linear(4 * config.n_embd, config.n_embd),
                act=NewGELU(),
                dropout=nn.Dropout(config.resid_pdrop),
            )
        )
        module = self.mlp
        self.mlpf = lambda x: module.dropout(module.c_proj(module.act(module.c_fc(x))))

    def forward(self, x):
        x = x + self.attn(self.ln_1(x))
        x = x + self.mlpf(self.ln_2(x))
        return x


class GPT(nn.Module):
    @staticmethod
    def get_default_config():
        config = CN()
        config.model_type = "gpt"
        config.n_layer = None
        config.n_head = None
        config.n_embd = None
        config.vocab_size = None
        config.block_size = None
        config.embd_pdrop = 0.1
        config.resid_pdrop = 0.1
        config.attn_pdrop = 0.1
        return config

    def __init__(self, config):
        super().__init__()
        assert config.vocab_size is not None
        assert config.block_size is not None
        self.block_size = config.block_size

        type_given = config.model_type is not None
        params_given = all([config.n_layer is not None, config.n_head is not None, config.n_embd is not None])
        assert type_given ^ params_given
        if type_given:
            config.merge_from_dict(
                {
                    "openai-gpt": dict(n_layer=12, n_head=12, n_embd=768),
                    "gpt2": dict(n_layer=12, n_head=12, n_embd=768),
                    "gpt2-medium": dict(n_layer=24, n_head=16, n_embd=1024),
                    "gpt2-large": dict(n_layer=36, n_head=20, n_embd=1280),
                    "gpt2-xl": dict(n_layer=48, n_head=25, n_embd=1600),
                    "gopher-44m": dict(n_layer=8, n_head=16, n_embd=512),
                    "gpt-mini": dict(n_layer=6, n_head=6, n_embd=192),
                    "gpt-micro": dict(n_layer=4, n_head=4, n_embd=128),
                    "gpt-nano": dict(n_layer=3, n_head=3, n_embd=48),
                }[config.model_type]
            )

        self.transformer = nn.ModuleDict(
            dict(
                wte=nn.Embedding(config.vocab_size, config.n_embd),
                wpe=nn.Embedding(config.block_size, config.n_embd),
                drop=nn.Dropout(config.embd_pdrop),
                h=nn.ModuleList([Block(config) for _ in range(config.n_layer)]),
                ln_f=nn.LayerNorm(config.n_embd),
            )
        )
        self.lm_head = nn.Linear(config.n_embd, config.vocab_size, bias=False)
        self.apply(self._init_weights)
        for parameter_name, parameter in self.named_parameters():
            if parameter_name.endswith("c_proj.weight"):
                torch.nn.init.normal_(parameter, mean=0.0, std=0.02 / math.sqrt(2 * config.n_layer))
        n_params = sum(p.numel() for p in self.transformer.parameters())
        print("number of parameters: %.2fM" % (n_params / 1e6,))

    def _init_weights(self, module):
        if isinstance(module, nn.Linear):
            torch.nn.init.normal_(module.weight, mean=0.0, std=0.02)
            if module.bias is not None:
                torch.nn.init.zeros_(module.bias)
        elif isinstance(module, nn.Embedding):
            torch.nn.init.normal_(module.weight, mean=0.0, std=0.02)
        elif isinstance(module, nn.LayerNorm):
            torch.nn.init.zeros_(module.bias)
            torch.nn.init.ones_(module.weight)

    def configure_optimizers(self, train_config):
        decay = set()
        no_decay = set()
        whitelist_weight_modules = (torch.nn.Linear,)
        blacklist_weight_modules = (torch.nn.LayerNorm, torch.nn.Embedding)
        for module_name, module in self.named_modules():
            for parameter_name, _ in module.named_parameters():
                full_parameter_name = f"{module_name}.{parameter_name}" if module_name else parameter_name
                if parameter_name.endswith("bias"):
                    no_decay.add(full_parameter_name)
                elif parameter_name.endswith("weight") and isinstance(module, whitelist_weight_modules):
                    decay.add(full_parameter_name)
                elif parameter_name.endswith("weight") and isinstance(module, blacklist_weight_modules):
                    no_decay.add(full_parameter_name)

        parameter_dict = {parameter_name: parameter for parameter_name, parameter in self.named_parameters()}
        inter_params = decay & no_decay
        union_params = decay | no_decay
        assert len(inter_params) == 0
        assert len(parameter_dict.keys() - union_params) == 0

        optimizer_groups = [
            {"params": [parameter_dict[pn] for pn in sorted(list(decay))], "weight_decay": train_config.weight_decay},
            {"params": [parameter_dict[pn] for pn in sorted(list(no_decay))], "weight_decay": 0.0},
        ]
        return torch.optim.AdamW(optimizer_groups, lr=train_config.learning_rate, betas=train_config.betas)

    def forward(self, idx, targets=None):
        device = idx.device
        _, sequence_length = idx.size()
        assert sequence_length <= self.block_size, f"Cannot forward sequence of length {sequence_length}, block size is only {self.block_size}"
        pos = torch.arange(0, sequence_length, dtype=torch.long, device=device).unsqueeze(0)

        tok_emb = self.transformer.wte(idx)
        pos_emb = self.transformer.wpe(pos)
        x = self.transformer.drop(tok_emb + pos_emb)
        for block in self.transformer.h:
            x = block(x)
        x = self.transformer.ln_f(x)
        logits = self.lm_head(x)

        loss = None
        if targets is not None:
            loss = F.cross_entropy(logits.view(-1, logits.size(-1)), targets.view(-1), ignore_index=-1)
        return logits, loss

    @torch.no_grad()
    def generate(self, idx, max_new_tokens, temperature=1.0, do_sample=False, top_k=None):
        for _ in range(max_new_tokens):
            idx_cond = idx if idx.size(1) <= self.block_size else idx[:, -self.block_size:]
            logits, _ = self(idx_cond)
            logits = logits[:, -1, :] / temperature
            if top_k is not None:
                values, _ = torch.topk(logits, top_k)
                logits[logits < values[:, [-1]]] = -float("Inf")
            probs = F.softmax(logits, dim=-1)
            if do_sample:
                idx_next = torch.multinomial(probs, num_samples=1)
            else:
                _, idx_next = torch.topk(probs, k=1, dim=-1)
            idx = torch.cat((idx, idx_next), dim=1)
        return idx