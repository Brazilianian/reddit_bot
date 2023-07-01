namespace reddit_bot.domain.task
{
    public class RedditPostTaskPost : RedditPostTask
    {
        public RedditPostTaskPost() : base() { }

        public RedditPostTaskPost(string title, string text) 
            : base(title)
        {
            Text = text;
        }

        public RedditPostTaskPost(string title, bool isSpoiler, bool isNSFW, string text) 
            : base(title, isSpoiler, isNSFW)
        {
            Text = text;
        }

        public string Text { get; set; }
    }
}
