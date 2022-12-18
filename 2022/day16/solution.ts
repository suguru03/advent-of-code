import * as path from 'path';
import { File } from '../utils/file';

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

interface Status {
  valve: Valve;
  delay: number;
  history?: string;
}

class Solution {
  private start = 'AA';
  solve1() {
    const map = this.parse();
    const flatMap = this.flat(map);
    let max = 0;
    const timeLimit = 30;
    const openedSet = new Set<Valve>();
    open(this.start, 0, 0, -1);
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

  solve2() {
    const map = this.parse();
    const flatMap = this.flat(map);
    let max = 0;
    const timeLimit = 26;
    const openedSet = new Set<Valve>([this.start]);
    const status = { valve: this.start, delay: 0, history: this.start };
    open(status, { ...status }, 0, 0, -1);
    return max;

    function open(current1: Status, current2: Status, score: number, total: number, time: number) {
      current1 = { ...current1 };
      current2 = { ...current2 };
      if (time === timeLimit) {
        max = Math.max(max, total);
        return;
      }

      if (current1.delay < 0 && current2.delay < 0) {
        max = Math.max(max, total + (timeLimit - time) * score);
        return;
      }

      time++;
      total += score;

      let data1 = flatMap.get(current1.valve)!;
      let data2 = flatMap.get(current2.valve)!;
      let update1 = false;
      let update2 = false;
      if (--current1.delay === -1) {
        score += data1.rate;
        update1 = true;
      }
      if (--current2.delay === -1) {
        score += data2.rate;
        update2 = true;
      }

      if (!update1 && !update2) {
        open(current1, current2, score, total, time);
        return;
      }
      if (!update1 && update2) {
        [data1, data2] = [data2, data1];
        [current1, current2] = [current2, current1];
        [update1, update2] = [update2, update1];
      }

      if (update1 && !update2) {
        let opened = false;
        for (const [next, distance] of data1.distanceMap) {
          if (openedSet.has(next)) {
            continue;
          }
          opened = true;
          openedSet.add(next);
          open({ valve: next, delay: distance }, current2, score, total, time);
          openedSet.delete(next);
        }
        if (!opened) {
          open(current1, current2, score, total, time);
        }
        return;
      }

      let opened1 = false;
      for (const [n1, d1] of data1.distanceMap) {
        if (openedSet.has(n1)) {
          continue;
        }
        opened1 = true;
        openedSet.add(n1);
        const next1 = { valve: n1, delay: d1 };

        let opened2 = false;
        for (const [n2, d2] of data2.distanceMap) {
          if (openedSet.has(n2)) {
            continue;
          }
          opened2 = true;
          openedSet.add(n2);
          open(next1, { valve: n2, delay: d2 }, score, total, time);
          openedSet.delete(n2);
        }
        if (!opened2) {
          open(next1, current2, score, total, time);
        }
        openedSet.delete(n1);
      }
      if (opened1) {
        return;
      }

      let opened2 = false;
      for (const [n2, d2] of data2.distanceMap) {
        if (openedSet.has(n2)) {
          continue;
        }
        opened2 = true;
        openedSet.add(n2);
        open(current1, { valve: n2, delay: d2 }, score, total, time);
        openedSet.delete(n2);
      }
      if (opened2) {
        return;
      }
      open(current1, current2, score, total, time);
    }
  }

  private flat(map: Map<Valve, Data>): Map<Valve, FlatData> {
    const flatMap = new Map<Valve, FlatData>();
    resolve();

    const ignoreMap = Array.from(map.values())
      .filter((data, i) => data.rate === 0)
      .map((data) => data.valve);
    for (const data of flatMap.values()) {
      for (const target of ignoreMap) {
        data.distanceMap.delete(target);
      }
    }

    return flatMap;

    function resolve() {
      for (const [valve, data] of map) {
        distanceMap: new Map<Valve, Distance>(data.valves.map((valve) => [valve, 1])),
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
console.log(new Solution().solve2());
