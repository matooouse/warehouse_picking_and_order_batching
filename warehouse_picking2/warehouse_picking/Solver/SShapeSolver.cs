
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace warehouse_picking.Solver
{
    internal class SShapeSolver : ISolver
    {
        private Warehouse Warehouse { get; set; }
        private IPickings Pickings { get; set; }

        public SShapeSolver(Warehouse currentWarehouse, IPickings currentPickings)
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
                if (isLastDirectionUp)
                {
                    // add bottom path
                    var bottomWish = new ShiftPoint(lastShiftPoint.X, 0);
                    shiftPoints.Add(bottomWish);
                }
                else
                {
                    var wishY = (Warehouse.NbBlock - 1)*(Warehouse.AisleLenght + 2) + Warehouse.AisleLenght + 1;
                    // add top path
                    var topWish = new ShiftPoint(lastShiftPoint.X, wishY);
                    shiftPoints.Add(topWish);
                }
                AddIntermediatePositionIfNeeded(shiftPoints, shiftPointList);
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

        private static void AddIntermediatePositionIfNeeded(IEnumerable<ShiftPoint> orderWishes,
            List<ShiftPoint> shiftPointList)
        {
            var lastShiftPoint = shiftPointList.Last();
            foreach (var wish in orderWishes)
            {
                if (wish.X == lastShiftPoint.X || wish.Y == lastShiftPoint.Y)
                {
                    // only vertical or horizontal move, no add needed
                }
                else
                {
                    var intermdiateShiftPoint = new ShiftPoint(wish.X, lastShiftPoint.Y);
                    shiftPointList.Add(intermdiateShiftPoint);
                }
                var shiftPoint = new ShiftPoint(wish.X, wish.Y);
                shiftPointList.Add(shiftPoint);
                lastShiftPoint = wish;
            }
        }

        internal IList<ShiftPoint> OrderWishesByAisle(IEnumerable<PickingPos> wishes, bool isLastDirectionUp)
        {
            List<PickingPos> orderedWishes;
            if (isLastDirectionUp)
            {
                orderedWishes = wishes.OrderByDescending(w => w.BlockIdx)
                    .ThenByDescending(w => w.PositionIdx).ThenBy(w => w.AislesIdx).ToList();
            }
            else
            {
                orderedWishes = wishes.OrderBy(w => w.BlockIdx)
                    .ThenBy(w => w.PositionIdx).ThenBy(w => w.AislesIdx).ToList();
            }
            // delete wishes on the same position idx
            var result = new List<ShiftPoint>();
            ShiftPoint lastShiftPoint = null;
            foreach (var wish in orderedWishes)
            {
                var newShiftPoint = wish.ConverToShiftPoint();
                if (!newShiftPoint.Equals(lastShiftPoint))
                {
                    result.Add(newShiftPoint);
                    lastShiftPoint = newShiftPoint;
                }
            }
            return result;
        }
    }
}