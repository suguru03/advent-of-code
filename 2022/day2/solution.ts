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

enum ElfGuide2 {
  X = 'Lose',
  Y = 'Draw',
  Z = 'Win',
}

const pointHandMap = {
  Rock: 1,
  Paper: 2,
  Scissors: 3,
};

enum Point {
  Win = 6,
  Draw = 3,
  Lose = 0,
}

const pointMap = {
  '-2': Point.Win,
  1: Point.Win,
  0: Point.Draw,
};

interface Row {
  guide: string;
  hand: string;
}

const rows = parseInput(file);
console.log(getScore1(rows));

function parseInput(file: string) {
  const list: Row[] = [];
  const rows = file.split(/\n/g);
  for (const row of rows) {
    if (row == '') {
      continue;
    }
    const [guide, hand] = row.split(/\s/);
    list.push({ guide, hand });
  }

  return list;
}

function getScore1(nums: Row[]) {
  return nums.reduce((acc, row) => {
    const guide = pointHandMap[Hand[ElfGuide[row.guide]]];
    const hand = pointHandMap[Hand[row.hand]];
    const diff = hand - guide;
    const point = pointMap[diff] ?? Point.Lose;
    return acc + hand + point;
  }, 0);
}

assert.strictEqual(getScore1([{ guide: 'A', hand: 'X' }]), 1 + 3);
assert.strictEqual(getScore1([{ guide: 'A', hand: 'Y' }]), 2 + 6);
assert.strictEqual(getScore1([{ guide: 'A', hand: 'Z' }]), 3);

assert.strictEqual(getScore1([{ guide: 'B', hand: 'X' }]), 1);
assert.strictEqual(getScore1([{ guide: 'B', hand: 'Y' }]), 2 + 3);
assert.strictEqual(getScore1([{ guide: 'B', hand: 'Z' }]), 3 + 6);

assert.strictEqual(getScore1([{ guide: 'C', hand: 'X' }]), 1 + 6);
assert.strictEqual(getScore1([{ guide: 'C', hand: 'Y' }]), 2);
assert.strictEqual(getScore1([{ guide: 'C', hand: 'Z' }]), 3 + 3);

console.log(getScore2(rows));

function getScore2(nums: Row[]) {
  return nums.reduce((acc, row) => {
    const guide = pointHandMap[Hand[ElfGuide[row.guide]]] - 1;
    const point = Point[ElfGuide2[row.hand]];
    assert.ok(typeof point === 'number');
    const hand = ((((point / 3) | 0) + guide - 1) % 3) + 1 || guide % 3 || 3;
    return acc + point + hand;
  }, 0);
}

assert.strictEqual(getScore2([{ guide: 'A', hand: 'X' }]), 3);
assert.strictEqual(getScore2([{ guide: 'A', hand: 'Y' }]), 1 + 3);
assert.strictEqual(getScore2([{ guide: 'A', hand: 'Z' }]), 2 + 6);

assert.strictEqual(getScore2([{ guide: 'B', hand: 'X' }]), 1);
assert.strictEqual(getScore2([{ guide: 'B', hand: 'Y' }]), 2 + 3);
assert.strictEqual(getScore2([{ guide: 'B', hand: 'Z' }]), 3 + 6);

assert.strictEqual(getScore2([{ guide: 'C', hand: 'X' }]), 2);
assert.strictEqual(getScore2([{ guide: 'C', hand: 'Y' }]), 3 + 3);
assert.strictEqual(getScore2([{ guide: 'C', hand: 'Z' }]), 1 + 6);
