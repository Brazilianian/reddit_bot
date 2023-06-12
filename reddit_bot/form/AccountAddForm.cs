﻿using System;
using System.Threading;
using System.Windows.Forms;
using Reddit;
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
            var redditAccount = new RedditAccount
            {
                AppId = textBox1.Text,
                Secret = textBox2.Text
            };

            _redditService.Login(redditAccount.AppId, redditAccount.Secret);

            label3.Text = "Тепер підтвердіть використання акаунта в браузері";
            while (_redditService.GetAccessToken() == null || _redditService.GetRefreshToken() == null)
            {
                Thread.Sleep(500);
            }

            redditAccount.AccessToken = _redditService.GetAccessToken();
            redditAccount.RefreshToken = _redditService.GetRefreshToken();
            
            //FIXME edit user agent
            RedditClient redditClient = _redditService.GetRedditClient(redditAccount,
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36");

            if (_accountService.IsAccountAlreadyExists(redditClient.Account.Me.Id))
            {
                label3.Text = "Такий користувач вже існує";
                return;
            }

            redditAccount.AccountId = redditClient.Account.Me.Id;
            _accountService.Save(redditAccount);
            label3.Text = "Акаунт успішно збережено";
        }
    }
}