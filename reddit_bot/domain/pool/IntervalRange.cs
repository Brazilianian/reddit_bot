namespace reddit_bor.domain.pool
{
    public class IntervalRange
    {
        public IntervalRange() { }

        private int to;
        private int from;

        public int From { get => from; set => from = value; }
        public int To { get => to; set => to = value; }

        public IntervalRange(int from, int to)
        {
            To = to;
            From = from;
        }
    }
}
