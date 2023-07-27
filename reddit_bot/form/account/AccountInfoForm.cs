using System;
using System.Windows.Forms;
using Reddit;
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
            InitializeComponent();

            _redditService = new RedditService();
            
            _accountsForm = accountsForm;
            _redditAccount = redditAccount;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _accountsForm.Show();
            Close();
        }

        private void AccountInfoForm_Load(object sender, EventArgs e)
        {
            _redditClient = _redditService.GetRedditClient(_redditAccount, RequestsUtil.GetUserAgent());

            FillForm();
            FillTasks();
        }

        private void FillTasks()
        {
            //TODO get tasks
        }

        private void FillForm()
        {
            label1.Text = _redditClient.Account.Me.Name;
            label2.Text = _redditClient.Subreddit().Name;
            label4.Text = _redditClient.Account.Me.LinkKarma.ToString();
            label8.Text = _redditClient.Account.Me.Created.ToString("D") + " д.";
        }

        #region Menu Panel
        private void button3_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion

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
    }
}