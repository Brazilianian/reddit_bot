using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace reddit_bor.domain.task
{
    public class RedditPostTaskLink : RedditPostTask
    {
        public RedditPostTaskLink(string subredditName, string title, string link) : base(subredditName, title)
        {
            Link = link;
        }

        public string Link { get; set; }
    }
}
