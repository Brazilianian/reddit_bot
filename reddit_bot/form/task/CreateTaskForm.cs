using System;
using System.Windows.Forms;
using reddit_bot.domain;

namespace reddit_bot.form.task
{
    public partial class CreateTaskForm : Form
    {
        private readonly RedditAccount _redditAccount;
        private readonly AccountsForm _accountsForm;
        
        public CreateTaskForm(RedditAccount redditAccount, AccountsForm accountsForm)
        {
            InitializeComponent();

            _redditAccount = redditAccount;
            _accountsForm = accountsForm;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }
    }
}