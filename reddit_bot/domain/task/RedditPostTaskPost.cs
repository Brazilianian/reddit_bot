namespace reddit_bot.domain.task
{
    public class RedditPostTaskPost : RedditPostTask
    {
        public RedditPostTaskPost(string taskName, string subredditName, string title, string text) 
            : base(taskName, subredditName, title)
        {
            Text = text;
        }

        public string Text { get; set; }
    }
}
