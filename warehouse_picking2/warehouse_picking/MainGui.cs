using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using warehouse_picking.Solver;

namespace warehouse_picking
{
    public partial class MainGui : Form
    {
        public MainGui()
        {
            InitializeComponent();
        }

        private Drawer _drawer;
        private Warehouse _currentWarehouse;
        private IPickings _currentPickings;

        private void generate_Click(object sender, EventArgs e)
        {
            var rnd = new Random();
            //const int nbBlock = 1;
            int nbBlock = rnd.Next(1, 5);
            int nbAisles = rnd.Next(1, 20);
            int aisleLenght = rnd.Next(5, 25);
            //int nbBlock = 1;
            //int nbAisles = 3;
            //int aisleLenght = 2;
            var warehouse = new Warehouse(nbBlock, nbAisles, aisleLenght);
            if (_drawer == null)
            {
                _drawer = new Drawer();
            }
            else
            {
                _drawer.Clear();
            }
            _drawer.DrawWarehouse(warehouse);
            Paint += _drawer.Drawing_handler;
            int wishSize = rnd.Next(1, nbBlock*nbAisles*aisleLenght)/10;
            IPickings pickings = new Pickings(warehouse, wishSize);
            _drawer.DrawPickingObjectif(pickings);
            Refresh();
            _currentWarehouse = warehouse;
            _currentPickings = pickings;
            _dummySolver = null;
            _sShapeSolver = null;
            _largestGapSolver = null;
        }

        private void UpdateDistanceLastSolution(ISolution s)
        {
            int totalDistance = 0;
            for (int i = 0; i < s.ShiftPointList.Count - 1; i++)
            {
                var shiftPoint = s.ShiftPointList[i];
                var nextShiftPoint = s.ShiftPointList[i + 1];
                var isHoritontalMouvement = nextShiftPoint.Y == shiftPoint.Y;
                if (isHoritontalMouvement)
                {
                    totalDistance += Math.Abs(nextShiftPoint.X - shiftPoint.X);
                }
                else
                {
                    totalDistance += Math.Abs(nextShiftPoint.Y - shiftPoint.Y);
                }
            }
            distanceLastSolution.Text = totalDistance.ToString(CultureInfo.InvariantCulture);
        }

        private bool IsValidSolution(ISolution s, Warehouse currentWarehouse)
        {
            for (int i = 0; i < s.ShiftPointList.Count - 1; i++)
            {
                var shiftPoint = s.ShiftPointList[i];
                var nextShiftPoint = s.ShiftPointList[i + 1];
                var isHoritontalMouvement = nextShiftPoint.Y == shiftPoint.Y;

                if (isHoritontalMouvement)
                {
                    var moveOnY = shiftPoint.Y%(currentWarehouse.AisleLenght + 2);
                    if (moveOnY != 0 && moveOnY != currentWarehouse.AisleLenght + 1)
                    {
                        var error = "Forbidden move " + shiftPoint + " to " + nextShiftPoint;
                        Console.WriteLine(error);
                        MessageBox.Show(error);
                        return false;
                    }
                }
                else
                {
                    if (shiftPoint.X%3 == 1 && nextShiftPoint.X%3 == 1 && shiftPoint.X == nextShiftPoint.X) continue;
                    var error = "Forbidden move " + shiftPoint + " to " + nextShiftPoint;
                    Console.WriteLine(error);
                    MessageBox.Show(error);
                    return false;
                }
            }
            return true;
        }

        private ISolver _dummySolver;
        private ISolver _sShapeSolver;
        private ISolver _largestGapSolver;

        private void DummySolver_Click(object sender, EventArgs e)
        {
            if (_currentWarehouse == null || _currentPickings == null)
            {
                MessageBox.Show(@"Please start to generate a warehouse");
                return;
            }
            if (_dummySolver == null)
            {
                _dummySolver = new DummySolver(_currentWarehouse, _currentPickings);
            }
            var solution = _dummySolver.Solve();
            if (IsValidSolution(solution, _currentWarehouse))
            {
                _drawer.DrawSolution(solution);
                Refresh();
                UpdateDistanceLastSolution(solution);
            }
        }

        private void SShapeSolver_Click(object sender, EventArgs e)
        {
            if (_currentWarehouse == null || _currentPickings == null)
            {
                MessageBox.Show(@"Please start to generate a warehouse");
                return;
            }
            if (_sShapeSolver == null)
            {
                _sShapeSolver = new SShapeSolver(_currentWarehouse, _currentPickings);

            }
            var solution = _sShapeSolver.Solve();
            if (IsValidSolution(solution, _currentWarehouse))
            {
                ISolution simplifiedSolution = SimplifySolution(solution);
                _drawer.DrawSolution(simplifiedSolution);
                Refresh();
                UpdateDistanceLastSolution(solution);
            }
        }

        private ISolution SimplifySolution(ISolution s)
        {
            var simplifiedSolution = new DummySolution {ShiftPointList = new List<ShiftPoint>(), Color = s.Color};
            var origin = s.ShiftPointList[0];
            simplifiedSolution.ShiftPointList.Add(origin);
            var destination = s.ShiftPointList[1];
            var i = 1;
            var isHoritontalMouvement = origin.Y == destination.Y;
            var wayUp = isHoritontalMouvement ? origin.X < destination.X : origin.Y < destination.Y;
            while (i < s.ShiftPointList.Count - 1)
            {
                var shiftPoint = s.ShiftPointList[i];
                var nextShiftPoint = s.ShiftPointList[i + 1];
                var isHoritontalMouvement2 = nextShiftPoint.Y == shiftPoint.Y;
                var wayUp2 = isHoritontalMouvement2 ? shiftPoint.X < nextShiftPoint.X : shiftPoint.Y < nextShiftPoint.Y;
                if (isHoritontalMouvement.Equals(isHoritontalMouvement2) && wayUp.Equals(wayUp2))
                {
                    destination = s.ShiftPointList[i + 1];
                }
                else
                {
                    simplifiedSolution.ShiftPointList.Add(destination);
                    isHoritontalMouvement = isHoritontalMouvement2;
                    wayUp = wayUp2;
                    destination = nextShiftPoint;
                }
                i++;
            }
            simplifiedSolution.ShiftPointList.Add(destination);
            return simplifiedSolution;
        }

        private void LargestGapSolver_Click(object sender, EventArgs e)
        {
            if (_currentWarehouse == null || _currentPickings == null)
            {
                MessageBox.Show(@"Please start to generate a warehouse");
                return;
            }
            if (_largestGapSolver == null)
            {
                _largestGapSolver = new LargestGapSolver(_currentWarehouse, _currentPickings);

            }
            var solution = _largestGapSolver.Solve();
            if (IsValidSolution(solution, _currentWarehouse))
            {
                ISolution simplifiedSolution = SimplifySolution(solution);
                _drawer.DrawSolution(simplifiedSolution);
                Refresh();
                UpdateDistanceLastSolution(solution);
            }
        }

        private void ReturnSolver_Click(object sender, EventArgs e)
        {

        }

        private void CompositeSolver_Click(object sender, EventArgs e)
        {

        }
    }
}


