import * as path from 'path';
import { File } from '../utils/file';
import { Vector2 } from '../utils/vector';

const rocks = [
  [new Vector2(0, 0), new Vector2(1, 0), new Vector2(2, 0), new Vector2(3, 0)],
  [new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1), new Vector2(1, 2)],
  [new Vector2(0, 0), new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, 1), new Vector2(2, 2)],
  [new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 2), new Vector2(0, 3)],
  [new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1)],
];

class Solution {
  solve1() {
    const pattern = this.parse();
    const maxWidth = 7;
    const grid: number[][] = [];
    let il = -1;
    let ip = -1;
    const limit = 2022;
    while (++il < limit) {
      const rock = rocks[il % rocks.length];
      let x = 2;
      let y = grid.length + 4;
      while (--y >= 0) {
        ip = (ip + 1) % pattern.length;
        const dx = pattern[ip] === '>' ? 1 : -1;
        if (rock.every((tile) => isMovable(tile.add(new Vector2(x + dx, y))))) {
          x += dx;
        }
        if (rock.every((tile) => isMovable(tile.add(new Vector2(x, y - 1))))) {
          continue;
        }
        break;
      }

      for (const tile of rock) {
        const next = tile.add(new Vector2(x, y));
        grid[next.y] ??= Array(maxWidth).fill(0);
        grid[next.y][next.x] = 1;
      }

      function isMovable({ x, y }: Vector2) {
        if (x < 0 || x >= maxWidth || y < 0) {
          return false;
        }
        return grid[y]?.[x] !== 1;
      }
    }
    return grid.length;
  }

  private logGrid(grid: number[][]) {
    let str = grid
      .reverse()
      .map((row) => row.map((num) => (num === 0 ? '.' : '#')).join(''))
      .join('\n');
    console.log(str);
  }
  private parse(): string {
    return File.parse(path.resolve(__dirname, 'input.txt'), (file) => file);
  }
}

console.log(new Solution().solve1());
