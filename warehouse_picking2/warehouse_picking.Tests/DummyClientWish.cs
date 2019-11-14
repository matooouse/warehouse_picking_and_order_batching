using System.Collections.Generic;

namespace warehouse_picking.Tests
{
    internal class DummyPickings : IPickings
    {
        public List<PickingPos> PickingList { get; set; }
    }
}
