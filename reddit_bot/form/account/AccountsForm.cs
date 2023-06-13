﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Reddit;
using reddit_bot.domain;
using reddit_bot.service;

namespace reddit_bot
{
    public partial class AccountsForm : Form
    {
        private List<RedditAccount> _accounts;
        private readonly AccountService _accountService;
        private readonly RedditService _redditService;

        public AccountsForm()
        {
            InitializeComponent();

            _accountService = new AccountService();
            _redditService = new RedditService();

            _accounts = _accountService.GetAllAccounts();
            FillAccountsDataGrid();
        }

        private void FillAccountsDataGrid()
        {
            dataGridView1.Columns.Add(new DataGridViewColumn()
            {
                Name = "Ім'я"
            });
            dataGridView1.Columns.Add(new DataGridViewColumn()
            {
                Name = "Статус"
            });

            foreach (var account in _accounts)
            {
                DataGridViewRow dataGridViewRow = new DataGridViewRow();
                dataGridViewRow.Tag = account.AccountId;

                RedditClient redditClient = _redditService.GetRedditClient(account, RequestsUtil.GetUserAgent());
                _accountService.UpdateAccountStatus(account);

                dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell()
                {
                    Value = redditClient.Account.Me.Name
                });
                dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell()
                {
                    Value = account.Status
                });

                dataGridView1.Rows.Add(dataGridViewRow);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var accountAddForm = new AccountAddForm();
            accountAddForm.ShowDialog();
            RefreshDataGrid();
        }

        private void RefreshDataGrid()
        {
            dataGridView1.DataSource = null;
            _accounts = _accountService.GetAllAccounts();
            FillAccountsDataGrid();
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            var selectedRow = ((DataGridView)sender).SelectedRows[0];
            
            var accountId = selectedRow.Tag.ToString();
            var redditAccount = _accountService.GetAccountByAccountId(accountId);
            
            var accountInfoForm = new AccountInfoForm(redditAccount, this);
            accountInfoForm.Show();
            Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}