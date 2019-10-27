using System.Collections.Generic;
using System.Drawing;

namespace warehouse_picking
{
    internal interface ISolution
    {
        List<ShiftPoint> ShiftPointList { get;  }
        Color Color { get; }
    }
}
