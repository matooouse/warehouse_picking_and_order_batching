using System.Collections.Generic;
using warehouse_picking_core;

namespace warehouse_picking.Tests
{
    internal class DummyPickings : IPickings
    {
        public List<PickingPos> PickingList { get; set; }
    }
}
