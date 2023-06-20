using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Reddit;
using Reddit.Controllers;
using Reddit.Exceptions;
using reddit_bor.domain.forms;
using reddit_bor.domain.task;
using reddit_bor.util;
using reddit_bot.domain;
using reddit_bot.service;

namespace reddit_bot.form.task
{
    public partial class CreateTaskForm : Form
    {
        private readonly RedditAccount _redditAccount;
        private readonly AccountsForm _accountsForm;
        private readonly RedditService _reddditService;

        private ComboBoxItem currentPostFlair = null;

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
                    //AddUserFlairs();
                    AddSendButtonPost();
                    break;

                case TaskPostType.LINK:
                    AddSubredditName();
                    AddTitle();
                    AddLink();
                    AddAttributesInputs();
                    AddFlairs();
                    //AddUserFlairs();
                    AddSendButtonLink();
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

            ComboBox comboBoxFlair = ((ComboBox)Controls.Find("comboBoxFlair", true)[0]);
            ComboBoxItem flairItem = comboBoxFlair.SelectedItem as ComboBoxItem;

            if (flairItem == null)
            {
                if (currentPostFlair != null)
                {
                    flairItem = currentPostFlair;
                    flairItem.Text = comboBoxFlair.Text;
                }
            }

            //string flairUser = ((ComboBox)Controls.Find("comboBoxUserFlair", true)[0]).Text;

            try
            {
                //if (string.IsNullOrEmpty(flairUser)) 
                //{
                //    _subreddit.Flairs.CreateUserFlair(_subreddit.Name, flairUser);
                //}

                SelfPost post = _subreddit
                    .SelfPost(title: title, selfText: text);

                //TODO add flair to post
                if (flairItem != null)
                {
                    post.Submit(spoiler: isSpoiler, flairText: flairItem.Text, flairId: flairItem.Tag);
                } else
                {
                    post.Submit(spoiler: isSpoiler);
                }

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
            string flair = ((ComboBox)Controls.Find("comboBoxFlair", true)[0]).Text;
            //string flairUser = ((ComboBox)Controls.Find("comboBoxUserFlair", true)[0]).Text;

            try
            {
                //if (string.IsNullOrEmpty(flairUser))
                //{
                //    _subreddit.Flairs.CreateUserFlair(_redditClient.Account.Me.Name, flairUser);
                //}

                LinkPost post = _subreddit
                    .LinkPost(title: title, url: link);

                try
                {
                    if (!string.IsNullOrEmpty(flair))
                    {
                        post.Submit(spoiler: isSpoiler)
                        .SetFlair(flair);
                    } else
                    {
                        post.Submit(spoiler: isSpoiler);
                    }

                    if (isNsfw)
                    {
                        post.MarkNSFWAsync();
                    }
                    MessageBox.Show("Відправлено");
                } catch (RedditAlreadySubmittedException ex)
                {
                    MessageBox.Show(ex.Message);
                } 
            }
            catch (RedditControllerException exception)
            {
                MessageBox.Show(exception.Message);
            }

            //TODO OC tag
        }

        #region Load Subreddit/Flairs

        private void loadSubreddit(object sender, EventArgs e)
        {
            var comboBoxSubereddits = (ComboBox)Controls.Find("comboBoxSubereddits", true)[0];
            _subreddit = _redditClient.Subreddit(comboBoxSubereddits.Text);
        }

