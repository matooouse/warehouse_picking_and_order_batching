using System.Collections.Generic;
using NUnit.Framework;
using NFluent;
using warehouse_picking_core;
using warehouse_picking_core.Solver;

namespace warehouse_picking.Tests
{
    [TestFixture]
    public class CheckSolverSShape
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
            var solver = new SShapeSolver(warehouse, wishes);
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
            var warehouse = new Warehouse(2, 1, 1);
            var wishes = new DummyPickings
            {
                PickingList = new List<PickingPos>
                {
                    new PickingPos(1, 1, 1, 1, 1, 1),
                    new PickingPos(1, 2, 1, 1, 1, 1)
                }
            };
            var solver = new SShapeSolver(warehouse, wishes);
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
            var solver = new SShapeSolver(warehouse, wishes);
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
            var solver = new SShapeSolver(warehouse, wishes);
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
    }
}
