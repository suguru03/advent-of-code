import * as path from 'path';
import { File } from '../utils/file';

type Item = Node | number;
class Node {
  constructor(readonly list: Item[] = []) {}

  toString() {
    return `[${this.list.join(' ,')}]`;
  }
}

class Root {
  left: Node;
  right: Node;
}
class Solution {
  solve1() {
    const roots = this.parse();
    let sum = 0;
    for (const [index, { left, right }] of roots.entries()) {
      if (this.compare(left, right)) {
        sum += index + 1;
      }
    }

    return sum;
  }
  solve2() {
    const key1 = new Node([new Node([2])]);
    const key2 = new Node([new Node([6])]);
    const roots = this.parse();
    const nodes: Item[] = [key1, key2, ...roots.flatMap(({ left, right }) => [left, right])];
    nodes.sort((n1, n2) => {
      const result = this.compare(n1, n2);
      return result ? -1 : 1;
    });
    const i1 = nodes.findIndex((node) => node === key1);
    const i2 = nodes.findIndex((node) => node === key2);
    return (i1 + 1) * (i2 + 1);
  }

  private compare(left: Item, right: Item): boolean | null {
    const isLeftNode = left instanceof Node;
    const isRightNode = right instanceof Node;
    const ln = isLeftNode ? left : new Node([left]);
    const rn = isRightNode ? right : new Node([right]);
    let li = 0;
    let ri = 0;
    while (li < ln.list.length && ri < rn.list.length) {
      const l = ln.list[li];
      const r = rn.list[ri];
      if (l instanceof Node || r instanceof Node) {
        const result = this.compare(l, r);
        if (result !== null) {
          return result;
        }
        li++;
        ri++;
        continue;
      }

      if (l === r) {
        li++;
        ri++;
        continue;
      }

      return l < r;
    }

    if (li === ln.list.length && ri === rn.list.length) {
      return null;
    }

    return li === ln.list.length;
  }

  private parse(): Root[] {
    return File.parse(path.resolve(__dirname, 'input.txt'), (file) => {
      let root = new Root();
      const roots: Root[] = [];
      const rows = file.split(/\n/g);
      for (let row of rows) {
        if (row === '') {
          roots.push(root);
          root = new Root();
          continue;
        }

        const head = new Node();
        let node = head;
        let cur = '';
        const stack: Node[] = [];
        for (const c of row) {
          switch (c) {
            case '[': {
              stack.push(node);
              const next = new Node();
              node.list.push(next);
              node = next;
              break;
            }
            case ']': {
              if (cur) {
                node.list.push(this.toNumber(cur));
              }
              cur = '';
              node = stack.pop()!;
              break;
            }
            case ',': {
              if (cur) {
                node.list.push(this.toNumber(cur));
              }
              cur = '';
              break;
            }
            default: {
              cur += c;
              break;
            }
          }
        }
        node = head.list[0] as Node;
        if (!root.left) {
          root.left = node;
        } else {
          root.right = node;
        }
      }

      roots.push(root);

      return roots;
    });
  }

  private toNumber(str: string) {
    const num = Number(str);
    if (isNaN(num)) {
      throw new Error(`invalid ${str}`);
    }
    return num;
  }
}

console.log(new Solution().solve1());
console.log(new Solution().solve2());
