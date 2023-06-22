﻿namespace reddit_bot.domain.task
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
    }
}