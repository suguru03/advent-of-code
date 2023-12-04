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

  private parse() {
    return File.parse(this.filepath, (file) => file.split('\n'));
  }
}

assert.strictEqual(new Solution('example.txt').solve1(), 142);
console.log(new Solution().solve1());
