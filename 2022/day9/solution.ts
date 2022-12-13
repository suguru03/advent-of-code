import * as path from 'path';
import { File } from '../utils/file';
import { Vector2 } from '../utils/vector';

enum Direction {
  Left,
  Right,
  Up,
  Down,
}

interface Operation {
  direction: Direction;
  count: number;
}

const directionMap: Record<string, Direction> = {
  L: Direction.Left,
  R: Direction.Right,
  U: Direction.Up,
  D: Direction.Down,
};

const vectorMap: Record<Direction, Vector2> = {
  [Direction.Left]: Vector2.left,
  [Direction.Right]: Vector2.right,
  [Direction.Up]: Vector2.up,
  [Direction.Down]: Vector2.down,
};

class Solution {
  solve1() {
    const operations = this.parse();
    let head = Vector2.zero;
    let tail = Vector2.zero;
    const seen = new Set<string>([tail.id]);
    for (const { direction, count } of operations) {
      for (let i = 0; i < count; i++) {
        const next = head.add(vectorMap[direction]);
        if (next.distance(tail) >= 2) {
          tail = head;
          seen.add(tail.id);
        }
        head = next;
      }
    }
    return seen.size;
  }

  private parse(): Operation[] {
    return File.parse(path.resolve(__dirname, 'input.txt'), (file) =>
      file.split(/\n/g).map((row) => {
        const [key, count] = row.split(/\s/);
        return {
          direction: directionMap[key],
          count: Number(count),
        };
      }),
    );
  }
}

console.log(new Solution().solve1());
