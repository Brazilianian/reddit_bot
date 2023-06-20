using System.ComponentModel;

namespace reddit_bor.domain.task
{
    public enum TaskPostType
    {
        [Description("Пост")]
        POST,

        [Description("Посилання")]
        LINK,
    }
}
