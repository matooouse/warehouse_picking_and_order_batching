namespace warehouse_picking
{
    internal class Warehouse
    {
        public int NbBlock { get; set; }
        public int NbAisles { get; set; }
        public int AisleLenght { get; set; }

        public Warehouse(int nbBlock, int nbAisles, int aisleLenght)
        {
            NbBlock = nbBlock;
            NbAisles = nbAisles;
            AisleLenght = aisleLenght;
        }
    }
}
