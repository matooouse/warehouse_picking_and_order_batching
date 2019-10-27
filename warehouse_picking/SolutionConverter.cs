//using System;
//using System.Collections.Generic;
//using System.Windows.Forms;

//namespace warehouse_picking
//{
//    internal static class Helper
//    {
//// ReSharper disable once InconsistentNaming
//        static List<ShiftPoint> ConvertSolution (IClientWish clientWish, int[,] Yij, BottomOrTopOrDirect[,] bottomOrTopOrDirectMatrix)
//        {
//            if (Yij.GetLength(0) != Yij.GetLength(1))
//                return null;
//            if (bottomOrTopOrDirectMatrix.GetLength(0) != bottomOrTopOrDirectMatrix.GetLength(1))
//                return null;
//            if (Yij.GetLength(0) != bottomOrTopOrDirectMatrix.GetLength(1))
//                return null;
//            int currentLocation = 0;
//            var initShiftPoint = new ShiftPoint(0, 0);
//            var shiftPointList = new List<ShiftPoint> { initShiftPoint };
//            var currentShiftPoint = initShiftPoint;
//            for (int i = 0; i < Yij.GetLength(0); i++)
//            {
//                int nextLocation = GetNextLocation(Yij, 1);
//                var nextWish = clientWish.ClientWishes.Find(w => w.WishIdx == nextLocation);
//                if (currentShiftPoint.X != nextWish.WishX)
//                {
//                    var bottomOrTop = bottomOrTopOrDirectMatrix[currentLocation, nextLocation];
//                    int tempY = bottomOrTop == BottomOrTopOrDirect.B ? nextWish.BottomY : nextWish.TopY;
//                    var goTempY = new ShiftPoint(currentShiftPoint.X, tempY);
//                    shiftPointList.Add(goTempY);
//                    var goToX = new ShiftPoint(nextWish.WishX, tempY);
//                    shiftPointList.Add(goToX);
//                }
//                // pas besoin de prévoir d'autres mouvement si on reste dans la même rangée
//                currentShiftPoint = new ShiftPoint(nextWish.WishX, nextWish.WishY);
//                shiftPointList.Add(currentShiftPoint);
//                currentLocation = nextLocation;
//            }
//            return shiftPointList;
//        }

//// ReSharper disable once InconsistentNaming
//        private static int GetNextLocation(int[,] Yij, int currentLocation)
//        {
//            for (int i = 0; i < Yij.GetLength(0); i++)
//            {
//                if (Yij[currentLocation, i] == 0)
//                    return i;
//            }
//            MessageBox.Show("Fail to find next location");
//            return 0;
//        }

//        static Tuple<int[,], BottomOrTopOrDirect[,]> prepareSolving(Warehouse w)
//        {
//            var distance
//        }


//    }

//    public enum BottomOrTopOrDirect { B, T }
//}
