using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Reddit;
using Reddit.Controllers;
using Reddit.Exceptions;
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

        //Task Type
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

            try
            {
                SelfPost post = _subreddit
                    .SelfPost(title: title, selfText: text)
                    .Submit(spoiler: isSpoiler);

                if (isNsfw)
                {
                    post.MarkNSFWAsync();
                }
                MessageBox.Show("Відправлено");
            }
            catch (RedditControllerException exception)
            {
                MessageBox.Show(exception.Message);
            }

            //TODO OC tag

        }

        private void createPost_Link(object sender, EventArgs e)
        {
            string title = Controls.Find("textBoxTitle", true).First().Text;
            string link = Controls.Find("textBoxLink", true).First().Text;

            if (_subreddit == null)
            {
                MessageBox.Show("Невірне ім'я сабредіту");
                return;
            }

            bool isOs = ((CheckBox)Controls.Find("checkBoxOc", true)[0]).Checked;
            bool isSpoiler = ((CheckBox)Controls.Find("checkBoxSpoiler", true)[0]).Checked;
            bool isNsfw = ((CheckBox)Controls.Find("checkBoxNsfw", true)[0]).Checked;

            try
            {
                LinkPost post = _subreddit
                    .LinkPost(title: title, url: link)
                    .Submit(spoiler: isSpoiler);

                if (isNsfw)
                {
                    post.MarkNSFWAsync();
                }
                MessageBox.Show("Відправлено");
            }
            catch (RedditControllerException exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void FillForm()
        {
            comboBox1.Items.AddRange(Enum.GetNames(typeof(TaskType)));
        }

        private void fillInputPanel(object sender, EventArgs e)
        {
            TaskPostType taskPostType = (TaskPostType)((ComboBox)sender).SelectedIndex;
            panel2.Controls.Clear();

            switch (taskPostType)
            {
                case TaskPostType.POST:
                    AddSubredditName();
                    AddTitle();
                    AddText();
                    AddAttributesInputs();
                    AddFlairs();
                    AddSendButtonPost();
                    break;

                case TaskPostType.LINK:
                    AddSubredditName();
                    AddTitle();
                    AddLink();
                    AddAttributesInputs();
                    AddFlairs();
                    AddSendButtonLink();
                    break;
            }
        }

        private void loadSubreddit(object sender, EventArgs e)
        {
            var comboBoxSubereddits = (ComboBox)Controls.Find("comboBoxSubereddits", true)[0];
            _subreddit = _redditClient.Subreddit(comboBoxSubereddits.Text);
        }

        private void loadSubredditsByName(object sender, EventArgs e)
        {
            var comboBoxSubereddits = (ComboBox)Controls.Find("comboBoxSubereddits", true)[0];

            List<Subreddit> subreddits = _redditClient.SearchSubreddits(comboBoxSubereddits.Text, limit: 10);

            comboBoxSubereddits.Items.Clear();
            foreach (var subreddit in subreddits)
            {
                comboBoxSubereddits.Items.Add(subreddit.Name);
            }
            comboBoxSubereddits.DroppedDown = true;

            Label labelSubredditName = (Label)Controls.Find("labelSubredditName", true)[0];
            Button buttonSend = (Button)Controls.Find("buttonSend", true)[0];

            if (subreddits.Count == 0)
            {
                labelSubredditName.Text = "Жодних сабреддітів не знайдено";
                buttonSend.Enabled = false;
            }
            else
            {
                labelSubredditName.Text = "";
                buttonSend.Enabled = true;
            }
        }

        #region Input Controls

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

        private void AddSubredditName()
        {
            Label labelSubredditExample = new Label()
            {
                Location = new Point(5, 5),
                Text = "Введіть назву сабреддіту. Напр. (r/announcements)",
                AutoSize = true
            };

            var comboBoxSubreddits = new ComboBox()
            {
                Location = new Point(labelSubredditExample.Location.X, labelSubredditExample.Location.Y + labelSubredditExample.Height + 5),
                Size = new Size(300, 35),
                Name = "comboBoxSubereddits"
            };

            comboBoxSubreddits.TextChanged += loadSubreddit;

            Button searchButton = new Button()
            {
                Size = new Size(200, 50),
                Location = new Point(comboBoxSubreddits.Location.X + comboBoxSubreddits.Width + 10, labelSubredditExample.Location.Y),
                Text = "Пошук"
            };

            searchButton.Click += loadSubredditsByName;

            Label labelSubredditName = new Label()
            {
                Location = new Point(comboBoxSubreddits.Location.X, comboBoxSubreddits.Location.Y + comboBoxSubreddits.Height + 5),
                AutoSize = true,
                ForeColor = Color.Red,
                Name = "labelSubredditName"
            };

            panel2.Controls.Add(labelSubredditExample);
            panel2.Controls.Add(comboBoxSubreddits);
            panel2.Controls.Add(searchButton);
            panel2.Controls.Add(labelSubredditName);
        }

        private void AddTitle()
        {
            Label labelTitle = new Label()
            {
                Location = new Point(5, 90),
                Text = "Заголовок:",
                AutoSize = true,
                Name = "labelTitle"
            };

            TextBox textBoxTitle = new TextBox()
            {
                Name = "textBoxTitle",
                Height = 30,
                Width = panel2.Width / 2 - 10,
                Location = new Point(labelTitle.Location.X, labelTitle.Location.Y + labelTitle.Height + 5)
            };


            panel2.Controls.Add(labelTitle);
            panel2.Controls.Add(textBoxTitle);
        }

        private void AddAttributesInputs()
        {
            Label labelTitle = (Label)Controls.Find("labelTitle", true)[0];

            var checkBoxPanel = new Panel()
            {
                Size = new Size(350, 50),
                BorderStyle = BorderStyle.FixedSingle,
                Name = "checkBoxPanel"
            };
            checkBoxPanel.Location = new Point(panel2.Width - checkBoxPanel.Width - 5, labelTitle.Location.Y);

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

        private void AddFlairs()
        {
            var checkBoxPanel = (Panel)Controls.Find("checkBoxPanel", true)[0];

            var panelFlair = new Panel()
            {
                Size = checkBoxPanel.Size,
                Location = new Point(checkBoxPanel.Location.X, checkBoxPanel.Location.Y + checkBoxPanel.Height + 5),
                BorderStyle = BorderStyle.FixedSingle,
                AutoSize = true,
            };

            var labelFlair = new Label()
            {
                Text = "Виберіть флаєр",
                AutoSize = true,
                Location = new Point(5, 5)
            };

            var comboBoxFlair = new ComboBox()
            {
                Name = "comboBoxFlair",
                Size = new Size(panelFlair.Width - 10, 35),
                Location = new Point(5, labelFlair.Location.Y + labelFlair.Height + 5)
            };

            panelFlair.Controls.Add(labelFlair);
            panelFlair.Controls.Add(comboBoxFlair);

            panel2.Controls.Add(panelFlair);
        }

        #region Link

        private void AddLink()
        {
            Label labelLink = new Label()
            {
                Text = "Посилання:",
                AutoSize = true,
                Location = new Point(5, 160)
            };

            TextBox textBoxLink = new TextBox()
            {
                Size = new Size(panel2.Width / 2 - 10, 35),
                Location = new Point(labelLink.Location.X, labelLink.Location.Y + labelLink.Height + 5),
                Name = "textBoxLink"
            };

            panel2.Controls.Add(labelLink);
            panel2.Controls.Add(textBoxLink);
        }

        private void AddSendButtonLink()
        {
            Button buttonSend = new Button()
            {
                Text = "Опублікувати",
                Size = new Size(80, 45),
                Name = "buttonSend",
                Enabled = false,
            };

            buttonSend.Location = new Point(panel2.Width - buttonSend.Width - 5, panel2.Height - buttonSend.Height - 5);

            buttonSend.Click += createPost_Link;
            panel2.Controls.Add(buttonSend);
        }

        #endregion

        #region Post

        private void AddSendButtonPost()
        {
            Button buttonSend = new Button()
            {
                Text = "Опублікувати",
                Size = new Size(80, 45),
                Name = "buttonSend",
                Enabled = false,
            };

            buttonSend.Location = new Point(panel2.Width - buttonSend.Width - 5, panel2.Height - buttonSend.Height - 5);
           
            buttonSend.Click += createPost_Post;
            panel2.Controls.Add(buttonSend);
        }

        private void AddText()
        {
            Label labelText = new Label()
            {
                Location = new Point(5, 160),
                Text = "Текст (необов'язково):",
                AutoSize = true
            };

            RichTextBox richTextBoxText = new RichTextBox()
            {
                Name = "richTextBoxText",
                Height = 80,
                Width = panel2.Width / 2 - 10,
                Location = new Point(labelText.Location.X, labelText.Location.Y + labelText.Height + 5)
            };

            panel2.Controls.Add(labelText);
            panel2.Controls.Add(richTextBoxText);
        }

        #endregion

        #endregion

        //Back
        private void button1_Click(object sender, EventArgs e)
        {
            AccountInfoForm accountInfoForm = new AccountInfoForm(_redditAccount, _accountsForm);
            accountInfoForm.Show();
            Close();
        }
    }
}