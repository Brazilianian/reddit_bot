namespace reddit_bor.domain.pool
{
    public class IntervalRange
    {
        public IntervalRange() { }
        
        private int From { get; set; }
        private int To { get; set; }

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
