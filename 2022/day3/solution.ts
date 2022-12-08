import * as fs from 'fs';
import * as path from 'path';

const file = fs.readFileSync(path.resolve(__dirname, 'input.txt'), 'utf8');
const rows = parseInput(file);
console.log(getSum1(rows));
console.log(getSum2(rows));

function parseInput(file: string) {
  const list: string[] = [];
  const rows = file.split(/\n/g);
  for (const row of rows) {
    if (row == '') {
      continue;
    }
    list.push(row);
  }

  return list;
}

function getSum1(rows: string[]) {
  return rows.reduce((acc, row) => {
    const index = (row.length / 2) | 0;
    const left = new Set(row.slice(0, index));
    const right = new Set(row.slice(index));
    return acc + getScoreBySet(findCommon(left, right));
  }, 0);
}

function findCommon(left: Set<string>, right: Set<string>) {
  return new Set(Array.from(left).filter((char) => right.has(char)));
}

function getSum2(rows: string[]) {
  let sum = 0;
  for (let i = 0; i < rows.length / 3; i++) {
    const left = new Set(rows[i * 3]);
    const mid = new Set(rows[i * 3 + 1]);
    const right = new Set(rows[i * 3 + 2]);
    sum += getScoreBySet(findCommon(findCommon(left, mid), right));
  }
  return sum;
}

function getScoreBySet(set: Set<string>) {
  return Array.from(set).reduce((acc, c) => acc + getScore(c), 0);
}

function getScore(c: string) {
  const code = c.charCodeAt(0);
  return c === c.toUpperCase() ? code - 64 + 26 : code - 96;
}
