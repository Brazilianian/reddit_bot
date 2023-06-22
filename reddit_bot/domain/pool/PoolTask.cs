using reddit_bot.domain.task;

namespace reddit_bor.domain.pool
{
    public  class PoolTask
    {
        public RedditPostTask PostTask { get; set; }
        public int Count { get; set; }
        
        public PoolTask() { }

        public PoolTask(RedditPostTask postTask, int count) 
        {
            PostTask = postTask;
            Count = count;
        }
    }
}
