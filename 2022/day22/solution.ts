import * as path from 'path';
import { File } from '../utils/file';

type Command = number | 'R' | 'L';

interface Data {
  start: Node;
  commands: Command[];
  grid: Node[][];
}

enum Direction {
  Right,
  Down,
  Left,
  Up,
}

enum Item {
  Empty = ' ',
  Tile = '.',
  Wall = '#',
}

class Node {
  left: Node;
  right: Node;
  up: Node;
  down: Node;
  constructor(readonly x: number, readonly y: number, readonly val: Item) {}

  toString() {
    return `${this.x}:${this.y}:${this.val}`;
  }
}

class Solution {
  solve1() {
    const directionSize = 4;
    const { start, grid, commands } = this.parse();
    let current = start;
    let direction = Direction.Right;
    for (const command of commands) {
      if (command === 'R') {
        direction = (direction + 1) % directionSize;
        continue;
      }
      if (command === 'L') {
        direction = (direction - 1 + directionSize) % directionSize;
        continue;
      }

      for (let i = 0; i < command; i++) {
        const next = this.next(current, direction);
        // (grid[current.y][current.x] as any).val = direction === Direction.Right ? '>' : direction === Direction.Down ? 'v' : direction === Direction.Left ? '<' : '^'
        if (!movable(next)) {
          break;
        }
        current = next;
      }
    }
    // this.logGrid(grid);

    return this.getScore(current, direction);

    function movable(current: Node) {
      return current.val !== Item.Wall;
    }
  }

  private getScore(current: Node, direction: Direction) {
    return (current.y + 1) * 1_000 + (current.x + 1) * 4 + direction;
  }

  private next(current: Node, direction: Direction) {
    switch (direction) {
      case Direction.Right: {
        return current.right;
      }
      case Direction.Down: {
        return current.down;
      }
      case Direction.Left: {
        return current.left;
      }
      case Direction.Up: {
        return current.up;
      }
    }
  }

  private logGrid(grid: Node[][]) {
    let str = '';
    for (const row of grid) {
      for (const node of row) {
        str += node.val;
      }
      str += '\n';
    }
    console.log(str);
  }

  private parse(): Data {
    return File.parse(
      path.resolve(__dirname, 'input.txt'),
      (file) => {
        const rows = file.split('\n');
        const index = rows.indexOf('');
        const grid = rows.slice(0, index).map((row, y) => Array.from(row, (char, x) => new Node(x, y, char as Item)));
        const start = grid[0].find((node) => node.val !== Item.Empty)!;
        for (const [y, row] of grid.entries()) {
          for (const [x, node] of row.entries()) {
            if (node.val === Item.Empty) {
              continue;
            }

            let up = y;
            do {
              up = (up - 1 + grid.length) % grid.length;
            } while (getValue(x, up) === Item.Empty);
            node.up = grid[up][x];

            let down = y;
            do {
              down = (down + 1) % grid.length;
            } while (getValue(x, down) === Item.Empty);
            node.down = grid[down][x];

            let left = x;
            do {
              left = (left - 1 + row.length) % row.length;
            } while (getValue(left, y) === Item.Empty);
            node.left = grid[y][left];

            let right = x;
            do {
              right = (right + 1) % row.length;
            } while (getValue(right, y) === Item.Empty);
            node.right = grid[y][right];
          }
        }

        const str = rows[index + 1];
        const commands: Command[] = [];
        let cur = '';
        for (const char of str) {
          switch (char) {
            case 'R':
            case 'L': {
              commands.push(Number(cur), char);
              cur = '';
              break;
            }
            default: {
              cur += char;
              break;
            }
          }
        }
        if (cur) {
          commands.push(Number(cur));
        }
        return {
          start,
          commands,
          grid,
        };

        function getValue(x: number, y: number) {
          return grid[y]?.[x]?.val ?? Item.Empty;
        }
      },
      false,
    );
  }
}

console.log(new Solution().solve1());
