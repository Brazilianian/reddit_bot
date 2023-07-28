using static System.Net.Mime.MediaTypeNames;

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
            return $"Type: Post; Title: {Title}; Link: {Link}; Is Spoiler: {IsSpoiler}; Is NSFW: {IsNSFW}; ";
        }
    }
}
