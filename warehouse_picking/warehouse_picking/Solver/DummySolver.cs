using System.Collections.Generic;

namespace warehouse_picking.Solver
{
    internal class DummySolver : ISolver
    {
        public DummySolver(Warehouse warehouse, IClientWish clientWish)
        {
            ClientWish = clientWish;
            Warehouse = warehouse;
        }

        private Warehouse Warehouse { get; set; }
        private IClientWish ClientWish { get; set; }

        public ISolution Solve()
        {
            var lastPositionOnTheAisle = Warehouse.NbBlock*(Warehouse.AisleLenght + 2) - 2;
                // one for Top, one for last position
            var solution = new DummySolution();
            var initShiftPoint = new ShiftPoint(0, 0);
            var shiftPointList = new List<ShiftPoint> {initShiftPoint};
            int lastX = 0;
            for (int i = 1; i < Warehouse.NbAisles + 1; i++)
            {
                if (i%2 == 1)
                {
                    var x = ((i - 1)/2)*3 + 1;
                    var goToX = new ShiftPoint(x, 0);
                    shiftPointList.Add(goToX);
                    var goToLastPositionOnTheAisle = new ShiftPoint(x, lastPositionOnTheAisle);
                    shiftPointList.Add(goToLastPositionOnTheAisle);
                    lastX = x;
                }
                else
                {
                    var goToX = new ShiftPoint(lastX + 1, lastPositionOnTheAisle);
                    shiftPointList.Add(goToX);
                    var goToBottom = new ShiftPoint(lastX + 1, 0);
                    shiftPointList.Add(goToBottom);
                }
            }
            if (Warehouse.NbAisles%2 == 1)
            {
                var goToBottom = new ShiftPoint(lastX, 0);
                shiftPointList.Add(goToBottom);
            }
            shiftPointList.Add(initShiftPoint);
            solution.ShiftPointList = shiftPointList;
            return solution;
        }
    }
}
