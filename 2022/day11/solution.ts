import * as path from 'path';
import { File } from '../utils/file';

type MonkeyId = number;
const old = Symbol('old');

interface Monkey {
  id: MonkeyId;
  items: number[];
  operation: {
    operator: string;
    left: number | Symbol;
    right: number | Symbol;
  };
  test: {
    div: number;
    true: MonkeyId;
    false: MonkeyId;
  };
}

class Solution {
  solve1() {
    const monkeys = this.parse();
    const counts = Array.from(monkeys, () => 0);
    let count = 20;
    while (count-- > 0) {
      for (const { id, items, operation: op, test } of monkeys) {
        while (items.length > 0) {
          counts[id]++;
          const prev = items.shift();
          const next = eval(`${op.left === old ? prev : op.left} ${op.operator} ${op.right === old ? prev : op.right}`);
          const level = (next / 3) | 0;
          const target = test[`${level % test.div === 0}`];
          monkeys[target].items.push(level);
        }
      }
    }
    const [left, right] = counts.sort((n1, n2) => n2 - n1);
    return left * right;
  }

  solve2() {}

  private parse(): Monkey[] {
    return File.parse(path.resolve(__dirname, 'input.txt'), (file) => {
      const monkeys: Monkey[] = [];
      for (const row of file.split(/\n/g)) {
        const [, id] = row.match(/Monkey (\d+)/) ?? [];
        if (id) {
          monkeys.push({
            id: Number(id),
            items: [],
            operation: {
              operator: '',
              left: old,
              right: old,
            },
            test: {
              div: 1,
              true: 0,
              false: 0,
            },
          });
          continue;
        }
        const monkey = monkeys[monkeys.length - 1];
        const [, items] = row.match(/Starting items: (.+)/) ?? [];
        if (items) {
          monkey.items = items.split(/,/g).map((item) => Number(item.trim()));
          continue;
        }

        const [, operation] = row.match(/Operation: new = (.+)/) ?? [];
        if (operation) {
          const [left, operator, right] = operation.split(/\s/g).map((item) => item.trim());
          monkey.operation = {
            operator,
            left: left === 'old' ? old : Number(left),
            right: right === 'old' ? old : Number(right),
          };
          continue;
        }

        const [, div] = row.match(/Test: divisible by (.+)/) ?? [];
        if (div) {
          monkey.test.div = Number(div);
          continue;
        }

        const [, tId] = row.match(/If true: throw to monkey (.+)/) ?? [];
        if (tId) {
          monkey.test.true = Number(tId);
          continue;
        }

        const [, fId] = row.match(/If false: throw to monkey (.+)/) ?? [];
        if (fId) {
          monkey.test.false = Number(fId);
        }
      }

      return monkeys;
    });
  }
}

console.log(new Solution().solve1());
