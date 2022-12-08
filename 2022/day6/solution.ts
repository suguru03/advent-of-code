import * as fs from 'fs';
import * as path from 'path';
import * as assert from 'assert';

const file = fs.readFileSync(path.resolve(__dirname, 'input.txt'), 'utf8');
const str = parseInput(file);
console.log(findMarkerIndex(str));

function parseInput(file: string) {
  return file.trim();
}

function findMarkerIndex(str: string) {
  const indexMap = new Map<string, number>();
  const targetSize = 4;
  let index = -1;
  let left = -1;
  while (index - left < targetSize && ++index < str.length - targetSize) {
    const char = str.charAt(index);
    if (indexMap.has(char)) {
      left = Math.max(left, indexMap.get(char)!);
    }
    indexMap.set(char, index);
  }

  return index + 1;
}

assert.strictEqual(findMarkerIndex('mjqjpqmgbljsphdztnvjfqwrcgsmlb'), 7);
assert.strictEqual(findMarkerIndex('bvwbjplbgvbhsrlpgdmjqwftvncz'), 5);
assert.strictEqual(findMarkerIndex('nppdvjthqldpwncqszvftbrmjlhg'), 6);
assert.strictEqual(findMarkerIndex('nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg'), 10);
assert.strictEqual(findMarkerIndex('zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw'), 11);
