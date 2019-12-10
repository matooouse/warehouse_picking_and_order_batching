using System.Collections.Generic;
using NUnit.Framework;
using NFluent;
using warehouse_picking_core;
using warehouse_picking_core.Solver;

namespace warehouse_picking.Tests
{
    [TestFixture]
    public class CheckSolverSShapeV2
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
            var solver = new SShapeSolverV2(warehouse, wishes);
            var solution = solver.Solve();
            var shiftPointList = solution.ShiftPointList;
            var destination = new ShiftPoint(1, 1);
            var intermediatePoint = new ShiftPoint(1, 0);
            var wantedSolution = new List<ShiftPoint>
            {
                _initShiftPoint,
                intermediatePoint,
                destination,
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
            var solver = new SShapeSolverV2(warehouse, wishes);
            var solution = solver.Solve();
            var intermediatePoint = new ShiftPoint(1, 0);
            var intermediatePoint3 = new ShiftPoint(1, 0);
            var wantedSolution = new List<ShiftPoint>
            {
                _initShiftPoint,
                intermediatePoint,
                clientWish1.ConverToShiftPoint(),
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
            var solver = new SShapeSolverV2(warehouse, wishes);
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
        public void Should_no_go_in_aisle_in_middle_block_already_done_in_first_trip()
        {
            const int aiseLenght = 2;
            var warehouse = new Warehouse(2, 3, aiseLenght);
            var clientWish1 = new PickingPos(1, 1, 1, 1, aiseLenght, 1); //done in the first trip
            var clientWish2 = new PickingPos(2, 2, 1, 1, aiseLenght, 1);
            var clientWish3 = new PickingPos(3, 2, 3, 1, aiseLenght, 1);
            var wishes = new DummyPickings
            {
                PickingList = new List<PickingPos> { clientWish1, clientWish2, clientWish3 }
            };
            var solver = new SShapeSolverV2(warehouse, wishes);
            var solution = solver.Solve();
            var intermediatePoint = new ShiftPoint(1, 0);
            var intermediatePoint2 = new ShiftPoint(1, aiseLenght + 2);
            var intermediatePoint3 = new ShiftPoint(1, 2 * aiseLenght + 3);
            var intermediatePoint4 = new ShiftPoint(4, 2 * aiseLenght + 3);
            var intermediatePoint5 = new ShiftPoint(4, aiseLenght + 2);
            var intermediatePoint6 = new ShiftPoint(4, 0);
            var wantedSolution = new List<ShiftPoint>
            {
                _initShiftPoint,
                intermediatePoint,
                clientWish1.ConverToShiftPoint(),
                intermediatePoint2,
                clientWish2.ConverToShiftPoint(),
                intermediatePoint3,
                intermediatePoint4,
                clientWish3.ConverToShiftPoint(),
                intermediatePoint5,
                intermediatePoint6,
                _initShiftPoint
            };
            Check.That(solution.ShiftPointList).IsEqualTo(wantedSolution);
        }

        [Test]
        public void Should_go_in_nearest_aisle_when_changing_block()
        {
            const int aiseLenght = 2;
            var warehouse = new Warehouse(2, 5, aiseLenght);
            var clientWish1 = new PickingPos(1, 1, 1, 1, aiseLenght, 1); //done in the first trip
            var clientWish2 = new PickingPos(2, 2, 3, 1, aiseLenght, 1);
            var clientWish3 = new PickingPos(3, 1, 3, 1, aiseLenght, 1);
            var clientWish4 = new PickingPos(4, 1, 5, 1, aiseLenght, 1);
            var wishes = new DummyPickings
            {
                PickingList = new List<PickingPos> { clientWish1, clientWish2, clientWish3, clientWish4 }
            };
            var solver = new SShapeSolverV2(warehouse, wishes);
            var solution = solver.Solve();
            var intermediatePoint = new ShiftPoint(1, 0);
            var intermediatePoint2 = new ShiftPoint(1, aiseLenght + 2);
            var intermediatePoint3 = new ShiftPoint(4, aiseLenght + 2);
            var intermediatePoint4 = new ShiftPoint(4, 0);
            var intermediatePoint5 = new ShiftPoint(7, 0);
            var wantedSolution = new List<ShiftPoint>
            {
                _initShiftPoint,
                intermediatePoint,
                clientWish1.ConverToShiftPoint(),
                intermediatePoint2,
                intermediatePoint3,
                clientWish2.ConverToShiftPoint(),
                intermediatePoint3,
                clientWish3.ConverToShiftPoint(),
                intermediatePoint4,
                intermediatePoint5,
                clientWish4.ConverToShiftPoint(),
                intermediatePoint5,
                _initShiftPoint
            };
            Check.That(solution.ShiftPointList).IsEqualTo(wantedSolution);
        }

        [Test]
        public void Should_go_in_nearest_aisle_o_the_right_when_changing_block()
        {
            const int aiseLenght = 2;
            var warehouse = new Warehouse(2, 6, aiseLenght);
            var clientWish1 = new PickingPos(1, 1, 1, 1, aiseLenght, 1); //done in the first trip
            var clientWish2 = new PickingPos(2, 2, 3, 1, aiseLenght, 1);
            var clientWish3 = new PickingPos(3, 2, 5, 1, aiseLenght, 1);
            var clientWish4 = new PickingPos(4, 1, 5, 1, aiseLenght, 1);
            var clientWish5 = new PickingPos(5, 1, 4, 1, aiseLenght, 1);
            var wishes = new DummyPickings
            {
                PickingList = new List<PickingPos> { clientWish1, clientWish2, clientWish3, clientWish4, clientWish5 }
            };
            var solver = new SShapeSolverV2(warehouse, wishes);
            var solution = solver.Solve();
            var intermediatePoint = new ShiftPoint(1, 0);
            var intermediatePoint2 = new ShiftPoint(1, aiseLenght + 2);
            var intermediatePoint3 = new ShiftPoint(4, aiseLenght + 2);
            var intermediatePoint4 = new ShiftPoint(4, 2 * aiseLenght + 3);
            var intermediatePoint5 = new ShiftPoint(7, 2 * aiseLenght + 3);
            var intermediatePoint6 = new ShiftPoint(7, aiseLenght + 2);
            var intermediatePoint7 = new ShiftPoint(7, 0);
            var intermediatePoint8 = new ShiftPoint(4, 0);
            var wantedSolution = new List<ShiftPoint>
            {
                _initShiftPoint,
                intermediatePoint,
                clientWish1.ConverToShiftPoint(),
                intermediatePoint2,
                intermediatePoint3,
                clientWish2.ConverToShiftPoint(),
                intermediatePoint4,
                intermediatePoint5,
                clientWish3.ConverToShiftPoint(),
                intermediatePoint6,
                clientWish4.ConverToShiftPoint(),
                intermediatePoint7,
                intermediatePoint8,
                clientWish5.ConverToShiftPoint(),
                intermediatePoint8,
                _initShiftPoint
            };
            Check.That(solution.ShiftPointList).IsEqualTo(wantedSolution);
        }

        [Test]
        public void Bug()
        {
            const int aiseLenght = 1;
            var warehouse = new Warehouse(2, 6, aiseLenght);
            var clientWish1 = new PickingPos(1, 1, 2, 1, aiseLenght, 1); //done in the first trip
            var clientWish2 = new PickingPos(2, 2, 2, 1, aiseLenght, 1);
            var clientWish3 = new PickingPos(3, 1, 4, 1, aiseLenght, 1);
            var clientWish4 = new PickingPos(4, 2, 4, 1, aiseLenght, 1);
            var clientWish5 = new PickingPos(5, 1, 5, 1, aiseLenght, 1);
            var clientWish6 = new PickingPos(5, 1, 6, 1, aiseLenght, 1);
            var clientWish7 = new PickingPos(5, 2, 5, 1, aiseLenght, 1);
            var wishes = new DummyPickings
            {
                PickingList = new List<PickingPos> { clientWish1, clientWish2, clientWish3, clientWish4, clientWish5, clientWish6, clientWish7 }
            };
            var solver = new SShapeSolverV2(warehouse, wishes);
            var solution = solver.Solve();
            var wantedSolution = new List<ShiftPoint>
            {
                _initShiftPoint,
                new ShiftPoint(1,0),
                clientWish1.ConverToShiftPoint(),
                new ShiftPoint(1,3),
                clientWish2.ConverToShiftPoint(),
                new ShiftPoint(1,5),
                new ShiftPoint(4,5),
                clientWish4.ConverToShiftPoint(),
                new ShiftPoint(4,3),
                new ShiftPoint(7,3),
                clientWish7.ConverToShiftPoint(),
                new ShiftPoint(7,3),
                clientWish5.ConverToShiftPoint(),
                new ShiftPoint(7,0),
                new ShiftPoint(4,0),
                clientWish3.ConverToShiftPoint(),
                new ShiftPoint(4,0),
                _initShiftPoint
            };
            Check.That(solution.ShiftPointList).IsEqualTo(wantedSolution);
        }
    }
}
