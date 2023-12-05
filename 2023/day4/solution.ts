import * as assert from 'node:assert';

import { File } from '../utils/file';
import { BaseSolution } from '../utils/solution';

interface Data {
  winningNums: number[];
  hands: number[];
}

class Solution extends BaseSolution {
  solve1() {
    return this.parse().reduce((sum, { winningNums, hands }) => {
      const winningSet = new Set(winningNums);
      return sum + Math.floor(Math.pow(2, hands.reduce((total, cur) => total + Number(winningSet.has(cur)), 0) - 1));
    }, 0);
  }

  solve2() {
    const rows = this.parse();
    const instances = Array.from(rows, () => 1);
    for (const [i, { winningNums, hands }] of rows.entries()) {
      const winningSet = new Set(winningNums);
      let wins = 0;
      for (const hand of hands) {
        if (!winningSet.has(hand)) {
          continue;
        }

        const next = ++wins + i;
        if (next >= instances.length) {
          continue;
        }

        instances[next] += instances[i];
      }
    }

    return instances.reduce((sum, num) => sum + num);
  }

  private parse(): Data[] {
    const make = (str: string) =>
      str
        .trim()
        .split(' ')
        .filter((str) => str)
        .map((num) => Number(num));
    return File.parse(this.filepath, (file) =>
      file.split('\n').map((row) => {
        const [left, right] = row.split(':')[1].split('|');
        return {
          winningNums: make(left),
          hands: make(right),
        };
      }),
    );
  }
}

assert.strictEqual(new Solution('example.txt').solve1(), 13);
console.log(new Solution().solve1());
assert.strictEqual(new Solution('example.txt').solve2(), 30);
console.log(new Solution().solve2());
