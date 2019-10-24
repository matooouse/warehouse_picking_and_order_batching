namespace warehouse_picking
{
    internal class SShapeSolver : ISolver
    {
        public Warehouse Warehouse { get; private set; }
        public ClientWish ClientWish { get; private set; }
        public SShapeSolver(Warehouse currentWarehouse, ClientWish currentClientWish)
        {
            Warehouse = currentWarehouse;
            ClientWish = currentClientWish;
        }

        public ISolution Solve()
        {
            return null;
        }
    }
}
