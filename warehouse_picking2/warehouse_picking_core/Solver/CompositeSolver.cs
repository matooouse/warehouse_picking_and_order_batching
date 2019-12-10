using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace warehouse_picking_core.Solver
{
    public class CompositeSolver : ISolver
    {
        /* Lfrontj , a partial route that ends at the front of subaisle j, and
         * Lbackj , a partial route that ends at the back of subaisle j*/
        public const string SolverName = "Composite";
        private Warehouse Warehouse { get; set; }
        private IPickings Pickings { get; set; }

        internal CompositeSolver(Warehouse currentWarehouse, IPickings currentPickings)
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
            var solution = new DummySolution { Color = Color.Chocolate };
            var initShiftPoint = new ShiftPoint(0, 0);
            var shiftPointList = new List<ShiftPoint> { initShiftPoint };
            var topY = (Warehouse.NbBlock - 1) * (Warehouse.AisleLenght + 2) + Warehouse.AisleLenght + 1;
            var currentLFrontj = new List<ShiftPoint>();
            var currentLBackj = new List<ShiftPoint>();
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
                if (currentLFrontj.Count == 0)
                {
                    wishes = wishes.OrderBy(w => w.BlockIdx).ThenBy(w => w.PositionIdx).ToList();
                    var wishNearBackAisle = wishes.Last();
                    var topInit = new ShiftPoint(wishNearBackAisle.PickingPointX, topY);
                    var bottomInit = new ShiftPoint(wishNearBackAisle.PickingPointX, 0);
                    var pickingPos = wishes.Select(x => x.ConverToShiftPoint()).Distinct().ToList();
                    currentLFrontj.Add(bottomInit);
                    currentLFrontj.AddRange(pickingPos);
                    currentLFrontj.Add(bottomInit);
                    currentLBackj.Add(bottomInit);
                    currentLBackj.AddRange(pickingPos);
                    currentLBackj.Add(topInit);
                }
                else
                {
                    var res = Solve_for_next_aisle(wishes, currentLFrontj, currentLBackj);
                    currentLFrontj = res.Item1;
                    currentLBackj = res.Item2;
                }
            }
            shiftPointList.AddRange(currentLFrontj);
            shiftPointList.Add(initShiftPoint);
            solution.ShiftPointList = shiftPointList;
            return solution;
        }

        private Tuple<List<ShiftPoint>,List<ShiftPoint>> Solve_for_next_aisle(IEnumerable<PickingPos> wish, List<ShiftPoint> currentLFrontj, List<ShiftPoint> currentLBackj)
        {
            var orderWish = wish.OrderBy(w => w.BlockIdx).ThenBy(w => w.PositionIdx);
            var pickingPos = orderWish.Select(x => x.ConverToShiftPoint()).Distinct().ToList();
            var pickingPosRev = orderWish.Select(x => x.ConverToShiftPoint()).Reverse().Distinct().ToList();
            List<ShiftPoint> nextLTopj = GetNextLTopj(currentLFrontj, currentLBackj, pickingPos, pickingPosRev);
            currentLFrontj = GetNextLBottomj(currentLFrontj, currentLBackj, pickingPos, pickingPosRev);
             currentLBackj = nextLTopj;
            return new Tuple<List<ShiftPoint>, List<ShiftPoint>>(currentLFrontj, currentLBackj);
        }

        private List<ShiftPoint> GetNextLTopj(List<ShiftPoint> currentLFrontj, List<ShiftPoint> currentLBackj, List<ShiftPoint> pickingPos, IEnumerable<ShiftPoint> pickingPosRev)
        {
            var nextLTopj = new List<ShiftPoint>();
            var previousTopj = currentLBackj.Last();
            var previousBottomj = currentLFrontj.Last();
            var wishNearFrontAisle = pickingPos.First();
            var topj = new ShiftPoint(wishNearFrontAisle.X, previousTopj.Y);
            var bottomj = new ShiftPoint(wishNearFrontAisle.X, previousBottomj.Y);
            var transitionWithReturnForTop = new List<ShiftPoint> { previousTopj, topj }; // go to the top of the next aisle
            transitionWithReturnForTop.AddRange(pickingPosRev); // go to the farthest wish (on ajoute ceux au milieu pour rien)
            transitionWithReturnForTop.Add(topj); // return to top
            var transitionWithSShapeForTop = new List<ShiftPoint> {previousBottomj, bottomj}; // go to the bottom of the next aisle
            transitionWithSShapeForTop.AddRange(pickingPos); // ca n'a pas d'interet d'ajouter ce point juste si on voudrait vérifier qu'on passe par tout les points
            transitionWithSShapeForTop.Add(topj); // go throught to aisle

            if (Helper.Length_of_route(currentLBackj) + Helper.Length_of_route(transitionWithReturnForTop) >
                Helper.Length_of_route(currentLFrontj) + Helper.Length_of_route(transitionWithSShapeForTop))
            {
                transitionWithSShapeForTop.RemoveAt(0);
                nextLTopj.AddRange(currentLFrontj);
                nextLTopj.AddRange(transitionWithSShapeForTop);
            }
            else
            {
                transitionWithReturnForTop.RemoveAt(0);
                nextLTopj.AddRange(currentLBackj);
                nextLTopj.AddRange(transitionWithReturnForTop);
            }
            return nextLTopj;
        }

        private List<ShiftPoint> GetNextLBottomj(List<ShiftPoint> currentLFrontj, List<ShiftPoint> currentLBackj, List<ShiftPoint> pickingPos, IEnumerable<ShiftPoint> pickingPosRev)
        {
            var nextLBottomj = new List<ShiftPoint>();
            var previousTopj = currentLBackj.Last();
            var previousBottomj = currentLFrontj.Last();
            var wishNearFrontAisle = pickingPos.First();
            var topj = new ShiftPoint(wishNearFrontAisle.X, previousTopj.Y);
            var bottomj = new ShiftPoint(wishNearFrontAisle.X, previousBottomj.Y);
            var transitionWithReturnForBottom = new List<ShiftPoint> { previousBottomj, bottomj }; // go to the bottom of the next aisle
            transitionWithReturnForBottom.AddRange(pickingPos); // go to the farthest wish (on ajoute ceux au milieu pour rien)
            transitionWithReturnForBottom.Add(bottomj); // return to bottom
            var transitionWithSShapeForBottom = new List<ShiftPoint> { previousTopj, topj }; // go to the top of the next aisle
            transitionWithSShapeForBottom.AddRange(pickingPosRev); // ca n'a pas d'interet d'ajouter ce point juste si on voudrait vérifier qu'on passe par tout les points
            transitionWithSShapeForBottom.Add(bottomj); // go throught to aisle
            if (Helper.Length_of_route(currentLFrontj) + Helper.Length_of_route(transitionWithReturnForBottom) >
                Helper.Length_of_route(currentLBackj) + Helper.Length_of_route(transitionWithSShapeForBottom))
            {
                transitionWithSShapeForBottom.RemoveAt(0);
                nextLBottomj.AddRange(currentLBackj);
                nextLBottomj.AddRange(transitionWithSShapeForBottom);
            }
            else
            {
                transitionWithReturnForBottom.RemoveAt(0);
                nextLBottomj.AddRange(currentLFrontj);
                nextLBottomj.AddRange(transitionWithReturnForBottom);
            }
            return nextLBottomj;
        }
    }
}
