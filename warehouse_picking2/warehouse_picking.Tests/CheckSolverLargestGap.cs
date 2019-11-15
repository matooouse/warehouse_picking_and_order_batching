using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using warehouse_picking.Solver;
using NFluent;

namespace warehouse_picking.Tests
{
    [TestFixture]
    public class CheckSolverLargestGap
    {
        readonly ShiftPoint _initShiftPoint = new ShiftPoint(0, 0);
        [Test]
        public void Should_go_to_the_first_wish_and_return()
        {
            var warehouse = new Warehouse(1, 1, 1);
            var wishes = new DummyPickings
            {
                PickingList = new List<PickingPos>
                {
                    new PickingPos(1, 1, 1, 1, 1, 1)
                }
            };
            var solver = new LargestGapSolver(warehouse, wishes);
            var solution = solver.Solve();
            var shiftPointList = solution.ShiftPointList;
            var destination = new ShiftPoint(1, 1);
            var intermediatePoint = new ShiftPoint(1, 0);
            var intermediatePoint2 = new ShiftPoint(1, 2);
            var wantedSolution = new List<ShiftPoint>
            {
                _initShiftPoint,
                intermediatePoint,
                destination,
                intermediatePoint2,
                intermediatePoint,
                _initShiftPoint
            };
            Check.That(shiftPointList).IsEqualTo(wantedSolution);
        }
        
        [Test]
        public void Should_go_to_the_top_of_all_bloc_and_return()
        {
            var warehouse = new Warehouse(2, 3, 1);
            var wishes = new DummyPickings
            {
                PickingList = new List<PickingPos>
                {
                    new PickingPos(1, 1, 1, 1, 1, 1),
                    new PickingPos(1, 2, 1, 1, 1, 1)
                }
            };
            var solver = new LargestGapSolver(warehouse, wishes);
            var solution = solver.Solve();
            var shiftPointList = solution.ShiftPointList;
            var destination1 = new ShiftPoint(1, 1);
            var destination2 = new ShiftPoint(1, 4);
            var intermediatePoint = new ShiftPoint(1, 0);
            var intermediatePoint2 = new ShiftPoint(1, 5);
            var wantedSolution = new List<ShiftPoint>
            {
                _initShiftPoint,
                intermediatePoint,
                destination1,
                destination2,
                intermediatePoint2,
                intermediatePoint,
                _initShiftPoint
            };
            Check.That(shiftPointList).IsEqualTo(wantedSolution);
        }

        [Test]
        public void Should_consider_that_2_wishes_on_the_same_position_idx_but_different_aisle_idx_are_picked_in_the_same_time()
        {
            const int aiseLenght = 1;
            const int nbAisle = 2;
            var warehouse = new Warehouse(1, nbAisle, aiseLenght);
            var clientWish1 = new PickingPos(1, 1, 1, 1, aiseLenght, 1);
            var clientWish2 = new PickingPos(2, 1, 2, 1, aiseLenght, 1);
            var wishes = new DummyPickings
            {
                PickingList = new List<PickingPos> { clientWish1, clientWish2 }
            };
            var solver = new LargestGapSolver(warehouse, wishes);
            var solution = solver.Solve();
            var intermediatePoint = new ShiftPoint(1, 0);
            var intermediatePoint2 = new ShiftPoint(1, aiseLenght + 1);
            var intermediatePoint3 = new ShiftPoint(1, 0);
            var wantedSolution = new List<ShiftPoint>
            {
                _initShiftPoint,
                intermediatePoint,
                clientWish1.ConverToShiftPoint(),
                intermediatePoint2,
                intermediatePoint3,
                _initShiftPoint
            };
            Check.That(solution.ShiftPointList).IsEqualTo(wantedSolution);
        }
        
