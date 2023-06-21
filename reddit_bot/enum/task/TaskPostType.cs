using System.ComponentModel;

namespace reddit_bot.domain.task
{
    public enum TaskPostType
    {
        [Description("Пост")]
        POST,

        [Description("Посилання")]
        LINK,
    }
}
