import * as path from 'path';
import { File } from '../utils/file';
import { Vector2 } from '../utils/vector';

enum Item {
  Air = '.',
  Rock = '#',
  Sand = 'o',
}

class Solution {
  solve1() {
    return this.solve(false);
  }

  solve2() {
    return this.solve(true);
  }

  private solve(hasFloor: boolean) {
    const [grid] = this.parse();
    let bottom = grid.length + (hasFloor ? 1 : 0);
    const start = new Vector2(500, 0);
    let count = 0;
    fillItem(start, Item.Air);
    while (grid[start.y][start.x] === Item.Air && fillSand(start)) {
      count++;
    }

    return count;

    function fillSand(vector: Vector2) {
      if (vector.y >= bottom) {
        if (!hasFloor) {
          bottom = 0;
        }
        return false;
      }

      if (isBlocked(vector)) {
        return false;
      }

      const up = vector.up;
      if (!isBlocked(up)) {
        return fillSand(up);
      }

      if (isBlocked(up.left) && isBlocked(up.right)) {
        fillItem(vector, Item.Sand);
        return true;
      }

      return fillSand(up.left) || fillSand(up.right);
    }

    function isBlocked({ x, y }: Vector2) {
      if (hasFloor && y === bottom) {
        return true;
      }
      const cur = grid[y]?.[x];
      return cur === Item.Rock || cur === Item.Sand;
    }

    function fillItem({ x, y }: Vector2, item: Item) {
      grid[y] ??= [];
      grid[y][x] = item;
    }
  }

  private logGrid(grid: string[][], minX: number, maxX: number) {
    let str = '';
    for (const row of grid) {
      for (let x = minX; x <= maxX; x++) {
        str += row?.[x] ?? Item.Air;
      }
      str += '\n';
    }
    console.log(str.trim());
  }

  private parse(): [string[][], number, number] {
    return File.parse(path.resolve(__dirname, 'input.txt'), (file) => {
      const grid: string[][] = [];
      let minX = Infinity;
      let maxX = -Infinity;
      const rows = file.split(/\n/g);
      for (const row of rows) {
        let [left, ...rest] = row.split('->').map((item) => item.split(',').map((str) => Number(str.trim())));
        while (rest.length) {
          let [xl, yl] = left;
          const right = rest.shift()!;
          const [xr, yr] = right;
          fillRock(xr, yr);
          while (xl !== xr) {
            fillRock(xl, yl);
            xl += Math.sign(xr - xl);
          }
          while (yl !== yr) {
            fillRock(xl, yl);
            yl += Math.sign(yr - yl);
          }
          left = right;
        }
      }
      return [grid, minX, maxX];

      function fillRock(x: number, y: number) {
        minX = Math.min(minX, x);
        maxX = Math.max(maxX, x);
        grid[y] ??= [];
        grid[y][x] = Item.Rock;
      }
    });
  }
}

console.log(new Solution().solve1());
console.log(new Solution().solve2());
