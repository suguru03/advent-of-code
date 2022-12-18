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

interface Memo {
  pk: string;
  min: number;
  max: number;
}

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

  solve2() {
    const pattern = this.parse();
    const maxWidth = 7;
    const grid: number[][] = [];
    let il = -1;
    let ip = -1;
    const limit = 1_000_000_000_000;
    const memo: Memo[] = [];
    const memoIndexMap = new Map<string, number[]>();
    let foundCount = 0;
    let foundIndices: number[] = [];
    while (++il < limit) {
      const ir = il % rocks.length;
      const rock = rocks[ir];
      let x = 2;
      let y = grid.length + 4;
      const x0 = x;
      const y0 = y;
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

      const pk = [ir, x0 - x, y0 - y].join(':');
      if (!memoIndexMap.has(pk)) {
        foundCount = 0;
        foundIndices = [];
      } else {
        const indices = new Set(memoIndexMap.get(pk)!);
        for (let i = foundIndices.length - 1; i >= 0; i--) {
          const foundIndex = foundIndices[i];
          const targetIndex = foundIndex + foundCount;
          if (!indices.has(targetIndex)) {
            foundIndices.splice(i, 1);
          }
        }
        if (foundIndices.length !== 0) {
          foundCount++;
        } else if (ir === 0) {
          foundCount = 1;
          foundIndices = [...indices];
        } else {
          foundCount = 0;
          foundIndices = [];
        }
      }
      if (foundCount === rocks.length * 2) {
        const size = grid.length;
        let [foundIndex] = foundIndices;
        const lastIndex = memo.length - foundCount + 1;
        const repeats = memo.slice(foundIndex, lastIndex);
        const repeatSize = getMemoSize(repeats);
        const duplicateSize = getMemoSize(memo.slice(lastIndex));

        const div = repeats.length;
        const rest = limit - il + foundCount - 1;
        const times = Math.floor(rest / div);
        let remaining = rest % div;
        let result = size - duplicateSize + repeatSize * times;
        if (remaining !== 0) {
          result += getMemoSize(repeats.slice(0, remaining));
        }
        return result;
      }

      function getMemoSize(list: Memo[]) {
        if (list.length === 0) {
          return 0;
        }
        let min = Infinity;
        let max = -Infinity;
        for (const data of list) {
          min = Math.min(min, data.min);
          max = Math.max(max, data.max);
        }
        return max - min + 1;
      }

      let maxY = 0;
      for (const tile of rock) {
        const next = tile.add(new Vector2(x, y));
        grid[next.y] ??= Array(maxWidth).fill(0);
        grid[next.y][next.x] = 1;
        maxY = Math.max(maxY, next.y);
      }

      memo.push({ pk, min: y, max: maxY });
      memoIndexMap.set(pk, memoIndexMap.get(pk) ?? []);
      memoIndexMap.get(pk)?.push(il);

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
    const size = Math.ceil(Math.log10(grid.length + 1));
    const str = grid
      .reverse()
      .map((row, i) => i.toString().padStart(size, '0') + ':' + row.map((num, i) => (num === 0 ? '.' : '#')).join(''))
      .join('\n');
    console.log(str);
  }
  private parse(): string {
    return File.parse(path.resolve(__dirname, 'input.txt'), (file) => file);
  }
}

console.log(new Solution().solve1());
console.log(new Solution().solve2());
