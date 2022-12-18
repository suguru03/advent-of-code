import * as path from 'path';
import { File } from '../utils/file';

const delta = [
  [-1, 0, 0],
  [1, 0, 0],
  [0, -1, 0],
  [0, 1, 0],
  [0, 0, -1],
  [0, 0, 1],
] as const;

class Solution {
  solve1() {
    const rows = this.parse();
    const grid: number[][][] = [];
    for (const [x, y, z] of rows) {
      grid[z] ??= [];
      grid[z][y] ??= [];
      grid[z][y][x] = 1;
    }

    let count = 0;
    for (const row of rows) {
      for (const d of delta) {
        const [x, y, z] = row.map((n, i) => n + d[i]);
        if (grid[z]?.[y]?.[x] === 1) {
          continue;
        }
        count++;
      }
    }

    return count;
  }

  private parse(): [number, number, number][] {
    return File.parse(path.resolve(__dirname, 'input.txt'), (file) =>
      file.split('\n').map((row) => row.split(',').map((str) => Number(str)) as [number, number, number]),
    );
  }
}

console.log(new Solution().solve1());
