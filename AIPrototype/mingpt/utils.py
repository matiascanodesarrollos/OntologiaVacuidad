import json
import os
import random
import sys
from ast import literal_eval

import numpy as np
import torch


def set_seed(seed):
    random.seed(seed)
    np.random.seed(seed)
    torch.manual_seed(seed)
    torch.cuda.manual_seed_all(seed)


def setup_logging(config):
    work_dir = config.system.work_dir
    os.makedirs(work_dir, exist_ok=True)
    with open(os.path.join(work_dir, "args.txt"), "w", encoding="utf-8") as handle:
        handle.write(" ".join(sys.argv))
    with open(os.path.join(work_dir, "config.json"), "w", encoding="utf-8") as handle:
        handle.write(json.dumps(config.to_dict(), indent=4))


class CfgNode:
    def __init__(self, **kwargs):
        self.__dict__.update(kwargs)

    def __str__(self):
        return self._str_helper(0)

    def _str_helper(self, indent):
        parts = []
        for key, value in self.__dict__.items():
            if isinstance(value, CfgNode):
                parts.append(f"{key}:\n")
                parts.append(value._str_helper(indent + 1))
            else:
                parts.append(f"{key}: {value}\n")
        return "".join([(" " * (indent * 4)) + part for part in parts])

    def to_dict(self):
        return {key: value.to_dict() if isinstance(value, CfgNode) else value for key, value in self.__dict__.items()}

    def merge_from_dict(self, values):
        self.__dict__.update(values)

    def merge_from_args(self, args):
        for arg in args:
            keyval = arg.split("=")
            assert len(keyval) == 2, f"expecting each override arg to be of form --arg=value, got {arg}"
            key, value = keyval
            try:
                value = literal_eval(value)
            except ValueError:
                pass

            assert key[:2] == "--"
            key = key[2:]
            keys = key.split(".")
            obj = self
            for partial in keys[:-1]:
                obj = getattr(obj, partial)
            leaf_key = keys[-1]
            assert hasattr(obj, leaf_key), f"{key} is not an attribute that exists in the config"
            print(f"command line overwriting config attribute {key} with {value}")
            setattr(obj, leaf_key, value)