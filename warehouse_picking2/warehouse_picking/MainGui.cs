using System;
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
            int nbAisles = rnd.Next(1, 10);
            int aisleLenght = rnd.Next(5, 25);
            //int nbBlock = 3;
            //int nbAisles = 2;
            //int aisleLenght = 1;
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
            _sShapeSolver = null;
            _dummySolver = null;
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

        private bool IsValidSolution(ISolution s)
        {
            for (int i = 0; i < s.ShiftPointList.Count - 1; i++)
            {
                var shiftPoint = s.ShiftPointList[i];
                var nextShiftPoint = s.ShiftPointList[i + 1];
                var isHoritontalMouvement = nextShiftPoint.Y == shiftPoint.Y;

                if (isHoritontalMouvement)
                {
                    if (((shiftPoint.X % 3 == 1 || nextShiftPoint.X % 3 == 1) && shiftPoint.Y != 0) || shiftPoint.X == nextShiftPoint.X)
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
            if (IsValidSolution(solution))
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
            if (IsValidSolution(solution))
            {
                _drawer.DrawSolution(solution);
                Refresh();
                UpdateDistanceLastSolution(solution);
            }
        }

        private void LargestGapSolver_Click(object sender, EventArgs e)
        {

        }

        private void ReturnSolver_Click(object sender, EventArgs e)
        {

        }

        private void CompositeSolver_Click(object sender, EventArgs e)
        {

        }
    }
}


