namespace reddit_bot.domain.task
{
    public class RedditPostTaskPost : RedditPostTask
    {
        public RedditPostTaskPost() : base() { }

        public RedditPostTaskPost(string taskName, string subredditName, string title, string text) 
            : base(taskName, subredditName, title)
        {
            Text = text;
        }

        public RedditPostTaskPost(string taskName, string subredditName, string title, bool isSpoiler, bool isNSFW, string text) 
            : base(taskName, subredditName, title, isSpoiler, isNSFW)
        {
            Text = text;
        }

        public string Text { get; set; }
    }
}
