using System.Collections.Generic;

namespace warehouse_picking_core.Solver
{
    public class DummySolver : ISolver
    {
        public const string SolverName = "Dummy";
        internal DummySolver(Warehouse warehouse, IPickings pickings)
        {
            Pickings = pickings;
            Warehouse = warehouse;
        }

        private Warehouse Warehouse { get; set; }
        private IPickings Pickings { get; set; }

        public ISolution Solve()
        {
            var lastPositionOnTheAisle = Warehouse.NbBlock*(Warehouse.AisleLenght + 2) - 2;
                // one for Top, one for last position
            var solution = new DummySolution();
            var initShiftPoint = new ShiftPoint(0, 0);
            var shiftPointList = new List<ShiftPoint> {initShiftPoint};
            for (int i = 1; i < Warehouse.NbAisles + 1; i = i + 2)
            {
                var x = ((i - 1)/2)*3 + 1;
                var goToX = new ShiftPoint(x, 0);
                shiftPointList.Add(goToX);
                var goToLastPositionOnTheAisle = new ShiftPoint(x, lastPositionOnTheAisle);
                shiftPointList.Add(goToLastPositionOnTheAisle);
                shiftPointList.Add(goToX);
            }
            shiftPointList.Add(initShiftPoint);
            solution.ShiftPointList = shiftPointList;
            return solution;
        }
    }
}
