import * as path from 'path';
import { File } from '../utils/file';
import { Vector2 } from '../utils/vector';

class Solution {
  solve1() {
    const [start, end, grid] = this.parse();
    const yl = grid.length;
    const xl = grid[yl - 1].length;
    const minGrid = Array.from(grid, (row) => Array.from(row, () => Infinity));
    return findMin(start, -1, -1);

    function findMin(vector: Vector2, prev: number, count: number) {
      const { x, y } = vector;
      if (x < 0 || x >= xl || y < 0 || y >= yl) {
        return Infinity;
      }
      if (++count >= minGrid[y][x]) {
        return Infinity;
      }

      const cur = grid[y][x];
      if (cur - prev > 1) {
        return Infinity;
      }

      if (vector.equal(end)) {
        minGrid[y][x] = count;
        return count;
      }

      minGrid[y][x] = count;
      return Math.min(
        findMin(vector.left, cur, count),
        findMin(vector.right, cur, count),
        findMin(vector.up, cur, count),
        findMin(vector.down, cur, count),
      );
    }
  }

  solve2() {}

  private parse(): [Vector2, Vector2, number[][]] {
    const base = 'a'.charCodeAt(0);
    let start: Vector2 = Vector2.zero;
    let end: Vector2 = Vector2.zero;
    const grid = File.parse(path.resolve(__dirname, 'input.txt'), (file) =>
      file.split(/\n/g).map((row, y) =>
        row.split('').map((char, x) => {
          if (char === 'S') {
            start = new Vector2(x, y);
            char = 'a';
          }
          if (char === 'E') {
            end = new Vector2(x, y);
            char = 'z';
          }
          return char.charCodeAt(0) - base;
        }),
      ),
    );
    return [start, end, grid];
  }
}

console.log(new Solution().solve1());
