using System.Collections.Generic;
using System.Drawing;

namespace warehouse_picking_core
{
    public interface ISolution
    {
        List<ShiftPoint> ShiftPointList { get;  }
        Color Color { get; }
        int Length();
    }
}
