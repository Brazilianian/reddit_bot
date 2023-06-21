namespace reddit_bot.domain.forms
{
    public class ComboBoxItem
    {
        public string Text { get; set; }
        public string Tag { get; set; }

        public ComboBoxItem(string text = "", string tag = "")
        {
            Text = text;
            Tag = tag;
        }

        public override string ToString()
        {
            return Text;
        }

    }
}
