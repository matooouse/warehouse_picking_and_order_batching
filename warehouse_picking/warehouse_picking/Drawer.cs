using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace warehouse_picking
{
    internal class Drawer
    {
        private const int Lenght = 600;
        private const int Width = 300;
        private const int DefaultUpperLeftAngleX = 20;
        private const int DefaultUpperLeftAngleY = 20;
        private int _verticalLineLenght;
        private int _horizontalLineLenght;
        private int _verticalTotalLenght;
        private readonly List<Action<PaintEventArgs>> _drawList = new List<Action<PaintEventArgs>>();

        public void Drawing_handler(object sender, PaintEventArgs e)
        {
            if (_drawList.Count <= 0) return;
            _drawList.ForEach(a => a(e));
        }

        private readonly List<Warehouse> _alreadyDrawed = new List<Warehouse>();

        public void DrawWarehouse(Warehouse warehouse)
        {
            if (_alreadyDrawed.Contains(warehouse))
                return;
            int verticalCoef = (warehouse.AisleLenght + 2)*warehouse.NbBlock;
            _verticalLineLenght = Width/verticalCoef;
            int realWidth = _verticalLineLenght*verticalCoef;
            int horizontalCoef = warehouse.NbAisles%2 == 0 ? warehouse.NbAisles/2*3 : (warehouse.NbAisles + 1)/2*3 - 1;
            _horizontalLineLenght = Lenght/horizontalCoef;
            int realLenght = _horizontalLineLenght*horizontalCoef;
            _verticalTotalLenght = realWidth;
            _drawList.Add(e => e.Graphics.DrawRectangle(new Pen(Color.Black),
                new Rectangle(DefaultUpperLeftAngleX, DefaultUpperLeftAngleY, realLenght, realWidth)));
            int upperLeftAngleX = DefaultUpperLeftAngleX;
            int upperLeftAngleY = DefaultUpperLeftAngleY;
            for (var i = 1; i < warehouse.NbBlock + 1; i++)
            {
                int initUpperLeftAngleY = upperLeftAngleY;
                for (var j = 1; j < warehouse.NbAisles + 1; j++)
                {
                    upperLeftAngleY = initUpperLeftAngleY + _verticalLineLenght;
                    for (var k = 1; k < warehouse.AisleLenght + 1; k++)
                    {
                        int x = upperLeftAngleX;
                        int y = upperLeftAngleY;
                        Action<PaintEventArgs> draw = e => e.Graphics.DrawRectangle(new Pen(Color.Black),
                            new Rectangle(x, y, _horizontalLineLenght, _verticalLineLenght));
                        _drawList.Add(draw);
                        upperLeftAngleY += _verticalLineLenght;
                    }
                    upperLeftAngleX += j%2 == 0 ? _horizontalLineLenght : 2*_horizontalLineLenght;
                }
                upperLeftAngleX = DefaultUpperLeftAngleX;
                upperLeftAngleY = initUpperLeftAngleY + _verticalLineLenght*(warehouse.AisleLenght + 2);
            }
            _alreadyDrawed.Add(warehouse);
        }

        public void Clear()
        {
            _drawList.Clear();
        }

        private readonly List<IClientWish> _alreadyDrawedWishes = new List<IClientWish>();

        public void DrawClientWish(IClientWish clientWish)
        {
            if (_alreadyDrawedWishes.Contains(clientWish))
                return;
            foreach (var clientWishPos in clientWish.ClientWishes)
            {
                var blueBrush = new SolidBrush(Color.Blue);
                int x = DefaultUpperLeftAngleX + clientWishPos.UpperLeftX*_horizontalLineLenght;
                int y = DefaultUpperLeftAngleY + clientWishPos.UpperLeftY*_verticalLineLenght;
                var rect = new Rectangle(x, y, _horizontalLineLenght, _verticalLineLenght);
                Action<PaintEventArgs> draw = e => e.Graphics.FillRectangle(blueBrush, rect);
                _drawList.Add(draw);
            }
        }

        private readonly List<ISolution> _alreadyDrawedSolution = new List<ISolution>();

        private int horizontalOffset(int x)
        {
            if (x == 0)
            {
                return 0;
            }
            if (x%3 == 1)
            {
                return 1;
            }
            return -1;
        }

        public void DrawSolution(ISolution s)
        {
            if (_alreadyDrawedSolution.Contains(s))
                return;
            var dashedPen = new Pen(s.Color, 2) {DashStyle = DashStyle.Dash};
            int defaultOffSetVerticalOnGoing = _verticalLineLenght/3*2;
            int defaultOffSetVerticalOnComing = _verticalLineLenght/3;
            int defaultOffSetHorizontal = _horizontalLineLenght/3;

            for (int i = 0; i < s.ShiftPointList.Count - 1; i++)
            {
                int verticalOffSetStart;
                int verticalOffSetEnd;
                var shiftPoint = s.ShiftPointList[i];
                var nextShiftPoint = s.ShiftPointList[i + 1];
                var isHoritontalMouvement = nextShiftPoint.Y == shiftPoint.Y;
                int horizontalOffSetStart = horizontalOffset(shiftPoint.X)*defaultOffSetHorizontal;
                int horizontalOffSetEnd = horizontalOffset(nextShiftPoint.X)*defaultOffSetHorizontal;

                if (isHoritontalMouvement)
                {
                    if (i + 1 == s.ShiftPointList.Count - 1)
                    {
                        // dernier trajet horizontal
                        verticalOffSetStart = defaultOffSetVerticalOnComing;
                        verticalOffSetEnd = defaultOffSetVerticalOnComing;
                        //if (shiftPoint.X % 3 == 1)
                        //{
                        //    horizontalOffSetStart = 2*defaultOffSetHorizontal;
                        //}
                    }
                    else
                    {
                        verticalOffSetStart = defaultOffSetVerticalOnGoing;
                        verticalOffSetEnd = defaultOffSetVerticalOnGoing;
                    }
                }
                else
                {
                    if (i + 2 > s.ShiftPointList.Count - 1)
                    {
                        MessageBox.Show(@"Manque au moins un ShiftPoint dans la s");
                        continue;
                    }
                    verticalOffSetStart = defaultOffSetVerticalOnGoing;
                    if (i + 2 == s.ShiftPointList.Count - 1)
                    {
                        // dernier trajet vertical
                        verticalOffSetEnd = defaultOffSetVerticalOnComing;
                        //if (nextShiftPoint.X % 3 == 1)
                        //{
                        //    horizontalOffSetEnd = 2 * defaultOffSetHorizontal;
                        //}
                    }
                    else
                    {
                        verticalOffSetEnd = defaultOffSetVerticalOnGoing;
                    }
                }

                int x1 = DefaultUpperLeftAngleX + shiftPoint.X*_horizontalLineLenght + horizontalOffSetStart;
                int y1 = DefaultUpperLeftAngleY + _verticalTotalLenght -
                         (shiftPoint.Y*_verticalLineLenght + verticalOffSetStart);
                int x2 = DefaultUpperLeftAngleX + nextShiftPoint.X*_horizontalLineLenght + horizontalOffSetEnd;
                int y2 = DefaultUpperLeftAngleY + _verticalTotalLenght -
                         (nextShiftPoint.Y*_verticalLineLenght + verticalOffSetEnd);
                Action<PaintEventArgs> draw = e => e.Graphics.DrawLine(dashedPen, x1, y1, x2, y2);
                _drawList.Add(draw);
            }
        }
    }
}
