namespace reddit_bot.domain.task
{
    public class RedditPostTaskLink : RedditPostTask
    {
        public RedditPostTaskLink(string taskName, string subredditName, string title, string link) 
            : base(taskName, subredditName, title)
        {
            Link = link;
        }

        public string Link { get; set; }
    }
}
