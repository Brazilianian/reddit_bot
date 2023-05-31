using System;
using System.Collections.Generic;
using System.Windows.Forms;
using reddit_bot.domain;
using reddit_bot.service;

namespace reddit_bot
{
    public partial class AccountsForm : Form
    {
        private readonly List<RedditAccount> _accounts;
        private readonly AccountService _accountService;
        public AccountsForm()
        {
            InitializeComponent();

            _accountService = new AccountService();
            _accounts = _accountService.GetAllAccounts();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var accountAddForm = new AccountAddForm();
            accountAddForm.ShowDialog();
        }
    }
}