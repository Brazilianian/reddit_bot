using System.ComponentModel;

namespace reddit_bor.domain.task
{
    internal enum TaskPostType
    {
        [Description("Пост")]
        POST,

        [Description("Посилання")]
        LINK,
    }
}
