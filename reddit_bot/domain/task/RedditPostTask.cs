namespace reddit_bot.domain.task
{
    public abstract class RedditPostTask
    {
        public RedditPostTask(string taskName, string subredditName, string title)
        {
            TaskName = taskName;
            SubredditName = subredditName;
            Title = title;
        }

        public string TaskName { get; set; }

        public string SubredditName { get; set; }
        public string Title { get; set; }
        public bool IsSpoiler { get; set; }
        public bool IsNSFW { get; set; }
        public PostFlair postFlair { get; set; }
    }
}
