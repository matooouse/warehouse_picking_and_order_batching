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
            _alreadyDrawed.Clear();
            _alreadyDrawedWishes.Clear();
        }

        private readonly List<IPickings> _alreadyDrawedWishes = new List<IPickings>();

        public void DrawPickingObjectif(IPickings pickings)
        {
            if (_alreadyDrawedWishes.Contains(pickings))
                return;
            foreach (var pickingPos in pickings.PickingList)
            {
                var blueBrush = new SolidBrush(Color.Blue);
                int x = DefaultUpperLeftAngleX + pickingPos.UpperLeftX*_horizontalLineLenght;
                int y = DefaultUpperLeftAngleY + pickingPos.UpperLeftY*_verticalLineLenght;
                var rect = new Rectangle(x, y, _horizontalLineLenght, _verticalLineLenght);
                Action<PaintEventArgs> draw = e => e.Graphics.FillRectangle(blueBrush, rect);
                _drawList.Add(draw);
            }
            _alreadyDrawedWishes.Add(pickings);
        }

        private readonly List<ISolution> _alreadyDrawedSolution = new List<ISolution>();

        private int HorizontalOffset(Trip t)
        {
            if (t is RoundTrip)
            {
                return _horizontalLineLenght/3;
            }
            return _horizontalLineLenght/2;
        }

        private Action<PaintEventArgs> BuildActionDraw(Pen dashedPen, ShiftPoint start, ShiftPoint end,
            int verticalOffSetStart,
            int verticalOffSetEnd, int horizontalOffSetStart, int horizontalOffSetEnd)
        {
            int x1 = DefaultUpperLeftAngleX + start.X*_horizontalLineLenght + horizontalOffSetStart;
            int y1 = DefaultUpperLeftAngleY + _verticalTotalLenght -
                     (start.Y*_verticalLineLenght + verticalOffSetStart);
            int x2 = DefaultUpperLeftAngleX + end.X*_horizontalLineLenght + horizontalOffSetEnd;
            int y2 = DefaultUpperLeftAngleY + _verticalTotalLenght -
                     (end.Y*_verticalLineLenght + verticalOffSetEnd);
            Action<PaintEventArgs> draw = e => e.Graphics.DrawLine(dashedPen, x1, y1, x2, y2);
            return draw;
        }

        public void DrawSolution(ISolution s)
        {
            if (_alreadyDrawedSolution.Contains(s))
                return;
            var dashedPen = new Pen(s.Color, 2) {DashStyle = DashStyle.Dash};
            var trips = new List<Trip>();
            int i = 0;
            while (i < s.ShiftPointList.Count - 1)
            {
                var shiftPoint = s.ShiftPointList[i];
                var nextShiftPoint = s.ShiftPointList[i + 1];
                if (i < s.ShiftPointList.Count - 2)
                {
                    var nextNextShiftPoint = s.ShiftPointList[i + 2];
                    if (shiftPoint.Equals(nextNextShiftPoint))
                    {
                        var roundTrip = new RoundTrip(shiftPoint, nextShiftPoint);
                        trips.Add(roundTrip);
                        i++;
                    }
                    else
                    {
                        var isHoritontalMouvement = nextShiftPoint.Y == shiftPoint.Y;
                        SimpleTrip simpleTrip = i == 0
                            ? new FirstTrip(shiftPoint, nextShiftPoint, isHoritontalMouvement)
                            : new SimpleTrip(shiftPoint, nextShiftPoint, isHoritontalMouvement);
                        trips.Add(simpleTrip);
                    }
                }
                else
                {
                    var isHoritontalMouvement = nextShiftPoint.Y == shiftPoint.Y;
                    if (!isHoritontalMouvement)
                    {
                        MessageBox.Show(@"Manque au moins un ShiftPoint dans la solution");
                    }
                    var finalTrip = new FinalTrip(shiftPoint, nextShiftPoint, isHoritontalMouvement);
                    trips.Add(finalTrip);
                }
                i++;
            }

            var verticalOffSetEnd = 0;
            var horizontalOffSetStart = 0;
            int horizontalOffSetEnd = 0;
            for (int index = 0; index < trips.Count; index++)
            {
                var trip = trips[index];
                int verticalOffSetStart;
                if (trip is RoundTrip)
                {
                    horizontalOffSetStart = _horizontalLineLenght/3;
                    horizontalOffSetEnd = _horizontalLineLenght/3;
                    verticalOffSetStart = _verticalLineLenght/3*2;
                    verticalOffSetEnd = _verticalLineLenght/3*2;
                    // aller
                    var draw = BuildActionDraw(dashedPen, trip.Start, trip.End, verticalOffSetStart,
                        verticalOffSetEnd, horizontalOffSetStart, horizontalOffSetEnd);
                    _drawList.Add(draw);
                    // demi tour
                    horizontalOffSetEnd = _horizontalLineLenght/3*2;
                    var draw2 = BuildActionDraw(dashedPen, trip.End, trip.End, verticalOffSetStart,
                        verticalOffSetEnd, horizontalOffSetStart, horizontalOffSetEnd);
                    _drawList.Add(draw2);
                    // retour
                    horizontalOffSetStart = _horizontalLineLenght/3*2;
                    var nextTrip = trips[index + 1];
                    if (nextTrip is FinalTrip)
                    {
                        verticalOffSetEnd = _verticalLineLenght/3;
                    }
                    else
                    {
                        verticalOffSetEnd = _verticalLineLenght/3*2;
                    }
                    var draw3 = BuildActionDraw(dashedPen, trip.End, trip.Start, verticalOffSetStart,
                        verticalOffSetEnd, horizontalOffSetStart, horizontalOffSetEnd);
                    _drawList.Add(draw3);
                }
                else
                {
                    if (trip is FinalTrip)
                    {
                        // on recupère le previous offset
                        horizontalOffSetStart = horizontalOffSetEnd;
                        horizontalOffSetEnd = 0;
                        verticalOffSetStart = _verticalLineLenght/3;
                        verticalOffSetEnd = _verticalLineLenght/3;
                    }
                    else
                    {
                        var nextTrip = trips[index + 1];
                        verticalOffSetStart = _verticalLineLenght/3*2;
                        if (trip is FirstTrip)
                        {
                            horizontalOffSetStart = 0;
                            horizontalOffSetEnd = HorizontalOffset(nextTrip);
                            verticalOffSetEnd = _verticalLineLenght/3*2;
                        }
                        else
                        {
                            var simpleTrip = trip as SimpleTrip;
                            if (simpleTrip != null)
                            {
                                if (simpleTrip.IsHorizontal)
                                {
                                    // on recupère le previous offset
                                    horizontalOffSetStart = horizontalOffSetEnd;
                                    horizontalOffSetEnd = HorizontalOffset(nextTrip);
                                    verticalOffSetEnd = _verticalLineLenght/3*2;
                                }
                                else
                                {
                                    horizontalOffSetStart = _horizontalLineLenght/2;
                                    horizontalOffSetEnd = horizontalOffSetStart;
                                    if (nextTrip is FinalTrip)
                                    {
                                        verticalOffSetEnd = _verticalLineLenght/3;
                                    }
                                    else
                                    {
                                        verticalOffSetEnd = _verticalLineLenght/3*2;
                                    }
                                }
                            }
                        }
                    }
                    // aller
                    var draw = BuildActionDraw(dashedPen, trip.Start, trip.End, verticalOffSetStart,
                        verticalOffSetEnd, horizontalOffSetStart, horizontalOffSetEnd);
                    _drawList.Add(draw);
                }
            }
        }
    }
}
