namespace reddit_bor.domain.task
{
    public abstract class RedditPostTask
    {
        public string SubredditName { get; set; }
        public string Title { get; set; }
        public bool IsSpoiler { get; set; }
        public bool IsNSFW { get; set; }
        public PostFlair postFlair { get; set; }
    }
}
