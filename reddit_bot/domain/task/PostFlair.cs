using System.Collections.Generic;

namespace reddit_bot.domain.task
{
    public class PostFlair
    {
        public string Text { get; set; }
        public string Id { get; set; }

        public PostFlair(string text, string id="")
        {
            Text = text;
            Id = id;
        }

        public override bool Equals(object obj)
        {
            return obj is PostFlair flair &&
                   Text == flair.Text &&
                   Id == flair.Id;
        }

        public override int GetHashCode()
        {
            int hashCode = -2123746546;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Text);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Id);
            return hashCode;
        }
    }
}
