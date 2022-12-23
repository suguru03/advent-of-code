import * as fs from 'fs';

export class File {
  static parse<T>(filepath: string, parser: (file: string) => T, trim = true) {
    const file = fs.readFileSync(filepath, 'utf8');
    return parser(trim ? file.trim() : file);
  }
}
