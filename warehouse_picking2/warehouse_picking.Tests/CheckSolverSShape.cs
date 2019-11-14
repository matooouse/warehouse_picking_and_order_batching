using System.Collections.Generic;
using NUnit.Framework;
using warehouse_picking.Solver;
using NFluent;

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
        public void Should_priority_to_the_wish_on_the_same_position_in_a_pair_of_aisles()
        {
            var warehouse = new Warehouse(1, 2, 2);
            var clientWish1 = new PickingPos(1, 1, 1, 1, 2, 1);
            var clientWish2 = new PickingPos(2, 1, 1, 2, 2, 1);
            var clientWish3 = new PickingPos(3, 1, 2, 1, 2, 1);
            var wishes = new DummyPickings
            {
                PickingList = new List<PickingPos>
                {clientWish1, clientWish2, clientWish3}
            };
            var solver = new SShapeSolver(warehouse, wishes);
            var orderWishesByAisle = solver.OrderWishesByAisle(wishes.PickingList, false);
            var wantedSolution = new List<ShiftPoint>
            {
                clientWish1.ConverToShiftPoint(),
                clientWish3.ConverToShiftPoint(),
                clientWish2.ConverToShiftPoint()
            };
            Check.That(orderWishesByAisle).IsEqualTo(wantedSolution);
        }

        [Test]
        public void Should_deinterlace_when_there_is_too_much_right_letf_going_and_coming()
        {
            var warehouse = new Warehouse(1, 2, 3);
            var clientWish1 = new PickingPos(1, 1, 1, 1, 3, 1);
            var clientWish2 = new PickingPos(2, 1, 1, 2, 3, 1);
            var clientWish3 = new PickingPos(3, 1, 1, 3, 3, 1);
            var clientWish4 = new PickingPos(4, 1, 2, 1, 3, 1);
            var clientWish5 = new PickingPos(5, 1, 2, 2, 3, 1);
            var clientWish6 = new PickingPos(6, 1, 2, 3, 3, 1);
            var wishes = new DummyPickings
            {
                PickingList = new List<PickingPos>
                {clientWish1, clientWish2, clientWish3, clientWish4, clientWish5, clientWish6}
            };
            var solver = new SShapeSolver(warehouse, wishes);
            var orderWishesByAisle = solver.OrderWishesByAisle(wishes.PickingList, false);
            var wantedSolution = new List<ShiftPoint>
            {
                clientWish1.ConverToShiftPoint(),
                clientWish4.ConverToShiftPoint(),
                clientWish5.ConverToShiftPoint(),
                clientWish2.ConverToShiftPoint(),
                clientWish3.ConverToShiftPoint(),
                clientWish6.ConverToShiftPoint()
            };
            Check.That(orderWishesByAisle).IsEqualTo(wantedSolution);
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
        public void Should_reoganize_when_changing_aisles()
        {
            const int aiseLenght = 2;
            var warehouse = new Warehouse(1, 4, aiseLenght);
            var clientWish1 = new PickingPos(1, 1, 1, 1, aiseLenght, 1);
            var clientWish2 = new PickingPos(2, 1, 3, 1, aiseLenght, 1);
            var clientWish3 = new PickingPos(3, 1, 3, 2, aiseLenght, 1);
            var clientWish4 = new PickingPos(4, 1, 4, 2, aiseLenght, 1);
            var wishes = new DummyPickings
            {
                PickingList = new List<PickingPos> { clientWish1, clientWish2, clientWish3, clientWish4 }
            };
            var solver = new SShapeSolver(warehouse, wishes);
            var solution = solver.Solve();
            var intermediatePoint = new ShiftPoint(1, 0);
            var intermediatePoint2 = new ShiftPoint(1, 3);
            var intermediatePoint3 = new ShiftPoint(4, 3);
            var intermediatePoint4 = new ShiftPoint(4, 2);
            var intermediatePoint5 = new ShiftPoint(4, 0);
            var wantedSolution = new List<ShiftPoint>
            {
                _initShiftPoint,
                intermediatePoint,
                clientWish1.ConverToShiftPoint(),
                intermediatePoint2,
                intermediatePoint3,
                clientWish3.ConverToShiftPoint(),
                clientWish4.ConverToShiftPoint(),
                intermediatePoint4,
                clientWish2.ConverToShiftPoint(),
                intermediatePoint5,
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
            var intermediatePoint2 = new ShiftPoint(1, 3);
            var intermediatePoint3 = new ShiftPoint(8, 3);
            var intermediatePoint4 = new ShiftPoint(8, 0);
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
