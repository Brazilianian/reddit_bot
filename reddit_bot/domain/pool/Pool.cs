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

        public Pool()
        {
            _tasks = new List<PoolTask>();
            Range = new IntervalRange();
        }
    }
}
