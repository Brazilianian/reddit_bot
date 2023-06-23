namespace reddit_bot.domain.task
{
    public abstract class RedditPostTask
    {
        protected RedditPostTask() { }

        protected RedditPostTask(string taskName, string subredditName, string title)
        {
            TaskName = taskName;
            SubredditName = subredditName;
            Title = title;
        }

        protected RedditPostTask(string taskName, string subredditName, string title, bool isSpoiler, bool isNSFW)
            : this(taskName, subredditName, title)
        {
            IsSpoiler = isSpoiler;
            IsNSFW = isNSFW;
        }

        public string TaskName { get; set; }

        public string SubredditName { get; set; }
        public string Title { get; set; }
        public bool IsSpoiler { get; set; }
        public bool IsNSFW { get; set; }
        public PostFlair postFlair { get; set; }

        public override string ToString()
        {
            return TaskName;
        }
    }
}
