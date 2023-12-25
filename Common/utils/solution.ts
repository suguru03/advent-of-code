import * as path from 'node:path';

export abstract class BaseSolution {
  protected filepath: string;
  constructor(filepath?: string) {
    this.filepath = filepath?.includes(process.cwd())
      ? filepath
      : path.resolve(
          new Error()
            .stack!.split('\n')[2]
            .match(/\((.*)\)/)![1]
            .split('/')
            .slice(0, -1)
            .join('/'),
          filepath ?? 'input.txt',
        );
  }
}
