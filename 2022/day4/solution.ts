import * as fs from 'fs';
import * as path from 'path';
import * as assert from 'assert';

const file = fs.readFileSync(path.resolve(__dirname, 'input.txt'), 'utf8');
const rows = parseInput(file);
console.log(getFullyContain(rows));

interface Pair {
  start: number;
  end: number;
}

interface Row {
  left: Pair;
  right: Pair;
}

function parseInput(file: string) {
  const list: Row[] = [];
  const rows = file.split(/\n/g);
  for (const row of rows) {
    if (row == '') {
      continue;
    }
    const [left, right] = row.split(',').map((str) => {
      const [start, end] = str.split('-').map((str) => Number(str));
      return { start, end };
    });
    list.push({ left, right });
  }

  return list;
}

function getFullyContain(rows: Row[]) {
  return rows.filter(({ left, right }) => {
    [left, right] = [left, right].sort((l1, l2) => l1.start - l2.start || l2.end - l1.end);
    return right.start <= left.end && right.end <= left.end;
  }).length;
}

assert.deepStrictEqual(getFullyContain([{ left: { start: 2, end: 8 }, right: { start: 3, end: 7 } }]), 1);
assert.deepStrictEqual(getFullyContain([{ left: { start: 6, end: 6 }, right: { start: 4, end: 6 } }]), 1);
assert.deepStrictEqual(getFullyContain([{ left: { start: 20, end: 87 }, right: { start: 20, end: 88 } }]), 1);
