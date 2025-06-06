using System;

namespace Runtime.Systems.GridSystem
{
    public struct GridPosition : IEquatable<GridPosition>
    {
        public int x;
        public int y;

        public GridPosition(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString() => $"{x},{y}";
        
        public readonly bool IsNearBy(GridPosition other)
        {
            return (other.x == x - 1 && other.y == y) ||
                   (other.x == x + 1 && other.y == y) ||
                   (other.y == y - 1 && other.x == x) ||
                   (other.y == y + 1 && other.x == x);
        }

        public bool Equals(GridPosition other)
        {
            return x == other.x && y == other.y;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }

        public override bool Equals(object obj)
        {
            return obj is GridPosition other && Equals(other);
        }
        public static bool operator ==(GridPosition a, GridPosition b)
        {
            return a.x == b.x && a.y == b.y;
        }
        public static bool operator !=(GridPosition a, GridPosition b)
        {
            return !(a == b);
        }
        public static GridPosition operator +(GridPosition a, GridPosition b)
        {
            return new GridPosition(a.x+b.x,a.y+b.y);
        }
        public static GridPosition operator -(GridPosition a, GridPosition b)
        {
            return new GridPosition(a.x-b.x,a.y-b.y);
        }
        
    }
}
