import * as fs from 'fs';
import * as path from 'path';
import * as assert from 'assert';

const file = fs.readFileSync(path.resolve(__dirname, 'input.txt'), 'utf8');
const rows = parseInput(file);
console.log(execute(rows));

interface Operation {
  move: number;
  from: number;
  to: number;
}

interface Data {
  stacks: string[][];
  operations: Operation[];
}

function parseInput(file: string) {
  const data: Data = {
    stacks: [],
    operations: [],
  };
  const rows = file.split(/\n/g);
  const index = rows.findIndex((row) => /\d/.test(row));
  for (const row of rows.slice(0, index)) {
    const list = row.match(/(\s{4}|[A-Z])/g) ?? [];
    for (const [i, char] of list.entries()) {
      if (!/[A-Z]/.test(char)) {
        continue;
      }
      data.stacks[i] ??= [];
      data.stacks[i].unshift(char);
    }
  }

  for (const row of rows.slice(index + 2)) {
    if (row === '') {
      continue;
    }

    const [move, from, to] = row.match(/\d+/g)?.map((str) => Number(str)) ?? [];
    data.operations.push({ move, from, to });
  }

  return data;
}

function execute({ stacks, operations }: Data) {
  for (const op of operations) {
    const from = stacks[op.from - 1];
    const to = stacks[op.to - 1];
    to.push(...from.splice(-op.move));
  }

  return stacks.map((stack) => stack.pop()).join('');
}
