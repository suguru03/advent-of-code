import * as path from 'path';
import { File } from '../utils/file';

interface Operation {
  command: string;
  count: number;
}

class Solution {
  solve1() {
    const operations = this.parse();
    const offset = 20;
    let cycle = -offset;
    let x = 1;
    let sum = 0;
    const queue: number[] = [];
    while (queue.length > 0 || operations.length > 0) {
      if (++cycle % 40 === 0) {
        sum += (cycle + offset) * x;
      }
      if (queue.length !== 0) {
        x += queue.shift()!;
        continue;
      }

      const op = operations.shift();
      if (op?.command === 'addx') {
        queue.push(op.count);
      }
    }
    return sum;
  }


  private parse(): Operation[] {
    return File.parse(path.resolve(__dirname, 'input.txt'), (file) =>
      file.split(/\n/g).map((row) => {
        const [command, count] = row.split(/\s/);
        return {
          command,
          count: Number(count ?? 0),
        };
      }),
    );
  }
}

console.log(new Solution().solve1());
