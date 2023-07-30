using System;
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

        private Thread _loginThread;

        [Obsolete]
        public AccountAddForm(AccountsForm accountsForm)
        {
            InitializeComponent();

            _redditService = new RedditService();
            _accountService = new AccountService();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label3.Text = "Тепер підтвердіть використання акаунта в браузері";

            _loginThread = new Thread(LoginNewAccount);
            _loginThread.Start();
        }

        private void LoginNewAccount()
        {
            var redditAccount = new RedditAccount
            {
                AppId = textBox1.Text,
                Secret = textBox2.Text
            };

            _redditService.Login(redditAccount.AppId, redditAccount.Secret);

            //Crutch. Waits 10sec or suspend login
            //upd wait infinity
            while(true) { 
                
                if (_redditService.GetAccessToken() == null || _redditService.GetRefreshToken() == null)
                {
                    Thread.Sleep(500);
                } else
                {
                    break;
                }
            }

            redditAccount.AccessToken = _redditService.GetAccessToken();
            redditAccount.RefreshToken = _redditService.GetRefreshToken();

            if (redditAccount.AccessToken == null || redditAccount.RefreshToken == null)
            {
                Invoke(new Action(() =>
                {
                    label3.Text = "Помилка авторизації (очікування 20 сек.)";
                }));
                _redditService.StopListen();
                return;
            }

            RedditClient redditClient = _redditService.GetRedditClient(redditAccount, RequestsUtil.GetUserAgent());
            if (_accountService.IsAccountAlreadyExists(redditClient.Account.Me.Id))
            {
                Invoke(new Action(() =>
                {
                    label3.Text = "Такий користувач вже існує";
                }));
                _redditService.StopListen();
                return;
            }

            redditAccount.AccountId = redditClient.Account.Me.Id;
            _accountService.Save(redditAccount);

            Invoke(new Action(() =>
            {
                label3.Text = "Акаунт успішно збережено";
            }));
            _redditService.StopListen();

            Invoke(new Action(() =>
            {
                Close();
            }));
        }

        [Obsolete]
        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        [Obsolete]
        private void AccountAddForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _redditService.StopListen();

            if (_loginThread != null)
            {
                if(_loginThread.IsAlive)
                {
                    _loginThread.Suspend();
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}