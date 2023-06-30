using System.Collections.Generic;

namespace reddit_bot.domain.task
{
    public abstract class RedditPostTask
    {
        protected RedditPostTask() { }

        protected RedditPostTask(string title)
        {
            Title = title;
        }

        protected RedditPostTask(string title, bool isSpoiler, bool isNSFW)
            : this(title)
        {
            IsSpoiler = isSpoiler;
            IsNSFW = isNSFW;
        }

        public string Title { get; set; }
        public bool IsSpoiler { get; set; }
        public bool IsNSFW { get; set; }

        public override bool Equals(object obj)
        {
            return obj is RedditPostTask task &&
                   Title == task.Title &&
                   IsSpoiler == task.IsSpoiler &&
                   IsNSFW == task.IsNSFW;
        }

        public override int GetHashCode()
        {
            int hashCode = 1787047951;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Title);
            hashCode = hashCode * -1521134295 + IsSpoiler.GetHashCode();
            hashCode = hashCode * -1521134295 + IsNSFW.GetHashCode();
            return hashCode;
        }
    }
}
