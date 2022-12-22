import * as path from 'path';
import { File } from '../utils/file';
import * as assert from "assert";

type Name = string;

type Operation =
  | {
      name: Name;
      num: number;
    }
  | {
      name: Name;
      left: Name;
      right: Name;
      operator: string;
    };

class Solution {
  solve1() {
    const list = this.parse();
    const opMap = new Map(list.map(op => [op.name, op]));
    const dependantsMap = new Map<Name, Set<Name>>(list.map((op) => [op.name, new Set()]));
    const resolvedMap = new Map<Name, number>();
    const queue: Name[] = [];
    for (const op of list) {
      if ('num' in op) {
        queue.push(op.name);
        resolvedMap.set(op.name, op.num);
        continue;
      }
      dependantsMap.get(op.left)?.add(op.name);
      dependantsMap.get(op.right)?.add(op.name);
    }

    const targetName = 'root';
    while (queue.length !== 0 && !resolvedMap.has(targetName)) {
      const name = queue.shift()!;
      const dependants = dependantsMap.get(name)!;
      if (dependants.size === 0) {
        continue;
      }
      for (const name of dependants) {
        const op = opMap.get(name)!;
        assert.ok('left' in op);
        if (!resolvedMap.has(op.left) || !resolvedMap.get(op.right)) {
          continue;
        }

        const val = eval(`${resolvedMap.get(op.left)} ${op.operator} ${resolvedMap.get(op.right)}`);
        assert.ok(!isNaN(val));
        queue.push(op.name);
        resolvedMap.set(op.name, val);
      }
    }

    return resolvedMap.get(targetName) ?? 0;
  }

  private parse(): Operation[] {
    return File.parse(path.resolve(__dirname, 'input.txt'), (file) =>
      file.split('\n').map((row) => {
        const [, name, rest] = row.match(/(.+): (.+)/) ?? [];
        const num = Number(rest.trim());
        if (!isNaN(num)) {
          return {
            name: name.trim(),
            num,
          };
        }

        const [, left, operator, right] = rest.match(/(.+) ([+\-*/]) (.+)/) ?? [];
        return {
          name: name.trim(),
          left: left.trim(),
          right: right.trim(),
          operator: operator.trim(),
        };
      }),
    );
  }
}

console.log(new Solution().solve1());
// console.log(new Solution().solve2());
