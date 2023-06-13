using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Reddit;
using Reddit.Controllers;
using reddit_bor.domain.task;
using reddit_bot.domain;
using reddit_bot.service;

namespace reddit_bot.form.task
{
    public partial class CreateTaskForm : Form
    {
        private readonly RedditAccount _redditAccount;
        private readonly AccountsForm _accountsForm;
        private readonly RedditService _reddditService;

        private Subreddit _subreddit = null;

        private RedditClient _redditClient;

        public CreateTaskForm(RedditAccount redditAccount, AccountsForm accountsForm)
        {
            InitializeComponent();

            _reddditService = new RedditService();
            _redditAccount = redditAccount;
            _accountsForm = accountsForm;

            _redditClient = _reddditService.GetRedditClient(_redditAccount, RequestsUtil.GetUserAgent());

            FillForm();
        }

        private void createPost_Post(object sender, EventArgs e)
        {
            string title = Controls.Find("textBoxTitle", true).First().Text;
            string text = Controls.Find("richTextBoxText", true).First().Text;  

            if (_subreddit == null)
            {
                MessageBox.Show("Невірне ім'я сабредіту");
                return;
            }

            _subreddit.SelfPost(title: title, selfText: text).Submit();
            MessageBox.Show("Відправлено");
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var postType = (TaskType)int.Parse(((ComboBox)sender).SelectedIndex.ToString());

            switch (postType)
            {
                case TaskType.POST:
                    FillTaskCreationPanel(postType);
                    break;
            }
        }

        private void FillForm()
        {
            comboBox1.Items.AddRange(Enum.GetNames(typeof(TaskType)));
        }

        private void FillTaskCreationPanel(TaskType postType)
        {
            Label label = new Label();
            label.Text = "Виберіть тип посту";
            label.AutoSize = true;
            label.Location = new Point(5, 5);
            panel4.Controls.Add(label);

            ComboBox comboBox = new ComboBox();
            comboBox.Items.AddRange(Enum.GetNames(typeof(TaskPostType)));
            comboBox.Location = new Point(label.Location.X, label.Location.Y + label.Height + 5);
            comboBox.Width = panel4.Width - 10;
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.SelectedIndexChanged += fillInputPanel;
            panel4.Controls.Add(comboBox);
        }

        private void fillInputPanel(object sender, EventArgs e)
        {
            TaskPostType taskPostType = (TaskPostType)((ComboBox)sender).SelectedIndex;
            switch (taskPostType)
            {
                case TaskPostType.POST:
                    AddSubredditName();
                    AddTitle();
                    AddText();
                    AddSendButton();
                    break;
            }
        }

        private void AddSubredditName()
        {
            Label label = new Label()
            {
                Location = new Point(5, 5),
                Text = "Введіть назву сабреддіту. Напр. (r/announcements)",
                AutoSize = true
            };

            TextBox textBoxSubereddit = new TextBox()
            {
                Location = new Point(label.Location.X, label.Location.Y + label.Height + 5),
                Size = new Size(80, 35),
                Name = "textBoxSubereddit"
            };

            textBoxSubereddit.TextChanged += loadSubreddit;

            panel2.Controls.Add(label);
            panel2.Controls.Add(textBoxSubereddit);
        }

        private void loadSubreddit(object sender, EventArgs e)
        {
            var subredditName = ((TextBox)sender).Text;
            _subreddit = _redditClient.Subreddit(subredditName);
        }

        private void AddSendButton()
        {
            Button button = new Button()
            {
                Text = "Опублікувати",
                Size = new Size(80, 45),
            };

            button.Location = new Point(panel2.Width - button.Width - 5, panel2.Height - button.Height - 5);
           
            button.Click += createPost_Post;
            panel2.Controls.Add(button);
        }

        private void AddText()
        {
            Label labelText = new Label()
            {
                Location = new Point(5, 120),
                Text = "Текст (необов'язково):",
                AutoSize = true
            };

            RichTextBox richTextBoxText = new RichTextBox()
            {
                Name = "richTextBoxText",
                Height = 80,
                Width = panel2.Width - 10,
                Location = new Point(labelText.Location.X, labelText.Location.Y + labelText.Height + 5)
            };

            panel2.Controls.Add(labelText);
            panel2.Controls.Add(richTextBoxText);
        }
    

        private void AddTitle()
        {
            Label labelTitle = new Label()
            {
                Location = new Point(5, 60),
                Text = "Заголовок:",
                AutoSize = true,
            };

            TextBox textBoxTitle = new TextBox()
            {
                Name = "textBoxTitle",
                Height = 30,
                Width = panel2.Width - 10,
                Location = new Point(labelTitle.Location.X, labelTitle.Location.Y + labelTitle.Height + 5)
            };

        
            panel2.Controls.Add(labelTitle);
            panel2.Controls.Add(textBoxTitle);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AccountInfoForm accountInfoForm = new AccountInfoForm(_redditAccount, _accountsForm);
            accountInfoForm.Show();
            Close();
        }
    }
}