
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace warehouse_picking.Solver
{
    internal class SShapeSolverV2 : ISolver
    {
        private Warehouse Warehouse { get; set; }
        private IPickings Pickings { get; set; }

        public SShapeSolverV2(Warehouse currentWarehouse, IPickings currentPickings)
        {
            Warehouse = currentWarehouse;
            Pickings = currentPickings;
        }

        public ISolution Solve()
        {
            var wishesByBlockAndAisles = new Dictionary<int, Dictionary<int,List<PickingPos>>>();
            var leftPickAisle = int.MaxValue;
            foreach (var pickingPos in Pickings.PickingList)
            {
                if (pickingPos.AislesIdx < leftPickAisle)
                {
                    leftPickAisle = pickingPos.AislesIdx;
                }
                Dictionary<int, List<PickingPos>> wishesByAisles;
                if (wishesByBlockAndAisles.ContainsKey(pickingPos.BlockIdx))
                {
                    wishesByAisles = wishesByBlockAndAisles[pickingPos.BlockIdx];
                }
                else
                {
                    wishesByAisles = new Dictionary<int, List<PickingPos>>();
                    wishesByBlockAndAisles.Add(pickingPos.BlockIdx, wishesByAisles);
                }
                List<PickingPos> wishes;
                if (wishesByAisles.ContainsKey(pickingPos.AislesIdx))
                {
                    wishes = wishesByAisles[pickingPos.AislesIdx];
                }
                else
                {
                    wishes = new List<PickingPos>();
                    wishesByAisles.Add(pickingPos.AislesIdx, wishes);
                }
                wishes.Add(pickingPos);
            }
            var leftPickingPointX = ConvertAislesIndexToPickingX(leftPickAisle);
            var orderWishesByBlockAndAisles = wishesByBlockAndAisles.OrderByDescending(x => x.Value.Values.First().First().BlockIdx).ToList();
            var solution = new DummySolution {Color = Color.MidnightBlue};
            var initShiftPoint = new ShiftPoint(0, 0);
            var bottomLeftPickingAisle = new ShiftPoint(leftPickingPointX, 0);
            var shiftPointList = new List<ShiftPoint> { initShiftPoint, bottomLeftPickingAisle };
            //for (int i = 0; i < orderWishesByBlockAndAisles.Count; i = i + 2)
            var isFirstBlock = true;
            var lastShiftPoint = bottomLeftPickingAisle;    
            foreach (var wishesByAisleCouple in orderWishesByBlockAndAisles)
            {
                var blockIndex = wishesByAisleCouple.Key;
                var wishesByAisle = wishesByAisleCouple.Value;
                if (isFirstBlock)
                {
                    isFirstBlock = false;
                    var shiftPointDoneInFirstTrip = RemoveLastAisleForAllOtherBlock(leftPickAisle, orderWishesByBlockAndAisles);
                    shiftPointList.AddRange(shiftPointDoneInFirstTrip);
                    var bottomY = (blockIndex - 1) * (Warehouse.AisleLenght + 2); //we go to the bottom 
                    var frontFirstBlock = new ShiftPoint(lastShiftPoint.X, bottomY);
                    if (!frontFirstBlock.Equals(shiftPointList.Last()))
                    {
                        //go to the front of this block on the left aisle 
                        shiftPointList.Add(frontFirstBlock);
                        lastShiftPoint = frontFirstBlock;
                    }
                    var orderedWishesByAisle = wishesByAisle.OrderBy(x => x.Key).ToList();
                    lastShiftPoint = SShapeOnAisle(blockIndex, orderedWishesByAisle, wishesByAisle, shiftPointList, lastShiftPoint, false);
                }
                else
                {
                    // we are one cell upper than the back/top of this block
                    if (wishesByAisle.Values.Count == 0)
                    {
                        // we already have done all the aisle in this block in the frist trip
                        var bottomY = (blockIndex - 1) * (Warehouse.AisleLenght + 2); //we go to the bottom
                        var finalShitPoint = new ShiftPoint(shiftPointList.Last().X, bottomY);
                        shiftPointList.Add(finalShitPoint);
                        lastShiftPoint = finalShitPoint;
                    }
                    else
                    {
                        // we should redo sshape algo for this block
                        var aislesIdx = wishesByAisle.Keys.ToList();
                        aislesIdx.Sort();
                        var leftAisleCurrentBlock = aislesIdx.First();
                        var leftX = ConvertAislesIndexToPickingX(leftAisleCurrentBlock);
                        var rightAisleCurrentBlock = aislesIdx.Last();
                        var rightX = ConvertAislesIndexToPickingX(rightAisleCurrentBlock);
                        var sShapeLeftToRight = !(Math.Abs(lastShiftPoint.X - rightX) < Math.Abs(lastShiftPoint.X - leftX));
                        var orderedWishesByAisle = sShapeLeftToRight
                            ? wishesByAisle.OrderBy(x => x.Key).ToList()
                            : wishesByAisle.OrderByDescending(x => x.Key).ToList();
                        lastShiftPoint = SShapeOnAisle(blockIndex, orderedWishesByAisle, wishesByAisle, shiftPointList, lastShiftPoint, true);
                    }
                }
            }
            shiftPointList.Add(initShiftPoint);
            solution.ShiftPointList = shiftPointList;
            return solution;
        }

        private ShiftPoint SShapeOnAisle(int blockIndex, IEnumerable<KeyValuePair<int, List<PickingPos>>> orderedWishesByAisle, Dictionary<int, List<PickingPos>> wishesByAisle,
            List<ShiftPoint> shiftPointList, ShiftPoint lastShiftPoint, bool isLastDirectionUp)
        {
            var bottomY = (blockIndex - 1)*(Warehouse.AisleLenght + 2); //we go to the bottom
            var aisles = orderedWishesByAisle.Select(x => x.Key).ToList();
            while (aisles.Count > 0)
            {
                var aisleIdx = aisles.First();
                var pickingX = ConvertAislesIndexToPickingX(aisleIdx);
                aisles.Remove(aisleIdx);
                var wishes = wishesByAisle[aisleIdx];
                // if we are on the left aisle we should check the right aisle
                if (aisles.Contains(aisleIdx + 1) && aisleIdx%2 == 1)
                {
                    wishes.AddRange(wishesByAisle[aisleIdx + 1]);
                    aisles.Remove(aisleIdx + 1);
                }
                // if we are on the right aisle we should check the left aisle
                if (aisles.Contains(aisleIdx - 1) && aisleIdx % 2 == 0)
                {
                    wishes.AddRange(wishesByAisle[aisleIdx - 1]);
                    aisles.Remove(aisleIdx - 1);
                }
                // we go to the back of the aisle
                var intermediateShitPoint = new ShiftPoint(pickingX, shiftPointList.Last().Y);
                if (!intermediateShitPoint.Equals(shiftPointList.Last()))
                {
                    //go to the front of the first aisle of this block
                    shiftPointList.Add(intermediateShitPoint);
                }
                var orderedShiftPoints = OrderWishesByAisleAndConvert(wishes, isLastDirectionUp);
                shiftPointList.AddRange(orderedShiftPoints);

                // if there is no other aisle and we are coming from front aisle we return to front else go through
                if (aisles.Count == 0 && !isLastDirectionUp)
                {
                    var finalShitPoint = new ShiftPoint(pickingX, bottomY);
                    shiftPointList.Add(finalShitPoint);
                    lastShiftPoint = finalShitPoint;
                }
                else
                {
                    // we go through
                    var wantedY = bottomY; //we go to the bottom
                    if (!isLastDirectionUp)
                    {
                        wantedY += Warehouse.AisleLenght + 1; //we go to the top
                    }
                    var finalShitPoint = new ShiftPoint(pickingX, wantedY);
                    shiftPointList.Add(finalShitPoint);
                    lastShiftPoint = finalShitPoint;
                }
                isLastDirectionUp = !isLastDirectionUp;
            }
            return lastShiftPoint;
        }

        private int ConvertAislesIndexToPickingX(int aisleIdx)
        {
            return ((aisleIdx - 1) / 2) * 3 + 1;
        }
 

        private IEnumerable<ShiftPoint> RemoveLastAisleForAllOtherBlock(int leftPickAisle, List<KeyValuePair<int, Dictionary<int, List<PickingPos>>>> wishesByAisleCouple)
        {
            var shiftPointDoneInFirstTrip = new List<ShiftPoint>();
            var blockKeys = wishesByAisleCouple.Select(x=>x.Key).ToList();
            blockKeys.Sort();
            foreach (var blockKey in blockKeys)
            {
                if (blockKey == blockKeys.Last())
                    continue;
                var wishesByAisle = wishesByAisleCouple[blockKey].Value;
                List<PickingPos> wishes = null;
                if (leftPickAisle%2 == 1)
                {
                    // we should test the 2 aisles
                    if (wishesByAisle.ContainsKey(leftPickAisle) )
                    {
                        wishes = wishesByAisle[leftPickAisle];
                        wishesByAisle.Remove(leftPickAisle);
                    }
                    if (wishesByAisle.ContainsKey(leftPickAisle + 1))
                    {
                        if (wishes == null)
                        {
                            wishes = wishesByAisle[leftPickAisle+1];
                        }
                        else
                        {
                            wishes.AddRange(wishesByAisle[leftPickAisle + 1]);
                        }
                        wishesByAisle.Remove(leftPickAisle+1);
                    }
                }
                else
                {
                    // we should test only 1 aisle
                    if (wishesByAisle.ContainsKey(leftPickAisle))
                    {
                        wishes = wishesByAisle[leftPickAisle];
                        wishesByAisle.Remove(leftPickAisle);
                    }
                }
                if (wishes == null) continue;
                var orderedWishes = wishes.OrderBy(x => x.PositionIdx);
                var result = orderedWishes.Select(x => x.ConverToShiftPoint()).Distinct().ToList();
                shiftPointDoneInFirstTrip.AddRange(result);
            }
            return shiftPointDoneInFirstTrip;
        }

        internal IList<ShiftPoint> OrderWishesByAisleAndConvert(IEnumerable<PickingPos> wishes, bool isLastDirectionUp)
        {
            List<PickingPos> orderedWishes = isLastDirectionUp
                ? wishes.OrderByDescending(w => w.PositionIdx).ThenBy(w => w.AislesIdx).ToList()
                : wishes.OrderBy(w => w.PositionIdx).ThenBy(w => w.AislesIdx).ToList();
            // delete wishes on the same position idx
            var result = orderedWishes.Select(x => x.ConverToShiftPoint()).Distinct().ToList();
            return result;
        }
    }
}