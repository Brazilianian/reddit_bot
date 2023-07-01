namespace reddit_bot.domain.task
{
    public class RedditPostTaskLink : RedditPostTask
    {
        public RedditPostTaskLink() : base() { }

        public RedditPostTaskLink(string title, string link) 
            : base(title)
        {
            Link = link;
        }

        public RedditPostTaskLink(string title, bool isSpoiler, bool isNSFW, string link) 
            : base(title, isSpoiler, isNSFW)
        {
            Link = link;
        }

        public string Link { get; set; }
    }
}
