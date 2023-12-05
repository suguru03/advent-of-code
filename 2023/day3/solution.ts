import * as assert from 'node:assert';

import { File } from '../utils/file';
import { BaseSolution } from '../utils/solution';

class Solution extends BaseSolution {
  solve1() {
    const rows = this.parse();
    let sum = 0;
    for (let y = 0; y < rows.length; y++) {
      const row = rows[y];
      let valid = false;
      let current = '';
      for (let x = 0; x < row.length; x++) {
        const char = row[x];
        if (isNaN(Number(char))) {
          if (valid) {
            sum += Number(current);
            valid = false;
          }
          current = '';
          continue;
        }

        current += char;
        valid ||= this.isValid(rows, x, y);
      }

      if (valid) {
        sum += Number(current);
      }
    }

    return sum;
  }

  private isValid(rows: string[][], x: number, y: number) {
    for (let dy = -1; dy <= 1; dy++) {
      let y1 = y + dy;
      if (y1 < 0 || y1 === rows.length) {
        continue;
      }
      for (let dx = -1; dx <= 1; dx++) {
        let x1 = x + dx;
        if (x1 < 0 || x1 === rows[y1].length) {
          continue;
        }

        const char = rows[y1][x1];
        if (/\d/.test(char) || char === '.') {
          continue;
        }
        return true;
      }
    }

    return false;
  }

  solve2() {
    const rows = this.parse();
    class GearMap extends Map<string, number[]> {
      tryAdd(gearId: string | null, value: string) {
        if (!gearId || !value) {
          return false;
        }
        const nums = this.get(gearId) ?? [];
        nums.push(Number(value));
        this.set(gearId, nums);
        return true;
      }
    }

    const gearMap = new GearMap();
    for (let y = 0; y < rows.length; y++) {
      const row = rows[y];
      let gearId: string | null = null;
      let current = '';
      for (let x = 0; x < row.length; x++) {
        const char = row[x];
        if (isNaN(Number(char))) {
          gearMap.tryAdd(gearId, current);
          gearId = null;
          current = '';
          continue;
        }

        current += char;
        gearId ??= this.getGearId(rows, x, y);
      }
      gearMap.tryAdd(gearId, current);
    }

    return Array.from(gearMap.values()).reduce(
      (sum, nums) => sum + (nums.length !== 2 ? 0 : nums.reduce((left, right) => left * right)),
      0,
    );
  }

  private getGearId(rows: string[][], x: number, y: number) {
    for (let dy = -1; dy <= 1; dy++) {
      let y1 = y + dy;
      if (y1 < 0 || y1 === rows.length) {
        continue;
      }
      for (let dx = -1; dx <= 1; dx++) {
        let x1 = x + dx;
        if (x1 < 0 || x1 === rows[y1].length) {
          continue;
        }

        const char = rows[y1][x1];
        if (char !== '*') {
          continue;
        }
        return `${x1}:${y1}`;
      }
    }

    return null;
  }

  private parse(): string[][] {
    return File.parse(this.filepath, (file) => file.split('\n').map((row) => row.split('')));
  }
}

assert.strictEqual(new Solution('example.txt').solve1(), 4361);
console.log(new Solution().solve1());
assert.strictEqual(new Solution('example.txt').solve2(), 467835);
console.log(new Solution().solve2());
