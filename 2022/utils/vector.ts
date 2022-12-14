export class Vector2 {
  static readonly zero = new Vector2(0, 0);
  static readonly left = new Vector2(-1, 0);
  static readonly right = new Vector2(1, 0);
  static readonly down = new Vector2(0, -1);
  static readonly up = new Vector2(0, 1);

  get id() {
    return this.toString();
  }

  get left() {
    return this.add(Vector2.left);
  }

  get right() {
    return this.add(Vector2.right);
  }

  get down() {
    return this.add(Vector2.down);
  }

  get up() {
    return this.add(Vector2.up);
  }

  constructor(readonly x: number, readonly y: number) {}

  add(left: Vector2) {
    return new Vector2(this.x + left.x, this.y + left.y);
  }

  distance(left: Vector2) {
    return Math.sqrt(Math.pow(this.x - left.x, 2)  + Math.pow(this.y - left.y, 2));
  }

  equal(left: Vector2) {
    return this.x === left.x && this.y === left.y;
  }

  toString() {
    return `${this.x}:${this.y}`;
  }
}
