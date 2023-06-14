using System;
using System.Drawing;
using System.Linq;
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

            bool isOs = ((CheckBox)Controls.Find("checkBoxOc", true)[0]).Checked;
            bool isSpoiler = ((CheckBox)Controls.Find("checkBoxSpoiler", true)[0]).Checked;
            bool isNsfw = ((CheckBox)Controls.Find("checkBoxNsfw", true)[0]).Checked;

            SelfPost post = _subreddit
                .SelfPost(title: title, selfText: text)
                .Submit(spoiler: isSpoiler); 

            if (isNsfw)
            {
                post.MarkNSFWAsync();
            }

            //TODO OC tag
        
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

        private void loadSubreddit(object sender, EventArgs e)
        {
            var subredditName = ((TextBox)sender).Text;
            _subreddit = _redditClient.Subreddit(subredditName);
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
                    AddAttributesInputs();
                    AddSendButton();
                    break;
            }
        }

        private void AddAttributesInputs()
        {
            var checkBoxPanel = new Panel()
            {
                Size = new Size(350, 100),
                BorderStyle = BorderStyle.FixedSingle,

            };
            checkBoxPanel.Location = new Point(panel2.Width - checkBoxPanel.Width - 5, 260);

            var checkBoxOc = new CheckBox()
            {
                Text = "OC (не працює)",
                Name = "checkBoxOc",
                AutoSize = true,
                Location = new Point(5, 5)
            };
            checkBoxPanel.Controls.Add(checkBoxOc);

            var checkBoxSpoiler = new CheckBox()
            {
                Text = "Спойлер",
                Name = "checkBoxSpoiler",
                AutoSize = true,
                Location = new Point(checkBoxOc.Location.X + checkBoxOc.Width + 5, 5)
            };
            checkBoxPanel.Controls.Add(checkBoxSpoiler);

            var checkBoxNsfw = new CheckBox()
            {
                Text = "Небезпечний для роботи",
                Name = "checkBoxNsfw",
                AutoSize = true,
                Location = new Point(checkBoxSpoiler.Location.X + checkBoxSpoiler.Width + 5, 5)
            };
            checkBoxPanel.Controls.Add(checkBoxNsfw);

            panel2.Controls.Add(checkBoxPanel);
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

        //Back
        private void button1_Click(object sender, EventArgs e)
        {
            AccountInfoForm accountInfoForm = new AccountInfoForm(_redditAccount, _accountsForm);
            accountInfoForm.Show();
            Close();
        }
    }
}