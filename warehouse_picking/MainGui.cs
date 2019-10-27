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
        private IClientWish _currentClientWish;

        private void generate_Click(object sender, EventArgs e)
        {
            var rnd = new Random();
            //const int nbBlock = 1;
            int nbBlock = rnd.Next(1, 5);
            int nbAisles = rnd.Next(1, 10);
            int aisleLenght = rnd.Next(5, 25);
            //int nbBlock = 2;
            //int nbAisles = 1;
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
            IClientWish clientWish = new ClientWish(warehouse, wishSize);
            _drawer.DrawClientWish(clientWish);
            Refresh();
            _currentWarehouse = warehouse;
            _currentClientWish = clientWish;
            _sShapeSolver = null;
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

        private ISolver _dummySolver;
        private ISolver _sShapeSolver;

        private void DummySolver_Click(object sender, EventArgs e)
        {
            if (_currentWarehouse == null || _currentClientWish == null)
            {
                MessageBox.Show(@"Please start to generate a warehouse");
                return;
            }
            if (_dummySolver == null)
            {
                _dummySolver = new DummySolver(_currentWarehouse, _currentClientWish);
            }
            var solution = _dummySolver.Solve();
            _drawer.DrawSolution(solution);
            Refresh();
            UpdateDistanceLastSolution(solution);
        }

        private void SShapeSolver_Click(object sender, EventArgs e)
        {
            if (_currentWarehouse == null || _currentClientWish == null)
            {
                MessageBox.Show(@"Please start to generate a warehouse");
                return;
            }
            if (_sShapeSolver == null)
            {
                _sShapeSolver = new SShapeSolver(_currentWarehouse, _currentClientWish);

            }
            var solution = _sShapeSolver.Solve();
            _drawer.DrawSolution(solution);
            Refresh();
            UpdateDistanceLastSolution(solution);
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


