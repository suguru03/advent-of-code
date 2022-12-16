import * as path from 'path';
import { File } from '../utils/file';
import { Vector2 } from '../utils/vector';

interface Info {
  sensor: Vector2;
  beacon: Vector2;
  total: number;
}

enum Item {
  Sensor = 'S',
  Beacon = 'B',
  Signal = '#',
  Empty = '.',
}

class Solution {
  solve1(target = 2_000_000) {
    const list = this.parse();
    const row: Record<number, Item> = {};
    for (const { sensor, beacon, total } of list) {
      fill(sensor, Item.Sensor, true);
      fill(beacon, Item.Beacon, true);
      const sign = Math.sign(target - sensor.y);
      const maxY = sensor.y + sign * total;
      let rest = Math.abs(maxY - target);
      const diff = Math.sign(maxY - target);
      if (diff !== sign && diff !== 0) {
        continue;
      }
      const base = new Vector2(sensor.x, target);
      fill(base, Item.Signal, false);
      let left = base.left;
      let right = base.right;
      while (--rest >= 0) {
        fill(left, Item.Signal, false);
        fill(right, Item.Signal, false);
        left = left.left;
        right = right.right;
      }
    }

    let count = 0;
    const [minX, maxX] = this.getRange(row);
    for (let x = minX; x <= maxX; x++) {
      const item = row[x] ?? Item.Empty;
      count += Number(item === Item.Signal);
    }

    return count;

    function fill({ x, y }: Vector2, item: Item, override: boolean) {
      if (y !== target) {
        return;
      }
      row[x] = override ? item : row[x] ?? item;
    }
  }

  solve2() {
    const list = this.parse();
    const threshold = 4_000_000;
    for (let y = 0; y <= threshold; y++) {
      loop: for (let x = 0; x <= threshold; x++) {
        for (const { sensor, total } of list) {
          const distanceX = Math.abs(sensor.x - x);
          const distanceY = Math.abs(sensor.y - y);
          if (distanceX + distanceY > total) {
            continue;
          }
          x += total - distanceY - distanceX;
          continue loop;
        }

        return x * threshold + y;
      }
    }

    return -1;
  }

  private getRange(row: Record<number, Item>) {
    let minX = Infinity;
    let maxX = -Infinity;
    for (const x of Object.keys(row ?? {})) {
      minX = Math.min(minX, Number(x));
      maxX = Math.max(maxX, Number(x));
    }
    return [minX, maxX];
  }

  private logRow(row: Record<number, Item>) {
    const [min, max] = this.getRange(row);
    let str = '';
    for (let x = min; x <= max; x++) {
      str += row[x] ?? Item.Empty;
    }
    console.log(str.trim());
  }

  private parse(): Info[] {
    return File.parse(path.resolve(__dirname, 'input.txt'), (file) =>
      file.split(/\n/g).map((row) => {
        const [, xs, ys, xb, yb] = row.match(/.+x=(.+), y=(.+):.+x=(.+), y=(.+)/)?.map((num) => Number(num)) ?? [];
        const total = Math.abs(xs - xb) + Math.abs(ys - yb);
        return { sensor: new Vector2(xs, ys), beacon: new Vector2(xb, yb), total };
      }),
    );
  }
}

console.log(new Solution().solve1());
console.log(new Solution().solve2());
