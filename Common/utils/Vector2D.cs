namespace advent_of_code.Common.utils;

public record Vector2D(int X, int Y)
{
    public static Vector2D Zero => new(0, 0);
    public static Vector2D Right => new(1, 0);
    public static Vector2D Left => new(-1, 0);
    public static Vector2D Up => new(0, 1);
    public static Vector2D Down => new(0, -1);

    public static Vector2D operator +(Vector2D lhs, Vector2D rhs)
    {
        return new Vector2D(lhs.X + rhs.X, lhs.Y + rhs.Y);
    }

    public static Vector2D operator -(Vector2D lhs, Vector2D rhs)
    {
        return new Vector2D(lhs.X - rhs.X, lhs.Y - rhs.Y);
    }

    public HashSet<Vector2D> GetAdjacentSet()
    {
        return
        [
            this + Up,
            this + Down,
            this + Left,
            this + Right
        ];
    }
}
