﻿using reddit_bot.domain.task;
using System.Collections.Generic;

namespace reddit_bor.domain.pool
{
    public class PoolSubreddit
    {
        public string Name { get; set; }
        public PostFlair PostFlair { get; set; }
        public int Count { get; set; }
        public Trigger Trigger { get; set; }
        public string AdditionalInfo { get; set; }

        public PoolSubreddit() 
        {
            Count = 1;
        }

        public override bool Equals(object obj)
        {
            return obj is PoolSubreddit subreddit &&
                   Name == subreddit.Name &&
                   EqualityComparer<PostFlair>.Default.Equals(PostFlair, subreddit.PostFlair) &&
                   Count == subreddit.Count &&
                   EqualityComparer<Trigger>.Default.Equals(Trigger, subreddit.Trigger);
        }

        public override int GetHashCode()
        {
            int hashCode = -771201060;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<PostFlair>.Default.GetHashCode(PostFlair);
            hashCode = hashCode * -1521134295 + Count.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Trigger>.Default.GetHashCode(Trigger);
            return hashCode;
        }

        public override string ToString()
        {
            return $"Name: {Name}; Trigger: {Trigger}";
        }
    }
}