        [Test]
        public void Should_no_go_in_empty_aisles()
        {
            const int aiseLenght = 2;
            var warehouse = new Warehouse(1, 6, aiseLenght);
            var clientWish1 = new PickingPos(1, 1, 1, 1, aiseLenght, 1);
            var clientWish2 = new PickingPos(2, 1, 6, 1, aiseLenght, 1);
            var wishes = new DummyPickings
            {
                PickingList = new List<PickingPos> { clientWish1, clientWish2 }
            };
            var solver = new LargestGapSolver(warehouse, wishes);
            var solution = solver.Solve();
            var intermediatePoint = new ShiftPoint(1, 0);
            var intermediatePoint2 = new ShiftPoint(1, aiseLenght + 1);
            var intermediatePoint3 = new ShiftPoint(7, aiseLenght + 1);
            var intermediatePoint4 = new ShiftPoint(7, 0);
            var wantedSolution = new List<ShiftPoint>
            {
                _initShiftPoint,
                intermediatePoint,
                clientWish1.ConverToShiftPoint(),
                intermediatePoint2,
                intermediatePoint3,
                clientWish2.ConverToShiftPoint(),
                intermediatePoint4,
                _initShiftPoint
            };
            Check.That(solution.ShiftPointList).IsEqualTo(wantedSolution);
        }

        [Test]
        public void Should_no_do_the_largest_gap_in_a_aisle()
        {
            const int aiseLenght = 3;
            var warehouse = new Warehouse(1, 9, aiseLenght);
            var clientWish1 = new PickingPos(1, 1, 1, 1, aiseLenght, 1);
            // should not do all the 3/4 aisle, largest gap in the middle
            var clientWish2 = new PickingPos(2, 1, 3, 1, aiseLenght, 1);
            var clientWish3 = new PickingPos(3, 1, 4, 3, aiseLenght, 1);
            // should not do all the 5/6 aisle, largest gap in the start
            var clientWish4 = new PickingPos(4, 1, 5, 1, aiseLenght, 1);
            var clientWish5 = new PickingPos(5, 1, 6, 2, aiseLenght, 1);
            // should not do all the 7/8 aisle, largest gap in the end
            var clientWish6 = new PickingPos(6, 1, 7, 3, aiseLenght, 1);
            var clientWish7 = new PickingPos(7, 1, 8, 2, aiseLenght, 1);
            var clientWish8 = new PickingPos(8, 1, 9, 1, aiseLenght, 1);
            var wishes = new DummyPickings
            {
                PickingList = new List<PickingPos> { clientWish1, clientWish2, clientWish3, clientWish4, clientWish5, clientWish6, clientWish7, clientWish8 }
            };
            var solver = new LargestGapSolver(warehouse, wishes);
            var solution = solver.Solve();
            var intermediatePoint = new ShiftPoint(1, 0);
            var intermediatePoint2 = new ShiftPoint(1, aiseLenght + 1);
            var intermediatePoint3 = new ShiftPoint(4, aiseLenght + 1);
            var intermediatePoint4 = new ShiftPoint(7, aiseLenght + 1);
            var intermediatePoint5 = new ShiftPoint(10, aiseLenght + 1);
            var intermediatePoint6 = new ShiftPoint(13, aiseLenght + 1);
            var intermediatePoint7 = new ShiftPoint(13, 0);
            var intermediatePoint8 = new ShiftPoint(10, 0);
            var intermediatePoint9 = new ShiftPoint(7, 0);
            var intermediatePoint10 = new ShiftPoint(4, 0);
            var wantedSolution = new List<ShiftPoint>
            {
                _initShiftPoint,//0
                intermediatePoint,
                clientWish1.ConverToShiftPoint(),
                intermediatePoint2,
                intermediatePoint3,
                clientWish3.ConverToShiftPoint(),//5
                intermediatePoint3,
                intermediatePoint5,
                clientWish6.ConverToShiftPoint(),
                clientWish7.ConverToShiftPoint(),
                intermediatePoint5,//10
                intermediatePoint6,
                clientWish8.ConverToShiftPoint(),
                intermediatePoint7, // fin de l'allée
                intermediatePoint9,
                clientWish4.ConverToShiftPoint(),//15
                clientWish5.ConverToShiftPoint(),
                intermediatePoint9,
                intermediatePoint10,
                clientWish2.ConverToShiftPoint(),
                intermediatePoint10,
                _initShiftPoint
            };
            Check.That(solution.ShiftPointList).IsEqualTo(wantedSolution);
        }
    }
}
