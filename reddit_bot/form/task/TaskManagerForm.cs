using System.Windows.Forms;

namespace reddit_bot.form.task
{
    public partial class TaskManagerForm : Form
    {
        public TaskManagerForm(reddit_bot.domain.RedditAccount _redditAccount, reddit_bot.AccountsForm _accountsForm)
        {
            InitializeComponent();
        }
    }
}
