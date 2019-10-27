using System.Collections.Generic;

namespace warehouse_picking.Tests
{
    internal class DummyClientWish : IClientWish
    {
        public List<ClientWishPos> ClientWishes { get; set; }
    }
}
