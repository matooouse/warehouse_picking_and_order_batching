using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace warehouse_picking_core.Solver
{
    public class SShapeSolver : ISolver
    {
        public const string SolverName = "Sshape";
        private Warehouse Warehouse { get; set; }
        private IPickings Pickings { get; set; }

        internal SShapeSolver(Warehouse currentWarehouse, IPickings currentPickings)
        {
            Warehouse = currentWarehouse;
            Pickings = currentPickings;
        }

        public ISolution Solve()
        {
            var isLastDirectionUp = false;
            var wishesByAislesIdx = new List<PickingPos>[Warehouse.NbAisles];
            for (int i = 0; i < wishesByAislesIdx.Length; i++)
            {
                wishesByAislesIdx[i] = new List<PickingPos>();
            }
            foreach (var clientWish in Pickings.PickingList)
            {
                var arrayIdx = clientWish.AislesIdx - 1;
                wishesByAislesIdx[arrayIdx].Add(clientWish);
            }
            var solution = new DummySolution {Color = Color.Blue};
            var initShiftPoint = new ShiftPoint(0, 0);
            var shiftPointList = new List<ShiftPoint> {initShiftPoint};
            // top Y = last line of the warehouse
            var topY = (Warehouse.NbBlock - 1)*(Warehouse.AisleLenght + 2) + Warehouse.AisleLenght + 1;
            for (int i = 0; i < wishesByAislesIdx.Length; i = i + 2)
            {
                var wishes = new List<PickingPos>(wishesByAislesIdx[i]);
                if (i + 1 < wishesByAislesIdx.Length)
                {
                    var wishesBonus = wishesByAislesIdx[i + 1];
                    wishes.AddRange(wishesBonus);
                }
                if (wishes.Count == 0)
                {
                    continue;
                }
                var shiftPoints = OrderWishesByAisle(wishes, isLastDirectionUp);
                var lastShiftPoint = shiftPoints.Last();
                var topWish = new ShiftPoint(lastShiftPoint.X, topY);
                var bottomWish = new ShiftPoint(lastShiftPoint.X, 0);
                if (isLastDirectionUp)
                {
                    // add bottom path
                    shiftPoints.Add(bottomWish);
                    shiftPoints.Insert(0, topWish);
                }
                else
                {
                    // add top path
                    shiftPoints.Add(topWish);
                    shiftPoints.Insert(0, bottomWish);
                }
                shiftPointList.AddRange(shiftPoints);
                isLastDirectionUp = !isLastDirectionUp;
            }
            // this is the last aisles, we will go to the base
            var lastVisitedAisles = shiftPointList.Last();
            if (isLastDirectionUp)
            {
                var bottomOfLastAisles = new ShiftPoint(lastVisitedAisles.X, 0);
                shiftPointList.Add(bottomOfLastAisles);
            }
            shiftPointList.Add(initShiftPoint);
            solution.ShiftPointList = shiftPointList;
            return solution;
        }

        internal IList<ShiftPoint> OrderWishesByAisle(IEnumerable<PickingPos> wishes, bool isLastDirectionUp)
        {
            List<PickingPos> orderedWishes = isLastDirectionUp
                ? wishes.OrderByDescending(w => w.BlockIdx)
                    .ThenByDescending(w => w.PositionIdx)
                    .ThenBy(w => w.AislesIdx)
                    .ToList()
                : wishes.OrderBy(w => w.BlockIdx).ThenBy(w => w.PositionIdx).ThenBy(w => w.AislesIdx).ToList();
            // delete wishes on the same position idx
            var result = orderedWishes.Select(x => x.ConverToShiftPoint()).Distinct().ToList();
            return result;
        }
    }
}