        private void loadSubredditsByName(object sender, EventArgs e)
        {
            var comboBoxSubereddits = (ComboBox)Controls.Find("comboBoxSubereddits", true)[0];

            Label labelSubredditName = (Label)Controls.Find("labelSubredditName", true)[0];
            Button buttonSend = (Button)Controls.Find("buttonSend", true)[0];

            if (string.IsNullOrEmpty(comboBoxSubereddits.Text))
            {
                labelSubredditName.Text = "Жодних сабреддітів не знайдено";
                buttonSend.Enabled = false;
                return;
            }

            List<Subreddit> subreddits = _redditClient.SearchSubreddits(comboBoxSubereddits.Text, limit: 10);
            comboBoxSubereddits.Items.Clear();
            foreach (var subreddit in subreddits)
            {
                comboBoxSubereddits.Items.Add(subreddit.Name);
            }
            comboBoxSubereddits.DroppedDown = true;

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

        private void loadUserFlair(object sender, EventArgs e)
        {
            if (_subreddit == null)
            {
                return;
            }

            var flairText = ((ComboBox)sender).Text;

            ComboBox comboBoxUserFlair = (ComboBox)Controls.Find("comboBoxUserFlair", true)[0];

            Reddit.Things.FlairV2 flair = _subreddit.Flairs.UserFlairV2
                .Where(f => f.Text.Equals(flairText))
                .FirstOrDefault();

            if (flair == null)
            {
                MessageBox.Show("Flair is null");
                return;
            }
            if (flair.TextEditable)
            {
                comboBoxUserFlair.DropDownStyle = ComboBoxStyle.DropDown;
            }
            else
            {
                comboBoxUserFlair.DropDownStyle = ComboBoxStyle.DropDownList;
            }
        }

        private void loadUserFlairs(object sender, EventArgs e)
        {
            if (_subreddit == null)
            {
                return;
            }

            Flairs flairs = _subreddit.Flairs;

            Label labelUserFlair = (Label)Controls.Find("labelUserFlair", true)[0];
            ComboBox comboBoxUserFlair = (ComboBox)Controls.Find("comboBoxUserFlair", true)[0];

            comboBoxUserFlair.Items.Clear();

            try
            {
                labelUserFlair.Text = "";
                comboBoxUserFlair.DropDownStyle = ComboBoxStyle.DropDownList;

                List<Reddit.Things.FlairV2> flairList = flairs.UserFlairV2;
                foreach (var flair in flairList)
                {
                    comboBoxUserFlair.Items.Add(flair.Text);
                }
            }
            catch (Exception ex)
            {
                labelUserFlair.Text = "Флаєри не знайдено";
            }
        }

        // Fill flairs combobox 
        // If there is no items, block it
        // Calls on button to load flairs
        private void loadFlairs(object sender, EventArgs e)
        {
            if (_subreddit == null)
            {
                return;
            }

            Flairs flairs = _subreddit.Flairs;
            Label labelFlair = (Label)Controls.Find("labelFlair", true)[0];
            ComboBox comboBoxFlair = (ComboBox)Controls.Find("comboBoxFlair", true)[0];
          
            comboBoxFlair.Items.Clear();

            try
            {
                labelFlair.Text = "";
                comboBoxFlair.DropDownStyle = ComboBoxStyle.DropDownList;
                
                List<Reddit.Things.FlairV2> flairList = flairs.LinkFlairV2;
                foreach (var flair in flairList)
                {
                    comboBoxFlair.Items.Add(new ComboBoxItem(flair.Text, flair.Id));
                }
            } catch(Exception ex)
            {
                labelFlair.Text = "Флаєри не знайдено";
            }
        }

        // Get flair of subreddit by it name
        // Block combobox from editing depending on Subreddit settings
        // Calls on combobox selected index changing
        private void loadFlair(object sender, EventArgs e)
        {
            if (_subreddit == null)
            {
                return;
            }

            var flairText = ((ComboBox)sender).Text;

            ComboBox comboBoxFlair = (ComboBox)Controls.Find("comboBoxFlair", true)[0];

            Reddit.Things.FlairV2 flair = _subreddit.Flairs.GetLinkFlairV2()
                .Where(f => f.Text.Equals(flairText))
                .FirstOrDefault();
            
            if (flair == null)
            {
                MessageBox.Show("Flair is null");
                return;
            }
            if (flair.TextEditable)
            {
                currentPostFlair = new ComboBoxItem(flair.Text, flair.Id);
                comboBoxFlair.DropDownStyle = ComboBoxStyle.DropDown;
            }
            else
            {
                comboBoxFlair.DropDownStyle = ComboBoxStyle.DropDownList;
            }
        }

        #endregion

        #region Input Controls

        private void FillTaskCreationPanel(TaskType postType)
        {
            Label label = new Label();
            label.Text = "Виберіть тип посту";
            label.AutoSize = true;
            label.Location = new Point(5, 5);
            panel4.Controls.Add(label);

            ComboBox comboBox = new ComboBox();
         
            comboBox.Location = new Point(label.Location.X, label.Location.Y + label.Height + 5);
            comboBox.Width = panel4.Width - 10;
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.SelectedIndexChanged += fillInputPanel;
            comboBox.DataSource = Enum.GetValues(typeof(TaskPostType))
                            .Cast<TaskPostType>()
                            .Select(e => EnumUtil.GetDescription(e))
                            .ToList();
            panel4.Controls.Add(comboBox);
            comboBox.SelectedIndex = -1;
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
            comboBoxSubreddits.SelectedIndexChanged += loadFlairs;
            //comboBoxSubreddits.SelectedIndexChanged += loadUserFlairs;

            Button searchButton = new Button()
            {
                Size = new Size(200, 50),
                Location = new Point(comboBoxSubreddits.Location.X + comboBoxSubreddits.Width + 10, labelSubredditExample.Location.Y),
                Text = "Пошук"
            };

            searchButton.Click += loadSubredditsByName;
            searchButton.Click += loadFlairs;
            //searchButton.Click += loadUserFlairs;

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

            var panelFlairPost = new Panel()
            {
                Size = checkBoxPanel.Size,
                Location = new Point(checkBoxPanel.Location.X, checkBoxPanel.Location.Y + checkBoxPanel.Height + 5),
                BorderStyle = BorderStyle.FixedSingle,
                AutoSize = true,
                Name = "panelFlairPost"
            };

            var labelFlairTitle = new Label()
            {
                Text = "Виберіть флаєр для посту",
                AutoSize = true,
                Location = new Point(5, 5)
            };

            var comboBoxFlair = new ComboBox()
            {
                Name = "comboBoxFlair",
                Size = new Size(panelFlairPost.Width - 10, 35),
                Location = new Point(5, labelFlairTitle.Location.Y + labelFlairTitle.Height + 5),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            var labelFlair = new Label()
            {
                Name = "labelFlair",
                AutoSize = true,
                Location = new Point(comboBoxFlair.Location.X, comboBoxFlair.Location.Y + comboBoxFlair.Height + 2),
                ForeColor = Color.Red
            };

            var buttonFlairSearch = new Button()
            {
                Name = "buttonFlairSearch",
                AutoSize = true,
                Text = "Підгрузити"
            };
            buttonFlairSearch.Location = new Point(panelFlairPost.Width - buttonFlairSearch.Width - 5, labelFlairTitle.Location.Y);

            buttonFlairSearch.Click += loadFlairs;

            comboBoxFlair.SelectedIndexChanged += loadFlair;

            panelFlairPost.Controls.Add(labelFlairTitle);
            panelFlairPost.Controls.Add(comboBoxFlair);
            panelFlairPost.Controls.Add(labelFlair);
            panelFlairPost.Controls.Add(buttonFlairSearch);

            panel2.Controls.Add(panelFlairPost);
        }

        private void AddUserFlairs()
        {
            var panelFlairPost = (Panel)Controls.Find("panelFlairPost", true)[0];

            var panelUserFlair = new Panel()
            {
                Size = panelFlairPost.Size,
                Location = new Point(panelFlairPost.Location.X, panelFlairPost.Location.Y + panelFlairPost.Height + 5),
                BorderStyle = BorderStyle.FixedSingle,
                AutoSize = true,
            };

            var labelFlairTitle = new Label()
            {
                Text = "Виберіть флаєр для користувача",
                AutoSize = true,
                Location = new Point(5, 5)
            };

            var comboBoxUserFlair = new ComboBox()
            {
                Name = "comboBoxUserFlair",
                Size = new Size(panelUserFlair.Width - 10, 35),
                Location = new Point(5, labelFlairTitle.Location.Y + labelFlairTitle.Height + 5),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            var labelUserFlair = new Label()
            {
                Name = "labelUserFlair",
                AutoSize = true,
                Location = new Point(comboBoxUserFlair.Location.X, comboBoxUserFlair.Location.Y + comboBoxUserFlair.Height + 2),
                ForeColor = Color.Red
            };

            var buttonFlairUserSearch = new Button()
            {
                Name = "buttonFlairUserSearch",
                AutoSize = true,
                Text = "Підгрузити"
            };

            buttonFlairUserSearch.Location = new Point(panelUserFlair.Width - buttonFlairUserSearch.Width - 5, labelFlairTitle.Location.Y);

            buttonFlairUserSearch.Click += loadUserFlairs;

            comboBoxUserFlair.SelectedIndexChanged += loadUserFlair;

            panelUserFlair.Controls.Add(labelFlairTitle);
            panelUserFlair.Controls.Add(comboBoxUserFlair);
            panelUserFlair.Controls.Add(labelUserFlair);
            panelUserFlair.Controls.Add(buttonFlairUserSearch);

            panel2.Controls.Add(panelUserFlair);
        }

        private void FillForm()
        {
            comboBox1.SelectedIndexChanged -= comboBox1_SelectedIndexChanged;
            comboBox1.DataSource = Enum.GetValues(typeof(TaskType))
                              .Cast<TaskType>()
                              .Select(e => e.GetDescription())
                              .ToList();
            comboBox1.SelectedIndex = -1;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
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