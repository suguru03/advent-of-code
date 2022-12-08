import * as fs from 'fs';
import * as path from 'path';
import * as assert from 'assert';

const file = fs.readFileSync(path.resolve(__dirname, 'input.txt'), 'utf8');
const str = parseInput(file);

interface Operation {
  command: string;
  outputs: string[];
}

function parseInput(file: string) {
  const rows = file.split(/\n/g);
  const operations: Operation[] = [];
  let operation: Operation | null = null;
  for (const row of rows) {
    if (row === '') {
      continue;
    }

    if (row.startsWith('$')) {
      if (operation) {
        operations.push(operation);
      }
      const command = row.match(/\$(.+)/)?.[1].trim();
      assert.ok(command);
      operation = {
        command,
        outputs: [],
      };
      continue;
    }

    operation?.outputs.push(row);
  }

  if (operation) {
    operations.push(operation);
  }

  return operations;
}

class File {
  get size() {
    return this.fileSize;
  }
  constructor(readonly name: string, protected fileSize: number) {}
}
class Dir extends File {
  private readonly children = new Map<string, File>();
  get size() {
    this.fileSize ||= Array.from(this.children.values()).reduce((acc, file) => acc + file.size, 0);
    return this.fileSize;
  }

  constructor(readonly name: string) {
    super(name, 0);
  }

  getChildren() {
    return Array.from(this.children.values());
  }

  getChild(name: string) {
    return this.children.get(name);
  }

  setChild(file: File) {
    this.children.set(file.name, file);
    this.fileSize = 0;
    return this;
  }
}

console.log(getTotal(str));

function getTotal(operations: Operation[]) {
  const root = new Dir('root');
  const stack: Dir[] = [root];
  for (const op of operations) {
    const current = stack[stack.length - 1];
    if (op.command.startsWith('cd')) {
      const dirName = op.command.match(/cd (.+)/)?.[1].trim();
      if (dirName == '..') {
        stack.pop();
        continue;
      }
      assert.ok(dirName);
      const next = current.getChild(dirName) ?? new Dir(dirName);
      assert.ok(next instanceof Dir);
      current.setChild(next);
      stack.push(next);
      continue;
    }

    if (op.command.startsWith('ls')) {
      for (const output of op.outputs) {
        const [sizeOrDir, name] = output.split(' ');
        if (sizeOrDir == 'dir') {
          current.setChild(new Dir(name));
        } else {
          current.setChild(new File(name, Number(sizeOrDir)));
        }
      }
      continue;
    }

    throw new Error('Unknown command');
  }

  return getSum(root, 100_000);
}

function getSum(dir: Dir | File, threshold: number) {
  if (!(dir instanceof Dir)) {
    return 0;
  }
  return dir.getChildren().reduce((acc, file) => acc + getSum(file, threshold), dir.size > threshold ? 0 : dir.size);
}
