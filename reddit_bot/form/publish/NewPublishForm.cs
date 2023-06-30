using Reddit;
using Reddit.Controllers;
using Reddit.Models;
using reddit_bor.domain.pool;
using reddit_bor.domain.task;
using reddit_bor.service;
using reddit_bot;
using reddit_bot.domain;
using reddit_bot.domain.forms;
using reddit_bot.domain.task;
using reddit_bot.service;
using reddit_bot.util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace reddit_bor.form.publish
{
    public partial class NewPublishForm : Form
    {
        private readonly RedditAccount _redditAccount;
        private readonly AccountsForm _accountsForm;
        private readonly RedditService _redditService;
        private readonly PoolService _poolService;

        private readonly RedditClient _redditClient;

        private Pool _pool = new Pool();

        private Subreddit _subreddit;

        private PoolSubreddit _poolSubreddit;
        private TaskPostType _taskPostType;

        private readonly List<Pool> _existedPools;

        public NewPublishForm(RedditAccount redditAccount, AccountsForm accountsForm)
        {
            InitializeComponent();
            
            _redditService = new RedditService();
            _poolService = new PoolService();

            _redditAccount = redditAccount;
            _accountsForm = accountsForm;

            _redditClient = _redditService.GetRedditClient(redditAccount, RequestsUtil.GetUserAgent());
            _existedPools = _poolService.GetAllPools();
            _poolSubreddit = new PoolSubreddit();

            FillForm();
        }

        private void UpdateSubredditDataGrid()
        {
            dataGridView1.Rows.Clear();
            FillSubredditDataGrid();
        }

        private void UpdateTaskDataGrid()
        {
            dataGridView2.Rows.Clear();
            FillTaskDataGrid();
        }

        private void FillForm()
        {
            FillPostTypeComboBox();
            FillSubredditPanel();

            FillTaskDataGrid();
            FillSubredditDataGrid();
        }

        private void FillSubredditPanel()
        {
            FillSubredditComboBox();
            FillSubredditFlairComboBox();
        }

        private void FillSubredditFlairComboBox()
        {
            comboBox3.SelectedIndexChanged += loadFlair;
            comboBox3.TextChanged += changeFlairName;
        }

        private void FillSubredditComboBox()
        {
            comboBox2.SelectedIndexChanged += loadFlairs;
            comboBox2.TextChanged += loadSubreddit;
            comboBox2.Click += loadPresets;

            button10.Click += LoadSubredditsByName;
            button10.Click += loadFlairs;
        }

        private void FillPostTypeComboBox()
        {
            comboBox1.DataSource = Enum.GetValues(typeof(TaskPostType))
                            .Cast<TaskPostType>()
                            .Select(e => EnumUtil.GetDescription(e))
                            .ToList();
            comboBox1.SelectedIndex = -1;
            comboBox1.SelectedIndexChanged += fillInputPanel;
        }

        private void fillInputPanel(object sender, EventArgs e)
        {
            _taskPostType = (TaskPostType)((ComboBox)sender).SelectedIndex;
            panel7.Controls.Clear();

            switch (_taskPostType)
            {
                case TaskPostType.POST:
                    AddTitle();
                    AddText();
                    AddAttributesInputs();
                    AddCount();
                    AddCreateTaskButton();
                    break;

                case TaskPostType.LINK:
                    AddTitle();
                    AddLink();
                    AddAttributesInputs();
                    AddCount();
                    AddCreateTaskButton();
                    break;
            }
        }

        #region Inputs
        private void AddLink()
        {
            var textBox = (TextBox)Controls.Find("textBoxTitle", true)[0];
            Label labelLink = new Label()
            {
                Text = "Посилання:",
                AutoSize = true,
                Location = new Point(5, textBox.Location.Y + textBox.Height + 5),
            };
            panel7.Controls.Add(labelLink);

            TextBox textBoxLink = new TextBox()
            {
                Size = new Size(panel2.Width / 2 - 10, 35),
                Location = new Point(labelLink.Location.X, labelLink.Location.Y + labelLink.Height),
                Name = "textBoxLink"
            };

            panel7.Controls.Add(textBoxLink);
        }

        private void AddCreateTaskButton()
        {
            Button createTaskButton = new Button()
            {
                Size = new Size(75, 35),
                Text = "Додати шаблон"
            };
            createTaskButton.Location = new Point(panel7.Width - createTaskButton.Width - 5, panel7.Height - createTaskButton.Height - 5);

            createTaskButton.Click += createTask;

            panel7.Controls.Add(createTaskButton);
        }

        private void AddAttributesInputs()
        {
            Label labelTitle = (Label)Controls.Find("labelTitle", true)[0];

            var checkBoxPanel = new Panel()
            {
                Size = new Size(panel7.Width / 2 - 10, 50),
                BorderStyle = BorderStyle.FixedSingle,
                Name = "checkBoxPanel"
            };
            checkBoxPanel.Location = new Point(panel7.Width / 2 + 5, labelTitle.Location.Y);

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

            panel7.Controls.Add(checkBoxPanel);
        }

        private void AddText()
        {
            var textBox = (TextBox)Controls.Find("textBoxTitle", true)[0];
            Label labelText = new Label()
            {
                Location = new Point(5, textBox.Location.Y + textBox.Height + 5),
                Text = "Текст (необов'язково):",
                AutoSize = true
            };
            panel7.Controls.Add(labelText);

            RichTextBox richTextBoxText = new RichTextBox()
            {
                Name = "richTextBoxText",
                Height = 80,
                Width = panel2.Width / 2 - 10,
                Location = new Point(labelText.Location.X, labelText.Location.Y + labelText.Height)
            };

            panel7.Controls.Add(richTextBoxText);
        }

        private void AddTitle()
        {
            Label labelTitle = new Label()
            {
                Location = new Point(5, 5),
                Text = "Заголовок:",
                AutoSize = true,
                Name = "labelTitle"
            };
            panel7.Controls.Add(labelTitle);

            TextBox textBoxTitle = new TextBox()
            {
                Name = "textBoxTitle",
                Height = 30,
                Width = panel2.Width / 2 - 10,
                Location = new Point(labelTitle.Location.X, labelTitle.Location.Y + labelTitle.Height)
            };


            panel7.Controls.Add(textBoxTitle);
        }

        private void AddCount()
        {
            var checkBoxPanel = (Panel)Controls.Find("checkBoxPanel", true)[0];

            var labelCount = new Label()
            {
                Location = new Point(checkBoxPanel.Location.X, checkBoxPanel.Location.Y + checkBoxPanel.Height + 5),
                Text = "Кількість опублікованих разів",
                AutoSize = true
            };
            panel7.Controls.Add(labelCount);

            var numericCount = new NumericUpDown()
            {
                Name = "numericCount",
                Location = new Point(labelCount.Location.X, labelCount.Location.Y + labelCount.Height),
                Size = new Size(panel7.Width / 2 - 10, 35),
                Value = 1,
                Minimum = 1,
                Maximum = 9999,
            };
            panel7.Controls.Add(numericCount);
        }
        #endregion

        #region Menu Panel
        private void button4_Click(object sender, System.EventArgs e)
        {
            AccountInfoForm accountInfoForm = new AccountInfoForm(_redditAccount, _accountsForm);
            accountInfoForm.Show();
            Close();
        }
        #endregion

        #region Subrddits Panel
        private void loadPresets(object sender, EventArgs e)
        {
            List<PoolSubreddit> existedSubrddits = _existedPools.SelectMany(p => p._subreddits).ToList();

            if (string.IsNullOrWhiteSpace(comboBox2.Text))
            {
                comboBox2.Items.Clear();
                foreach (var subreddit in existedSubrddits)
                {
                    if (!comboBox2.Items.Contains(subreddit))
                    {
                        comboBox2.Items.Add(subreddit);
                    }
                }
            }
        }

        private void changeFlairName(object sender, EventArgs e)
        {
            if (_poolSubreddit.PostFlair != null)
            {
                _poolSubreddit.PostFlair.Text = ((Control)sender).Text;
                return;
            }

            _poolSubreddit.PostFlair = new PostFlair(((Control)sender).Text);
        }

        private void loadFlair(object sender, EventArgs e)
        {
            if (_subreddit == null)
            {
                return;
            }

            var flairText = ((ComboBox)sender).Text;

            Reddit.Things.FlairV2 flair = _subreddit.Flairs.GetLinkFlairV2()
                .Where(f => f.Text.Equals(flairText))
                .FirstOrDefault();

            if (flair == null)
            {
                _poolSubreddit.PostFlair = null;
                MessageBox.Show("Flair is null");
                return;
            }
            if (flair.TextEditable)
            {
                _poolSubreddit.PostFlair = new PostFlair(flair.Text, flair.Id);
                comboBox3.DropDownStyle = ComboBoxStyle.DropDown;
            }
            else
            {
                _poolSubreddit.PostFlair = new PostFlair(flair.Text, flair.Id);
                comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;
            }
        }

        private void loadSubreddit(object sender, EventArgs e)
        {
            _subreddit = _redditClient.Subreddit(comboBox2.Text);
        }

        private void loadFlairs(object sender, EventArgs e)
        {
            if (_subreddit == null)
            {
                return;
            }

            Flairs flairs = _subreddit.Flairs;

            comboBox3.Items.Clear();

            try
            {
                label11.Text = "";
                comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;

                List<Reddit.Things.FlairV2> flairList = flairs.LinkFlairV2;
                foreach (var flair in flairList)
                {
                    comboBox3.Items.Add(new ComboBoxItem(flair.Text, flair.Id));
                }

                if (comboBox3.Items.Count == 0)
                {
                    label11.Text = "Для даного сабредіту флаєри не знайдено";
                } else
                {
                    label11.Text = "";
                }
            }
            catch (Exception ex)
            {
                label11.Text = "Флаєри не знайдено";
            }
        }

        private void LoadSubredditsByName(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(comboBox2.Text))
            {
                label10.Text = "Жодних сабреддітів не знайдено";
                return;
            }

            List<Subreddit> subreddits = _redditClient.SearchSubreddits(comboBox2.Text, limit: 10);
            comboBox2.Items.Clear();
            foreach (var subreddit in subreddits)
            {
                comboBox2.Items.Add(subreddit.Name);
            }
            comboBox2.DroppedDown = true;

            if (subreddits.Count == 0)
            {
                label10.Text = "Жодних сабреддітів не знайдено";
            }
            else
            {
                label10.Text = "";
            }
        }
        #endregion

        #region DataGrid

        private void FillTaskDataGrid()
        {
            foreach (var task in _pool._tasks)
            {
                DataGridViewRow dataGridViewRow = new DataGridViewRow();
                dataGridViewRow.Tag = task;
                
                dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell()
                {
                   Value = task.PostTask.Title
                });

                if (task.PostTask is RedditPostTaskPost)
                {
                    dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell()
                    {
                        Value = ((RedditPostTaskPost)task.PostTask).Text
                    });
                }
                else if (task.PostTask is RedditPostTaskLink)
                {
                    dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell()
                    {
                        Value = ((RedditPostTaskLink)task.PostTask).Link
                    });
                }
                else
                {
                    throw new NotImplementedException();
                }

                dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell()
                {
                    Value = false
                });
                dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell()
                {
                    Value = task.PostTask.IsNSFW
                });
                dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell()
                {
                    Value = task.PostTask.IsSpoiler
                });
                dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell()
                {
                    Value = task.Count
                });

                dataGridView2.Rows.Add(dataGridViewRow);
            }
        }

        private void FillSubredditDataGrid()
        {
            foreach (var subreddit in _pool._subreddits)
            {
                DataGridViewRow dataGridViewRow = new DataGridViewRow();
                dataGridViewRow.Tag = subreddit;

                dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell()
                {
                    Value = subreddit.Name
                });
                dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell()
                {
                    Value = subreddit.PostFlair == null ? "" : subreddit.PostFlair.Text
                });
                dataGridView1.Rows.Add(dataGridViewRow);
            }
        }
        #endregion

        //Add subreddit and flair
        private void button9_Click(object sender, EventArgs e)
        {
            _poolSubreddit.Name = comboBox2.Text;
            if (_pool._subreddits.Contains(_poolSubreddit))
            {
                MessageBox.Show("Такий сабредіт з флаєром вже добавлений до списку");
                return;
            }
            _pool._subreddits.Add(_poolSubreddit);

            UpdateSubredditDataGrid();
            UpdateSubredditPanel();
        }

        private void UpdateSubredditPanel()
        {
            _poolSubreddit = new PoolSubreddit();
            comboBox2.Items.Clear();
            comboBox2.Text = "";

            comboBox3.Items.Clear();
            comboBox3.Text = "";
            comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;

            label10.Text = "";
            label11.Text = "";
        }

        private void createTask(object sender, EventArgs e)
        {
            string title = Controls.Find("textBoxTitle", true)[0].Text;
            bool isSpoiler = ((CheckBox)Controls.Find("checkBoxSpoiler", true)[0]).Checked;
            bool isNsfw = ((CheckBox)Controls.Find("checkBoxNsfw", true)[0]).Checked;
            int count = (int)((NumericUpDown)Controls.Find("numericCount", true)[0]).Value;

            _pool.IsRandom = checkBox1.Checked;
            _pool.Range = new IntervalRange((int)numericUpDown1.Value, (int)numericUpDown2.Value);

            switch (_taskPostType)
            {
                case TaskPostType.POST:
                    string text = Controls.Find("richTextBoxText", true)[0].Text;

                    var taskPost = new RedditPostTaskPost(title, isSpoiler, isNsfw, text);
                    var poolTaskPost = new PoolTask(taskPost, count);

                    _pool._tasks.Add(poolTaskPost);

                    UpdateTaskDataGrid();
                    break;
                case TaskPostType.LINK:
                    string link = Controls.Find("textBoxLink", true)[0].Text;

                    var taskLink = new RedditPostTaskLink(title, isSpoiler, isNsfw, link);
                    var poolTaskLink = new PoolTask(taskLink, count);
                    _pool._tasks.Add(poolTaskLink);

                    UpdateTaskDataGrid();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
