import * as fs from 'fs';
import * as path from 'path';
import * as assert from 'assert';

const file = fs.readFileSync(path.resolve(__dirname, 'input.txt'), 'utf8');
const str = parseInput(file);
console.log(findMarkerIndex1(str));
console.log(findMarkerIndex2(str));

function parseInput(file: string) {
  return file.trim();
}

function findMarkerIndex1(str: string) {
  return findMarkerIndex(str, 4);
}

function findMarkerIndex2(str: string) {
  return findMarkerIndex(str, 14);
}

function findMarkerIndex(str: string, targetSize: number) {
  const indexMap = new Map<string, number>();
  let index = -1;
  let left = -1;
  while (index - left < targetSize && ++index < str.length) {
    const char = str.charAt(index);
    if (indexMap.has(char)) {
      left = Math.max(left, indexMap.get(char)!);
    }
    indexMap.set(char, index);
  }

  return index + 1;
}

assert.strictEqual(findMarkerIndex1('mjqjpqmgbljsphdztnvjfqwrcgsmlb'), 7);
assert.strictEqual(findMarkerIndex1('bvwbjplbgvbhsrlpgdmjqwftvncz'), 5);
assert.strictEqual(findMarkerIndex1('nppdvjthqldpwncqszvftbrmjlhg'), 6);
assert.strictEqual(findMarkerIndex1('nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg'), 10);
assert.strictEqual(findMarkerIndex1('zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw'), 11);

assert.strictEqual(findMarkerIndex2('mjqjpqmgbljsphdztnvjfqwrcgsmlb'), 19);
assert.strictEqual(findMarkerIndex2('bvwbjplbgvbhsrlpgdmjqwftvncz'), 23);
assert.strictEqual(findMarkerIndex2('nppdvjthqldpwncqszvftbrmjlhg'), 23);
assert.strictEqual(findMarkerIndex2('nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg'), 29);
assert.strictEqual(findMarkerIndex2('zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw'), 26);
