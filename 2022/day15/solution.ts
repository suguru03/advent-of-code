import * as path from 'path';
import { File } from '../utils/file';
import { Vector2 } from '../utils/vector';
import { moveCursor } from 'readline';

interface Info {
  sensor: Vector2;
  beacon: Vector2;
}

enum Item {
  Sensor = 'S',
  Beacon = 'B',
  Signal = '#',
  Empty = '.',
}
class Solution {
  solve1() {
    let target = 2000000;
    const list = this.parse();
    const row: Record<number, Item> = {};
    for (const { sensor, beacon } of list) {
      fill(sensor, Item.Sensor, true);
      fill(beacon, Item.Beacon, true);
      const total = Math.abs(sensor.x - beacon.x) + Math.abs(sensor.y - beacon.y);
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
        const [, xs, xy, xb, yb] = row.match(/.+x=(.+), y=(.+):.+x=(.+), y=(.+)/)?.map((num) => Number(num)) ?? [];
        return { sensor: new Vector2(xs, xy), beacon: new Vector2(xb, yb) };
      }),
    );
  }
}

console.log(new Solution().solve1());
