import * as path from 'path';
import { File } from '../utils/file';

class Node {
  prev: Node;
  next: Node;
  constructor(readonly val: number) {}
}
class Solution {
  solve1() {
    const head = this.parse();
    let node = head;
    const set = new Set<Node>();
    while (!set.has(node)) {
      set.add(node);
      node = node.next;
    }
    const queue = Array.from(set);
    while (queue.length !== 0) {
      let node = queue.shift()!;
      for (let i = 0; i < node.val; i++) {
        const { prev, next } = node;
        const nextNext = next.next;
        // prev, node, next, next.next -> prev, next, node, next.next
        [prev.next, next.prev, next.next, node.prev, node.next, nextNext.prev] = [next, prev, node, next, nextNext, node];
      }
      for (let i = 0; i > node.val; i--) {
        const { prev, next } = node;
        const prevPrev = prev.prev;
        // prev.prev, prev, node, next -> prev.prev, node, prev, next
        [prevPrev.next, node.prev, node.next, prev.prev, prev.next, next.prev] = [node, prevPrev, prev, node, next, prev];
      }
    }
    node = head;
    while (node.val !== 0) {
      node = node.next;
    }
    const indexSet = new Set([1_000 % set.size, 2_000 % set.size, 3_000 % set.size]);
    let sum = 0;
    for (let i = 0; i < set.size; i++) {
      if (indexSet.has(i)) {
        sum += node.val;
      }
      node = node.next;
    }
    return sum;
  }

  private logDLL(head: Node) {
    let node = head;
    const set = new Set<Node>();
    set.clear();
    const nums: number[] = [];
    while (!set.has(node)) {
      nums.push(node.val);
      set.add(node);
      node = node.next;
    }
    console.log(nums.join(', '))
  }

  private parse(): Node {
    return File.parse(path.resolve(__dirname, 'input.txt'), (file) => {
      let head = new Node(0);
      let node = head;
      for (const str of file.split('\n')) {
        const next = new Node(Number(str));
        [node.next, next.prev, node] = [next, node, next];
      }
      head = head.next;
      [head.prev, node.next] = [node, head];
      return head;
    });
  }
}

console.log(new Solution().solve1());
