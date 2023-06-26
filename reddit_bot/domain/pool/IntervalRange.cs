namespace reddit_bor.domain.pool
{
    public class IntervalRange
    {
        public IntervalRange() { }

        private int from;
        private int to;

        public int From { get => from; set => from = from > to ? to : value; }
        public int To { get => to; set => to = to < from ? from : value; }

        public IntervalRange(int from, int to)
        {
            From = from;
            To = to;
        }
    }
}
