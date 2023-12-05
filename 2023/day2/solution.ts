import * as assert from 'node:assert';

import { File } from '../utils/file';
import { BaseSolution } from '../utils/solution';

interface Data {
  gameId: number;
  cubeMaps: Map<string, number>[];
}

class Solution extends BaseSolution {
  solve1() {
    const limitMap = new Map([
      ['red', 12],
      ['green', 13],
      ['blue', 14],
    ]);
    return this.parse().reduce(
      (sum, row) =>
        row.cubeMaps.every((cubeMap) =>
          Array.from(cubeMap).every(([cube, num]) => {
            const limit = limitMap.get(cube) ?? 0;
            return num <= limit;
          }),
        )
          ? sum + row.gameId
          : sum,
      0,
    );
  }

  solve2() {
    return this.parse().reduce((sum, row) => {
      const maxMap = new Map<string, number>();
      for (const cubeMap of row.cubeMaps) {
        for (const [cube, count] of cubeMap) {
          maxMap.set(cube, Math.max(maxMap.get(cube) ?? 0, count));
        }
      }
      return sum + Array.from(maxMap.values()).reduce((sum, row) => sum * row);
    }, 0);
  }

  private parse(): Data[] {
    return File.parse(this.filepath, (file) =>
      file.split('\n').map((row) => {
        const [game, data] = row.split(':');
        const [, gameIdStr] = game.match(/(\d+)/) ?? [];
        return {
          gameId: Number(gameIdStr),
          cubeMaps: data.split(';').map((str) => {
            return new Map(
              str.split(',').map((s) => {
                const [num, cube] = s.trim().split(' ');
                return [cube, Number(num)];
              }),
            );
          }),
        };
      }),
    );
  }
}

assert.strictEqual(new Solution('example.txt').solve1(), 8);
console.log(new Solution().solve1());
assert.strictEqual(new Solution('example.txt').solve2(), 2286);
console.log(new Solution().solve2());
