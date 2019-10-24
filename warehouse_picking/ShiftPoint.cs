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

        public override string ToString()
        {
            return "X : " + X + ", Y : " + Y;
        }
    }
}
