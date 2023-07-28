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

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"Type: Post; Title: {Title}; Text: {Text}; Is Spoiler: {IsSpoiler}; Is NSFW: {IsNSFW}; ";            
        }
    }
}
