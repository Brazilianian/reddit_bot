using System;
using System.Drawing;
using System.Windows.Forms;
using Reddit;
using reddit_bot.domain;
using reddit_bot.form.task;
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
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CreateTaskForm createTaskForm = new CreateTaskForm(_redditAccount, _accountsForm);
            createTaskForm.Show();
            Close();
        }
    }
}