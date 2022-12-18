import * as path from 'path';
import { File } from '../utils/file';
import { start } from 'repl';

type Valve = string;
type Rate = number;
type Distance = number;

interface Data {
  valve: Valve;
  rate: Rate;
  valves: Valve[];
}

interface FlatData {
  valve: Valve;
  rate: Rate;
  distanceMap: Map<Valve, Distance>;
}

interface Score {
  valve: Valve;
  depth: number;
  max: number;
}

class Solution {
  private start = 'AA';
  private dummyStart = '00';
  solve1() {
    const map = this.parse();
    const flatMap = this.flat(map);
    let max = 0;
    const timeLimit = 30;
    const openedSet = new Set<Valve>();
    open(this.dummyStart, 0, 0, -1);
    return max;

    function open(current: Valve, score: number, total: number, time: number) {
      if (time === timeLimit) {
        max = Math.max(max, total);
        return;
      }

      if (openedSet.has(current)) {
        max = Math.max(max, total + (timeLimit - time) * score);
        return;
      }

      const data = flatMap.get(current)!;
      openedSet.add(current);
      // open
      time++;
      total += score;
      score += data.rate;
      for (const [next, distance] of data.distanceMap) {
        if (time + distance > timeLimit) {
          const diff = timeLimit - time;
          open('', score, total + score * diff, timeLimit);
          continue;
        }
        open(next, score, total + score * distance, time + distance);
      }
      openedSet.delete(current);
    }
  }

  private flat(map: Map<Valve, Data>): Map<Valve, FlatData> {
    const flatMap = new Map<Valve, FlatData>();
    resolve();

    const ignoreMap = Array.from(map.values())
      .filter((data) => data.rate === 0)
      .map((data) => data.valve);
    for (const data of flatMap.values()) {
      for (const target of ignoreMap) {
        data.distanceMap.delete(target);
      }
    }

    const dummyStep: FlatData = {
      valve: this.dummyStart,
      rate: 0,
      distanceMap: new Map([[this.start, -1]]),
    };
    flatMap.set(dummyStep.valve, dummyStep);

    return flatMap;

    function resolve() {
      for (const [valve, data] of map) {
        flatMap.set(valve, {
          valve,
          rate: data.rate,
          distanceMap: new Map<Valve, Distance>(data.valves.map((valve) => [valve, 1])),
        });
      }
      const size = map.size - 1;
      const queue = Array.from(map.keys());
      while (queue.length) {
        const current = queue.shift()!;
        const data = map.get(current)!;
        const flatData = flatMap.get(current)!;
        for (const target of data.valves) {
          const targetFlatData = flatMap.get(target)!;
          for (const [child, distance] of targetFlatData.distanceMap) {
            if (current === child) {
              continue;
            }
            const min = Math.min(distance + 1, flatData.distanceMap.get(child) ?? Infinity);
            flatData.distanceMap.set(child, min);
          }

          for (const [child, distance] of flatData.distanceMap) {
            if (target === child) {
              continue;
            }
            const min = Math.min(distance + 1, targetFlatData.distanceMap.get(child) ?? Infinity);
            targetFlatData.distanceMap.set(child, min);
          }
          if (flatData.distanceMap.size === size) {
            continue;
          }
          queue.push(current);
        }
      }
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
