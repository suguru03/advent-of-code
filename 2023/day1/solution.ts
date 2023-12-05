import * as assert from 'node:assert';

import { File } from '../utils/file';
import { BaseSolution } from '../utils/solution';

class Solution extends BaseSolution {
  solve1() {
    const iter = (char: string) => /\d/.test(char);
    return this.parse().reduce((sum, row) => {
      const chars = row.split('');
      return sum + Number(chars.find(iter)) * 10 + Number(chars.findLast(iter));
    }, 0);
  }

  solve2() {
    const [leftMap, rightMap] = this.constructMaps();
    return this.parse().reduce((sum, row) => {
      const l = row.length;
      let left = '';
      for (let i = 0; i < l; i++) {
        left += row[i];
        while (left.length > 0 && !leftMap.has(left)) {
          left = left.slice(1);
        }
        const num = leftMap.get(left);
        if (typeof num !== 'number') {
          continue;
        }
        sum += num * 10;
        break;
      }

      let right = '';
      for (let i = l - 1; i >= 0; i--) {
        right = `${row[i]}${right}`;
        while (right.length > 0 && !rightMap.has(right)) {
          right = right.slice(0, -1);
        }
        const num = rightMap.get(right);
        if (typeof num !== 'number') {
          continue;
        }
        sum += num;
        break;
      }

      return sum;
    }, 0);
  }

  private constructMaps() {
    const numMap = Array.from({ length: 9 }, (_, i) => [(++i).toString(), i] as const);
    const leftMap = new Map<string, number | null>(numMap);
    const rightMap = new Map<string, number | null>(numMap);
    for (const [num, str] of ['', 'one', 'two', 'three', 'four', 'five', 'six', 'seven', 'eight', 'nine'].entries()) {
      if (!str) {
        continue;
      }
      let left = '';
      let right = '';
      const l = str.length;
      for (let i = 0; i < l; i++) {
        left += str[i];
        right = `${str[l - i - 1]}${right}`;
        leftMap.set(left, null);
        rightMap.set(right, null);
      }
      leftMap.set(left, num);
      rightMap.set(right, num);
    }

    return [leftMap, rightMap];
  }

  private parse() {
    return File.parse(this.filepath, (file) => file.split('\n'));
  }
}

assert.strictEqual(new Solution('example1.txt').solve1(), 142);
console.log(new Solution().solve1());
assert.strictEqual(new Solution('example2.txt').solve2(), 281);
console.log(new Solution().solve2());
