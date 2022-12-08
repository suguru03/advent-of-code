import * as fs from 'fs';
import * as path from 'path';

const file = fs.readFileSync(path.resolve(__dirname, 'input.txt'), 'utf8');
const rows = parseInput(file);
console.log(getSum(rows));

interface Row {
  left: string;
  right: string;
}

function parseInput(file: string) {
  const list: Row[] = [];
  const rows = file.split(/\n/g);
  for (const row of rows) {
    if (row == '') {
      continue;
    }
    const index = (row.length / 2) | 0;
    list.push({ left: row.slice(0, index), right: row.slice(index) });
  }

  return list;
}

function getSum(nums: Row[]) {
  return nums.reduce((acc, row) => {
    const leftSet = new Set(row.left);
    const rightSet = new Set(row.right);
    return Array.from(rightSet).reduce((acc, c) => acc + (leftSet.has(c) ? getScore(c) : 0), acc);
  }, 0);
}

function getScore(c: string) {
  const code = c.charCodeAt(0);
  return c === c.toUpperCase() ? code - 64 + 26 : code - 96;
}
