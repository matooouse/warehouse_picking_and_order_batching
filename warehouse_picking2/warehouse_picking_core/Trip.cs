namespace warehouse_picking_core
{
    public abstract class Trip
    {
        protected Trip(ShiftPoint start, ShiftPoint end)
        {
            End = end;
            Start = start;
        }

        public ShiftPoint Start { get; private set; }
        public ShiftPoint End { get; private set; }
    }

    public class RoundTrip : Trip
    {
        public RoundTrip(ShiftPoint start, ShiftPoint end) : base(start, end)
        {
        }
    }

    public class SimpleTrip : Trip
    {
        public bool IsHorizontal { get; private set; }

        public SimpleTrip(ShiftPoint start, ShiftPoint end, bool isHorizontal)
            : base(start, end)
        {
            IsHorizontal = isHorizontal;
        }
    }

    public class FinalTrip : SimpleTrip
    {
        public FinalTrip(ShiftPoint start, ShiftPoint end, bool isHorizontal)
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

    public class FirstTrip : SimpleTrip
    {
        public FirstTrip(ShiftPoint start, ShiftPoint end, bool isHorizontal)
            : base(start, end, isHorizontal)
        {
        }
    }
}
