import * as fs from 'fs';
import * as path from 'path';

type Grid = number[][];

const file = fs.readFileSync(path.resolve(__dirname, 'input.txt'), 'utf8');
const grid = parseInput(file);
console.log(countTree(grid));
function parseInput(file: string) {
  return file
    .trim()
    .split(/\n/g)
    .map((row) => row.split('').map((char) => Number(char)));
}

function countTree(grid: Grid) {
  const yl = grid.length;
  const xl = grid[yl - 1].length;
  const seen = Array.from(grid, (row) => Array(row.length).fill(0));
  for (let y = 0; y < yl; y++) {
    find(0, y, 1, 0, -1);
    find(xl - 1, y, -1, 0, -1);
  }
  for (let x = 0; x < xl; x++) {
    find(x, 0, 0, 1, -1);
    find(x, yl - 1, 0, -1, -1);
  }
  return seen.reduce((sum, row) => row.reduce((acc, num) => acc + num, sum), 0);

  function find(x: number, y: number, dx: number, dy: number, prev: number) {
    const cur = grid[y]?.[x] ?? null;
    if (cur === null) {
      return;
    }

    if (cur > prev) {
      seen[y][x] = 1;
    }

    find(x + dx, +y + dy, dx, dy, Math.max(cur, prev));
  }
}
