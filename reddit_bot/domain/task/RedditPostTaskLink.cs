namespace reddit_bot.domain.task
{
    public class RedditPostTaskLink : RedditPostTask
    {
        public RedditPostTaskLink() : base() { }

        public RedditPostTaskLink(string taskName, string subredditName, string title, string link) 
            : base(taskName, subredditName, title)
        {
            Link = link;
        }

        public RedditPostTaskLink(string taskName, string subredditName, string title, bool isSpoiler, bool isNSFW, string link)
            : base(taskName, subredditName, title, isSpoiler, isSpoiler)
        {
            Link = link;
        }

        public string Link { get; set; }
    }
}
