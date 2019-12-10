using System;
using System.Collections.Generic;

namespace warehouse_picking_core
{
    internal static class  Helper
    {
        public static int Length_of_route(List<ShiftPoint> route)
        {
            int totalDistance = 0;
            for (int i = 0; i < route.Count - 1; i++)
            {
                var shiftPoint = route[i];
                var nextShiftPoint = route[i + 1];
                totalDistance += Math.Abs(nextShiftPoint.X - shiftPoint.X);
                totalDistance += Math.Abs(nextShiftPoint.Y - shiftPoint.Y);
            }
            return totalDistance;
        }
    }
}
