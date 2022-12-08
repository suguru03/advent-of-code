import * as assert from 'assert';
import * as fs from 'fs';
import * as path from 'path';

const file = fs.readFileSync(path.resolve(__dirname, 'input.txt'), 'utf8');

enum Hand {
  X = 'Rock',
  Y = 'Paper',
  Z = 'Scissors',
}

enum ElfGuide {
  A = 'X',
  B = 'Y',
  C = 'Z',
}

const pointHandMap = {
  Rock: 1,
  Paper: 2,
  Scissors: 3,
};

const pointMap = {
  '-2': 6,
  1: 6,
  0: 3,
};

interface Row {
  guide: ElfGuide;
  hand: Hand;
}

const rows = parseInput(file);
console.log(getScore(rows));

function parseInput(file: string) {
  const list: Row[] = [];
  const rows = file.split(/\n/g);
  for (const row of rows) {
    if (row == '') {
      continue;
    }
    const [guide, hand] = row.split(/\s/);
    list.push({ guide: ElfGuide[guide], hand: Hand[hand] });
  }

  return list;
}

function getScore(nums: Row[]) {
  return nums.reduce((acc, row) => {
    const guide = pointHandMap[Hand[row.guide]];
    const hand = pointHandMap[row.hand];
    const diff = hand - guide;
    const point = pointMap[diff] ?? 0;
    return acc + hand + point;
  }, 0);
}

assert.strictEqual(getScore([{ guide: ElfGuide.A, hand: Hand.X }]), 1 + 3);
assert.strictEqual(getScore([{ guide: ElfGuide.A, hand: Hand.Y }]), 2 + 6);
assert.strictEqual(getScore([{ guide: ElfGuide.A, hand: Hand.Z }]), 3);

assert.strictEqual(getScore([{ guide: ElfGuide.B, hand: Hand.X }]), 1);
assert.strictEqual(getScore([{ guide: ElfGuide.B, hand: Hand.Y }]), 2 + 3);
assert.strictEqual(getScore([{ guide: ElfGuide.B, hand: Hand.Z }]), 3 + 6);

assert.strictEqual(getScore([{ guide: ElfGuide.C, hand: Hand.X }]), 1 + 6);
assert.strictEqual(getScore([{ guide: ElfGuide.C, hand: Hand.Y }]), 2);
assert.strictEqual(getScore([{ guide: ElfGuide.C, hand: Hand.Z }]), 3 + 3);
