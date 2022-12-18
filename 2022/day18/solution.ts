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

enum Item {
  Unknown,
  Cube,
  Air,
}

class Solution {
  solve1() {
    const rows = this.parse();
    const cubes: number[][][] = [];
    for (const [x, y, z] of rows) {
      cubes[z] ??= [];
      cubes[z][y] ??= [];
      cubes[z][y][x] = 1;
    }

    let count = 0;
    for (const row of rows) {
      for (const d of delta) {
        const [x, y, z] = row.map((n, i) => n + d[i]);
        if (cubes[z]?.[y]?.[x] === 1) {
          continue;
        }
        count++;
      }
    }

    return count;
  }

  solve2() {
    const rows = this.parse();
    const cubes: number[][][] = [];
    const minList = [Infinity, Infinity, Infinity];
    const maxList = [-Infinity, -Infinity, -Infinity];
    for (const row of rows) {
      fill(...row, Item.Cube);
      for (const [i, val] of row.entries()) {
        minList[i] = Math.min(minList[i], val - 1);
        maxList[i] = Math.max(maxList[i], val + 1);
      }
    }
    let count = 0;
    find(minList);
    return count;

    function find(row: number[]) {
      const queue = [row];
      while (queue.length) {
        const row = queue.shift()!;
        if (row.some((n, i) => n < minList[i] || n > maxList[i])) {
          continue;
        }
        const [x, y, z] = row;
        const item = cubes[z]?.[y]?.[x] ?? Item.Unknown;
        if (item > Item.Unknown) {
          if (item === Item.Cube) {
            count++;
          }
          continue;
        }

        fill(x, y, z, Item.Air);
        for (const d of delta) {
          queue.push(row.map((n, i) => n + d[i]));
        }
      }

      return false;
    }

    function fill(x: number, y: number, z: number, item: Item) {
      cubes[z] ??= [];
      cubes[z][y] ??= [];
      cubes[z][y][x] = item;
    }

    function logCubes(cubes: number[][][]) {
      for (const [z, c] of cubes.entries()) {
        console.log(`#### ${z} ####`);
        let str = '';
        for (let y = minList[1]; y <= maxList[1]; y++) {
          for (let x = minList[0]; x <= maxList[0]; x++) {
            str += c?.[y]?.[x] ?? Item.Unknown;
          }
          str += '\n';
        }
        console.log(str.trim());
      }
    }
  }

  private parse(): [number, number, number][] {
    return File.parse(path.resolve(__dirname, 'input.txt'), (file) =>
      file.split('\n').map((row) => row.split(',').map((str) => Number(str)) as [number, number, number]),
    );
  }
}

console.log(new Solution().solve1());
console.log(new Solution().solve2());
