using System.Collections.Generic;
using System.Drawing;

namespace warehouse_picking
{
    class DummySolution : ISolution
    {
        public DummySolution()
        {
            Color = Color.Red;
        }

        public List<ShiftPoint> ShiftPointList { get; set; }
        public Color Color { get; set; }
    }
}
