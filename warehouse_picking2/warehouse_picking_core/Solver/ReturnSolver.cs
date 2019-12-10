using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace warehouse_picking_core.Solver
{
    public class ReturnSolver : ISolver
    {
        public const string SolverName = "Return";
        private Warehouse Warehouse { get; set; }
        private IPickings Pickings { get; set; }

        internal ReturnSolver(Warehouse currentWarehouse, IPickings currentPickings)
        {
            Warehouse = currentWarehouse;
            Pickings = currentPickings;
        }

        public ISolution Solve()
        {
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
            var wishesByAisles = new List<List<PickingPos>>();
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
                wishes = RemoveWishWithSameShiftPoint(wishes);
                wishesByAisles.Add(wishes);
            }
            var solution = new DummySolution {Color = Color.Indigo};
            var initShiftPoint = new ShiftPoint(0, 0);
            var shiftPointList = new List<ShiftPoint> { initShiftPoint };
            solution.ShiftPointList = shiftPointList;
            foreach (List<PickingPos> iter in wishesByAisles)
            {
                var wishesByAisle = OrderWishes(iter);
                var lastWishInTheAisle = wishesByAisle.Last();
                shiftPointList.Add(new ShiftPoint(lastWishInTheAisle.PickingPointX, 0));
                shiftPointList.Add(lastWishInTheAisle.ConverToShiftPoint());
                shiftPointList.Add(new ShiftPoint(lastWishInTheAisle.PickingPointX, 0));
            }
            //go to the base
            shiftPointList.Add(initShiftPoint);
            return solution;
        }
        internal List<PickingPos> RemoveWishWithSameShiftPoint(IEnumerable<PickingPos> wishes)
        {
            List<PickingPos> orderedWishes = wishes.OrderBy(w => w.BlockIdx)
                    .ThenBy(w => w.PositionIdx).ThenBy(w => w.AislesIdx).ToList();
            // delete wishes on the same position idx
            var result = new List<PickingPos>();
            ShiftPoint lastShiftPoint = null;
            foreach (var wish in orderedWishes)
            {
                var newShiftPoint = wish.ConverToShiftPoint();
                if (newShiftPoint.Equals(lastShiftPoint)) continue;
                result.Add(wish);
                lastShiftPoint = newShiftPoint;
            }
            return result;
        }
        internal List<PickingPos> OrderWishes(IEnumerable<PickingPos> wishes)
        {
            var orderedWishes = wishes.OrderBy(w => w.BlockIdx)
                .ThenBy(w => w.PositionIdx).ToList();
            return orderedWishes;
        }
    }
}
