using System;

namespace warehouse_picking
{
    internal class ShiftPoint
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public ShiftPoint(int x, int y)
        {
            Y = y;
            X = x;
        }

        public override int GetHashCode()
        {
            return X*Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            var other = (ShiftPoint)obj;
            return X == other.X && Y == other.Y;
        }

        public override string ToString()
        {
            return "X : " + X + ", Y : " + Y;
        }
    }
}
