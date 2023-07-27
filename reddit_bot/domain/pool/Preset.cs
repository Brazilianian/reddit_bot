using System.Collections.Generic;

namespace reddit_bor.domain.pool
{
    public class Preset
    {
        public string Name { get; set; }
        public List<PoolSubreddit> Subreddits { get; set; }
    }
}
