using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace warehouse_picking_core.Solver
{
    public class LargestGapSolver : ISolver
    {
        public const string SolverName = "LargestGap";
        private Warehouse Warehouse { get; set; }
        private IPickings Pickings { get; set; }

        internal LargestGapSolver(Warehouse currentWarehouse, IPickings currentPickings)
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
            var solution = new DummySolution {Color = Color.Green};
            var initShiftPoint = new ShiftPoint(0, 0);
            var shiftPointList = new List<ShiftPoint> { initShiftPoint };
            solution.ShiftPointList = shiftPointList;
            // top Y = last line of the warehouse
            var topY = (Warehouse.NbBlock - 1) * (Warehouse.AisleLenght + 2) + Warehouse.AisleLenght + 1;
            // special case for first and last aisle
            var firstAisle = wishesByAisles.FirstOrDefault();
            if (firstAisle == null)
            {
                // no wish
                shiftPointList.Add(initShiftPoint);
                return solution;
            }
            wishesByAisles.Remove(firstAisle);
            var lastAisle = wishesByAisles.LastOrDefault();
            wishesByAisles.Remove(lastAisle);
            // go through the first aisle
            firstAisle = OrderWishes(firstAisle, true);
            var firstXPos = firstAisle.First().PickingPointX;
            shiftPointList.Add(new ShiftPoint(firstXPos, 0));
            shiftPointList.AddRange(firstAisle.Select(wish => wish.ConverToShiftPoint()));
            shiftPointList.Add(new ShiftPoint(firstXPos, topY));
            // do the largest algo coming
            foreach (List<PickingPos> iter in wishesByAisles)
            {
                var wishesByAisle = OrderWishes(iter, false);
                var largestGap = 0;
                ShiftPoint currentPos = null;
                int currentGap;
                int nbWishes = 0;
                int potentialNbWishes = 0;
                var previousY = topY;
                foreach (var currentWish in wishesByAisle)
                {
                    currentPos = currentWish.ConverToShiftPoint();
                    currentGap = previousY - currentPos.Y;
                    if (currentGap > largestGap)
                    {
                        largestGap = currentGap;
                        nbWishes += potentialNbWishes;
                        potentialNbWishes = 0;
                    }
                    potentialNbWishes++;
                    previousY = currentPos.Y;
                }
                currentGap = previousY - 0;
                if (currentGap > largestGap)
                {
                    nbWishes += potentialNbWishes;
                }
                var wishDoneInComming = new List<PickingPos>();
                if (nbWishes > 0)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    var topAisle = new ShiftPoint(currentPos.X, topY);
                    // we enter in this aisle
                    shiftPointList.Add(topAisle);
                    wishDoneInComming.AddRange(wishesByAisle.GetRange(0, nbWishes));
                    wishesByAisle.RemoveRange(0, nbWishes);
                    shiftPointList.AddRange(wishDoneInComming.Select(wish => wish.ConverToShiftPoint()));
                    // return to top
                    shiftPointList.Add(topAisle);
                }
                iter.Clear();
                iter.AddRange(wishesByAisle);
            }
            // go through the last aisle
            if (lastAisle == null)
            {
                // only one aisle
                shiftPointList.Add(new ShiftPoint(firstXPos, 0));
            }
            else
            {
                var lastXPos = lastAisle.First().PickingPointX;
                shiftPointList.Add(new ShiftPoint(lastXPos, topY));
                lastAisle = OrderWishes(lastAisle, false);
                shiftPointList.AddRange(lastAisle.Select(wish => wish.ConverToShiftPoint()));
                shiftPointList.Add(new ShiftPoint(lastXPos, 0));
            }
            // do the largest algo return
            wishesByAisles.Reverse();
            foreach (var wishesByAisle in wishesByAisles)
            {
                if (wishesByAisle.Count == 0) continue;
                // enter in the aisle
                var posX = wishesByAisle.First().PickingPointX;
                shiftPointList.Add(new ShiftPoint(posX, 0));
                wishesByAisle.Reverse();
                shiftPointList.AddRange(wishesByAisle.Select(wish => wish.ConverToShiftPoint()));
                // return to the bottom of the aisle
                shiftPointList.Add(new ShiftPoint(posX, 0));
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
        internal List<PickingPos> OrderWishes(IEnumerable<PickingPos> wishes, bool isFirstAisle)
        {
            List<PickingPos> orderedWishes;
            if (isFirstAisle)
            {
                orderedWishes = wishes.OrderBy(w => w.BlockIdx)
                    .ThenBy(w => w.PositionIdx).ToList();
            }
            else
            {
                orderedWishes = wishes.OrderByDescending(w => w.BlockIdx)
                    .ThenByDescending(w => w.PositionIdx).ToList();
            }
            return orderedWishes;
        }
    }
}
