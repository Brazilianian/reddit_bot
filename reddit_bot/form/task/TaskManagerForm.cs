using reddit_bot.domain;
using System.Windows.Forms;

namespace reddit_bot.form.task
{
    public partial class TaskManagerForm : Form
    {
        private readonly RedditAccount _redditAccount;
        private readonly AccountsForm _accountsForm;

        public TaskManagerForm(RedditAccount redditAccount, AccountsForm accountsForm)
        {
            InitializeComponent();

            _redditAccount = redditAccount;
            _accountsForm = accountsForm;
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            AccountInfoForm accountInfoForm = new AccountInfoForm(_redditAccount, _accountsForm);
            accountInfoForm.Show();
            Close();
        }

        private void button5_Click(object sender, System.EventArgs e)
        {
            CreateTaskForm createTaskForm = new CreateTaskForm(_redditAccount, _accountsForm);
            createTaskForm.Show();
            Close();
        }

        private void button4_Click(object sender, System.EventArgs e)
        {
            AccountInfoForm accountInfoForm = new AccountInfoForm(_redditAccount, _accountsForm);
            accountInfoForm.Show();
            Close();
        }
    }
}
