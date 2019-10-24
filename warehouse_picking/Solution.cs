using System;
using System.Collections.Generic;

namespace warehouse_picking
{
    internal class Solution
    {
        public List<ShiftPoint> ShiftPointList { get; private set; }

// ReSharper disable once InconsistentNaming
        public Solution(ClientWish clientWish, int [][] Yij, BottomOrTop[][]bottomOrTopMatrix)
        {
            if (Yij.GetLength(0) != Yij.GetLength(1))
                return;
            if (bottomOrTopMatrix.GetLength(0) != bottomOrTopMatrix.GetLength(1))
                return;
            if (Yij.GetLength(0) != bottomOrTopMatrix.GetLength(1))
                return;
            int currentLocation = 0;
            var initShitPoint = new ShiftPoint(0, 0);
            ShiftPointList = new List<ShiftPoint> { initShitPoint };
            var currentShitPoint = initShitPoint;
            for (int i = 0; i < Yij.GetLength(0); i++)
            {
                int nextLocation = Array.IndexOf(Yij[currentLocation], 1);
                var nextWish = clientWish.WishList.Find(w => w.PositionIdx == nextLocation);
                if (currentShitPoint.X != nextWish.WishX)
                {
                    var bottomOrTop = bottomOrTopMatrix[currentLocation][nextLocation];
                    int tempY = bottomOrTop == BottomOrTop.B ? nextWish.BottomY : nextWish.TopY;
                    var goTempY = new ShiftPoint(currentShitPoint.X, tempY);
                    ShiftPointList.Add(goTempY);
                    var goToX = new ShiftPoint(nextWish.WishX, tempY);
                    ShiftPointList.Add(goToX);
                }
                // pas besoin de prévoir d'autres mouvement si on reste dans la même rangée
                currentShitPoint = new ShiftPoint(nextWish.WishX, nextWish.WishY);
                ShiftPointList.Add(currentShitPoint);
                currentLocation = nextLocation;
            }
        }
    }

    public enum BottomOrTop { B, T }
}
