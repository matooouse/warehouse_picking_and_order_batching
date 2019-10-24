namespace warehouse_picking
{
    internal interface ISolver
    {
        Warehouse Warehouse { get; }
        ClientWish ClientWish { get; }
        ISolution Solve();
    }
}
