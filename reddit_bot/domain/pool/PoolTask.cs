using reddit_bot.domain.task;
using System.Collections.Generic;

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

        public override bool Equals(object obj)
        {
            return obj is PoolTask task &&
                   EqualityComparer<RedditPostTask>.Default.Equals(PostTask, task.PostTask) &&
                   Count == task.Count;
        }

        public override int GetHashCode()
        {
            int hashCode = -1104365422;
            hashCode = hashCode * -1521134295 + EqualityComparer<RedditPostTask>.Default.GetHashCode(PostTask);
            hashCode = hashCode * -1521134295 + Count.GetHashCode();
            return hashCode;
        }
    }
}
