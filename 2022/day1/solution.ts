import * as fs from 'fs';
import * as path from 'path';

const file = fs.readFileSync(path.resolve(__dirname, 'input.txt'), 'utf8');

const nums = parseInput(file);
console.log(findMax(nums));
console.log(getSumOfTop3(nums));

function parseInput(file: string) {
  const list: number[][] = [];
  const rows = file.split('\n');
  let cur: number[] = [];
  for (const row of rows) {
    if (row === '') {
      list.push(cur);
      cur = [];
      continue;
    }

    cur.push(Number(row));
  }

  return list;
}

function findMax(nums: number[][]) {
  return Math.max(...nums.map((row) => row.reduce((acc, num) => acc + num)));
}

function getSumOfTop3(nums: number[][]) {
  return nums
    .map((row) => row.reduce((acc, num) => acc + num))
    .sort((n1, n2) => n2 - n1)
    .slice(0, 3)
    .reduce((acc, sum) => acc + sum);
}
