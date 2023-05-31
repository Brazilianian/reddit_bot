using System;
using System.Windows.Forms;
using reddit_bot.domain;
using reddit_bot.service;

namespace reddit_bot
{
    public partial class AccountAddForm : Form
    {
        private RedditService _redditService;
        private AccountService _accountService;

        public AccountAddForm()
        {
            InitializeComponent();

            _redditService = new RedditService();
            _accountService = new AccountService();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RedditAccount redditAccount = new RedditAccount();
            redditAccount.Username = textBox1.Text;
            redditAccount.Password = textBox2.Text;
            _redditService.Login(redditAccount.Username, redditAccount.Password);

            AddAccountCredentialsForm addAccountCredentialsForm = new AddAccountCredentialsForm();
            if (addAccountCredentialsForm.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            redditAccount.AccessToken = addAccountCredentialsForm.AccessToken;
            redditAccount.RefreshToken = addAccountCredentialsForm.RefreshToken;

            if (_accountService.IsAccountAlreadyExists(redditAccount))
            {
                MessageBox.Show("Current account already exists");
                return;
            }

            _accountService.Save(redditAccount);
        }
    }
}