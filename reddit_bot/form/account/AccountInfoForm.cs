using System;
using System.Windows.Forms;
using Reddit;
using reddit_bor.form;
using reddit_bor.form.log;
using reddit_bor.form.preset;
using reddit_bor.form.publish;
using reddit_bot.domain;
using reddit_bot.service;

namespace reddit_bot
{
    public partial class AccountInfoForm : Form
    {
        private readonly AccountsForm _accountsForm;
        private readonly RedditService _redditService;

        private readonly RedditAccount _redditAccount;
        
        private RedditClient _redditClient;
        
        public AccountInfoForm(RedditAccount redditAccount, AccountsForm accountsForm)
        {
            _redditService = new RedditService();

            _accountsForm = accountsForm;
            _redditAccount = redditAccount;
            _redditClient = _redditService.GetRedditClient(_redditAccount, RequestsUtil.GetUserAgent());

            InitializeComponent();
            FillForm();
        }

        private void FillForm()
        {
            try
            {
                label1.Text = _redditClient.Account.Me.Name;
                label2.Text = _redditClient.Subreddit().Name;
                label4.Text = _redditClient.Account.Me.LinkKarma.ToString();
                label8.Text = _redditClient.Account.Me.Created.ToString("D") + " д.";
            } catch (Exception ex) 
            {
                MessageBox.Show($"Помилка підключення до акакунта\n{ex.Message}");
                _accountsForm.Show();
                Close();
            }
        }
        #region Menu Panel
        private void button1_Click(object sender, EventArgs e)
        {
            _accountsForm.Show();
            Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            NewPublishForm newPublishForm = new NewPublishForm(_redditAccount, _accountsForm);
            newPublishForm.Show();
            Close();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            PresetForm presetForm = new PresetForm(_redditAccount, _accountsForm);
            presetForm.Show();
            Close();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            LogForm logForm = new LogForm(_accountsForm, _redditAccount);
            logForm.Show();
            Close();
        }
        #endregion

        #region Forbid Close Button
        private const int CP_DISABLE_CLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle = cp.ClassStyle | CP_DISABLE_CLOSE_BUTTON;
                return cp;
            }
        }
        #endregion
    }
}