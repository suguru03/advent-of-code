import * as path from 'path';
import { File } from '../utils/file';

type Valve = string;
type Rate = number;

interface Data {
  valve: Valve;
  rate: Rate;
  valves: Valve[];
}

interface Score {
  valve: Valve;
  depth: number;
  max: number;
}

class Solution {
  solve1() {
    const map = this.parse();
    let sum = 0;
    let releasing = 0;
    let current = 'AA';
    let next = '';
    const availableSet = new Set<Valve>(Array.from(map.keys()));
    let min = 30;
    let moveCount = 0;
    while (min-- >= 0) {
      sum += releasing;
      if (moveCount > 0) {
        moveCount--;
        console.log('_(:3」∠)_________________', { current, next, sum, releasing, moveCount });
        continue;
      }

      if (next !== '') {
        availableSet.delete(next);
        releasing += map.get(next)!.rate;
        [current, next] = [next, ''];
      }
      const score = findHighestScore(current);
      if (score === null) {
        continue;
      }
      next = score.valve;
      moveCount = score.depth;
      console.log('_(:3」∠)_________________', { current, next, sum, releasing, moveCount });
    }

    return sum;

    function findHighestScore(current: Valve) {
      const scores: Score[] = [];
      const seen = new Set<Valve>();

      const queue = [current];
      let depth = 0;
      while (queue.length) {
        let target = '';
        let targetDepth = 0;
        let max = 0;
        let count = queue.length;
        let prevLoseScore = Math.max(0, ...scores.map((s) => (depth - s.depth) * s.max));
        // console.log('_(:3」∠)_________________depth', depth);
        while (--count >= 0) {
          const cur = queue.shift()!;
          if (seen.has(cur)) {
            continue;
          }
          seen.add(cur);
          const data = map.get(cur)!;
          if (availableSet.has(cur)) {
            const score = data.rate;
            // console.log('_(:3」∠)_________________score', { cur, score, max, nextMax });
            if (score > max) {
              target = cur;
              max = data.rate;
              targetDepth = depth;
            }
          }
          queue.push(...data.valves);
        }
        scores.push({ valve: target, depth, max });
        depth++;
      }
      if (current === 'HH') {
        console.log('_(:3」∠)_________________', availableSet);
        console.log('_(:3」∠)_________________', scores);
      }

      let highestScore: Score | null = null;
      for (const score of scores) {
        if (highestScore == null) {
          highestScore = score;
          continue;
        }
        const prev = (score.depth - highestScore.depth + 1) * highestScore.max;
        // console.log('_(:3」∠)_________________', { highestScore, score, prev });
        if (score.max > prev) {
          highestScore = score;
        }
      }

      return highestScore;
    }
  }

  private parse(): Map<Valve, Data> {
    return File.parse(
      path.resolve(__dirname, 'input.txt'),
      (file) =>
        new Map(
          file.split(/\n/g).map((row) => {
            const [, valve, rate, valves] = row.match(/Valve (.+) has .+ rate=(\d+);.+ valves? (.+)/) ?? [];
            const id = valve.trim();
            return [
              id,
              {
                valve: id,
                rate: Number(rate.trim()),
                valves: valves.split(',').map((item) => item.trim()),
              },
            ];
          }),
        ),
    );
  }
}

console.log(new Solution().solve1());
