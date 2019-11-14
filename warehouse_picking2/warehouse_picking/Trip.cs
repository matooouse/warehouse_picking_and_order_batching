namespace warehouse_picking
{
    internal abstract class Trip
    {
        public Trip(ShiftPoint start, ShiftPoint end)
        {
            End = end;
            Start = start;
        }

        internal ShiftPoint Start { get; private set; }
        internal ShiftPoint End { get; private set; }
    }

    internal class RoundTrip : Trip
    {
        public RoundTrip(ShiftPoint start, ShiftPoint end) : base(start, end)
        {
        }
    }
    internal class SimpleTrip : Trip
    {
        internal bool IsHorizontal { get; private set; }

        internal SimpleTrip(ShiftPoint start, ShiftPoint end, bool isHorizontal)
            : base(start, end)
        {
            IsHorizontal = isHorizontal;
        }
    }
    internal class FinalTrip : SimpleTrip
    {
        internal FinalTrip(ShiftPoint start, ShiftPoint end, bool isHorizontal)
            : base(start, end, isHorizontal)
        {
        }
    }
    internal class PreviousFinalTrip : SimpleTrip
    {
        internal PreviousFinalTrip(ShiftPoint start, ShiftPoint end, bool isHorizontal)
            : base(start, end, isHorizontal)
        {
        }
    }
    internal class FirstTrip : SimpleTrip
    {
        internal FirstTrip(ShiftPoint start, ShiftPoint end, bool isHorizontal)
            : base(start, end, isHorizontal)
        {
        }
    }
}
