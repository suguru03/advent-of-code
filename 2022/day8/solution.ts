import * as fs from 'fs';
import * as path from 'path';

type Grid = number[][];

const file = fs.readFileSync(path.resolve(__dirname, 'input.txt'), 'utf8');
const grid = parseInput(file);
console.log(countTree(grid));
console.log(findHighestScore(grid));

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
function findHighestScore(grid: Grid) {
  const deltaList = [
    { x: -1, y: 0 },
    { x: 1, y: 0 },
    { x: 0, y: -1 },
    { x: 0, y: 1 },
  ];
  const yl = grid.length;
  const xl = grid[yl - 1].length;
  let max = 0;
  for (let y = 1; y < yl - 1; y++) {
    for (let x = 1; x < xl - 1; x++) {
      const threshold = grid[y][x];
      const sum = deltaList.reduce((acc, d) => acc * count(x + d.x, y + d.y, d.x, d.y, threshold), 1);
      max = Math.max(max, sum);
    }
  }
  return max;

  function count(x: number, y: number, dx: number, dy: number, threshold: number) {
    const cur = grid[y]?.[x] ?? null;
    if (cur === null) {
      return 0;
    }

    if (cur >= threshold) {
      return 1;
    }

    return 1 + count(x + dx, +y + dy, dx, dy, threshold);
  }
}
