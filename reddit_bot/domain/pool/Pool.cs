using reddit_bor.domain.pool;
using System.Collections.Generic;

namespace reddit_bor.domain.task
{
    public class Pool
    {
        public string Name { get; set; }
        public readonly List<PoolTask> _tasks;
        public IntervalRange Range { get; set; }
        public bool IsRandom { get; set; }
        public readonly List<PoolSubreddit> _subreddits;

        public Pool()
        {
            _tasks = new List<PoolTask>();
            _subreddits = new List<PoolSubreddit>();
            Range = new IntervalRange();
        }
    }
}
