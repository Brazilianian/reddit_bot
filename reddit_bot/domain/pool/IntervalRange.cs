namespace reddit_bor.domain.pool
{
    public class IntervalRange
    {
        public IntervalRange() { }

        public IntervalRange(int from, int to) 
        {
            From = from;
            To = to;
        }

        public int From { get; private set; }
        public int To { get; private set; }

        public void SetFrom(int from) {
            if (To >= from)
            {
                From = from;
            }
        }

        public void SetTo(int to)
        {
            if (From <= to) 
            {
                To = to;
            }
        }
    }
}
