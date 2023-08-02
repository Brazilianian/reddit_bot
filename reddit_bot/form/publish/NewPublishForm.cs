using Reddit.Controllers;
using reddit_bor.domain.logs;
using reddit_bor.domain.pool;
using reddit_bor.domain.task;
using reddit_bor.form.log;
using reddit_bor.form.preset;
using reddit_bor.service;
using reddit_bot;
using reddit_bot.domain;
using reddit_bot.domain.task;
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
        private PresetForm _presetForm;

        private PresetService _presetService;
        private PublishService _publishService;
        
        private Pool _pool = new Pool();

        private Subreddit _subreddit;

        private TaskPostType _taskPostType;

        private bool _isWorking = false;
        private bool _isPause = false;

        private List<Preset> _presets;

        private IntervalRange _progress;


        public NewPublishForm(RedditAccount redditAccount, AccountsForm accountsForm)
        {
            InitializeComponent();

            _redditAccount = redditAccount;
            _accountsForm = accountsForm;

            _presetService = new PresetService();

            _presets = _presetService.FindAllPresets();

            FillForm();
            ResizeForm();
        }

        public NewPublishForm(RedditAccount redditAccount, AccountsForm accountsForm, PresetForm presetForm)
            : this(redditAccount, accountsForm)
        {
            _presetForm = presetForm;
        }

        private void FillForm()
        {
            FillPostTypeComboBox();

            FillTaskDataGrid();
            FillPresetDataGrid();
        }

        private void FillPresetDataGrid()
        {
            foreach (var preset in _presets)
            {
                DataGridViewRow dataGridViewRow = new DataGridViewRow();
                dataGridViewRow.Tag = preset;

                dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell()
                {
                    Value = preset.Name
                });
                dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell()
                {
                    Value = preset.Subreddits.Count
                });
                dataGridView3.Rows.Add(dataGridViewRow);
            }
        }

        private void UpdateTaskDataGrid()
        {
            dataGridView2.Rows.Clear();
            FillTaskDataGrid();
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
                Text = "Додати шаблон",
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,   
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
                Text = "Кількість публікацій",
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
        #endregion

        private void createTask(object sender, EventArgs e)
        {
            string title = Controls.Find("textBoxTitle", true)[0].Text;

            if (string.IsNullOrEmpty(title))
            {
                MessageBox.Show("Введіть заголовок");
                return;
            }

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

        private void getLogs(Log log, bool isIncrement)
        {
            if (isIncrement)
            {
                _progress.From = ++_progress.From;
            }

            UpdateProgress(log);
        }

        private void finishThePublishing()
        {
            _isWorking = false;
            _isPause = false;
            _progress.From = 0;
            UpdateProgressControls();
            _publishService.Stop();
        }

        #region Publish Control
        private void button6_Click(object sender, EventArgs e)
        {
            if (_isWorking)
            {
                return;
            }

            if (_isPause)
            {
                _publishService.Start();
                _isPause = false;
                _isWorking = true;
                return;
            }

            if (_pool == null)
            {
                MessageBox.Show("Виберіть пул для публікації");
                return;
            }

            if (_pool._tasks.Count == 0)
            {
                MessageBox.Show("Виберіть публікації");
                return;
            }

            if (_pool._subreddits.Count == 0)
            {
                MessageBox.Show("Виберіть сабредіти");
                return;
            }

            _pool.IsRandom = checkBox1.Checked;
            _pool.Range = new IntervalRange((int)numericUpDown1.Value, (int)numericUpDown2.Value);

            _publishService = new PublishService(_pool, _redditAccount);

            _publishService.MessageReceived += getLogs;
            _publishService.ProccessFinishing += finishThePublishing;

            int count = GetMaximumOfProgress();

            _progress = new IntervalRange(0, count);
            UpdateProgressControls();

            _publishService.Start();
            _isWorking = true;
        }

        private void UpdateProgressControls()
        {
            UpdateProgressLabel();
            UpdateProgressBar();
        }

        private int GetMaximumOfProgress()
        {
            int subMax = 0;
            foreach (var subreddit in _pool._subreddits)
            {
                subMax += subreddit.Count;
            }

            int postMax = 0;
            foreach (var post in _pool._tasks)
            {
                postMax += post.Count;
            }

            return Math.Min(subMax, postMax);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (!_isWorking)
            {
                return;
            }

            _publishService.Pause();
            _isWorking = false;
            _isPause = true;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            finishThePublishing();
        }
        #endregion

        #region Progress
        private void UpdateProgress(Log log)
        {
            UpdateProgressLabel();
            UpdateProgressBar();
            if (!string.IsNullOrEmpty(log.Message))
            {
                UpdateRichTextBox(log);
            }
        }

        private void UpdateProgressBar()
        {
            if (progressBar1.InvokeRequired)
            {
                progressBar1.BeginInvoke(new Action(UpdateProgressBar));
            }
            else
            {
                try
                {
                    progressBar1.Value = (int)((double)_progress.From / (double)_progress.To * 100);
                } catch(Exception ex)
                {
                    progressBar1.Value = 0;
                }
            }
        }

        private void UpdateProgressLabel()
        {
            if (label4.InvokeRequired)
            {
                label4.BeginInvoke(new Action(UpdateProgressLabel));
            }
            else
            {
                label4.Text = $"{_progress.From} / {_progress.To}";
            }
        }

        private void UpdateRichTextBox(Log log)
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.BeginInvoke(new Action<Log>(UpdateRichTextBox), log);
            }
            else
            {
                switch (log.LogLevel)
                {
                    case LogLevel.Info:
                        richTextBox1.SelectionColor = Color.Black;
                        break;
                    case LogLevel.Warn:
                        richTextBox1.SelectionColor = Color.Orange;
                        break;
                    case LogLevel.Error:
                        richTextBox1.SelectionColor = Color.Red;
                        break;
                }
                richTextBox1.AppendText(log.ToString() + Environment.NewLine);
            }
        }
        #endregion

        #region Menu Panel
        private void button4_Click(object sender, System.EventArgs e)
        {
            AccountInfoForm accountInfoForm = new AccountInfoForm(_redditAccount, _accountsForm);
            accountInfoForm.Show();
            Close();

            _presetForm?.Close();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (_presetForm == null)
            {
                _presetForm = new PresetForm(_redditAccount, _accountsForm, this);
            }
            _presetForm.Show();
            Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _accountsForm.Show();
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            LogForm logForm = new LogForm(_accountsForm, _redditAccount);
            logForm.Show();
            Close();

            _presetForm?.Close();
        }
        #endregion

        //Add Preset
        private void dataGridView3_DoubleClick(object sender, EventArgs e)
        {
            if (((DataGridView)sender).SelectedRows.Count == 0)
            {
                return;
            }

            Preset preset = (Preset)((DataGridView)sender).SelectedRows[0].Tag;
            _presets.Remove(preset);
            _pool._subreddits.AddRange(preset.Subreddits);
            UpdateSubredditsDataGrid();
            UpdatePresetsDataGrid();
        }

        private void UpdatePresetsDataGrid()
        {
            dataGridView3.Rows.Clear();
            FillPresetDataGrid();
        }

        private void UpdateSubredditsDataGrid()
        {
            dataGridView1.Rows.Clear();
            FillSubredditsDataGrid();
        }

        private void FillSubredditsDataGrid()
        {
            foreach (PoolSubreddit subreddit in _pool._subreddits)
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
                dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell()
                {
                    Value = subreddit.Count
                });
                dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell()
                {
                    Value = subreddit.Trigger == null ? "" : subreddit.Trigger.ToString()
                });

                dataGridView1.Rows.Add(dataGridViewRow);
            }
        }

        private void NewPublishForm_SizeChanged(object sender, EventArgs e)
        {
            ResizeForm();
        }

        private void ResizeForm()
        {
            ResizeMenu();
            ResizePool();
        }

        private void ResizePool()
        {
            Size windowSize = new Size(ClientSize.Width, ClientSize.Height);
            
            //Main
            panel1.Size = new Size(windowSize.Width * 90 / 100 - 20, windowSize.Height - 20);
            panel1.Location = new Point(panel5.Location.X + panel5.Width + 5, panel1.Location.Y);
            
            //First
            panel2.Size = new Size(panel1.Width * 3 / 4 - 6, panel1.Height * 3 / 10);
            panel7.Size = new Size(panel2.Width - 6, panel2.Height - (comboBox1.Height + comboBox1.Location.Y + 10));

            panel4.Size = new Size(panel1.Width / 4 - 4, panel1.Height / 5);
            panel4.Location = new Point(panel2.Location.X + panel2.Width + 2, panel4.Location.Y);
            label5.Location = new Point(panel4.Location.X, label5.Location.Y);

            //Second
            label2.Location = new Point(label2.Location.X, panel2.Location.Y + panel2.Height + 4);
            panel3.Location = new Point(panel3.Location.X, label2.Location.Y + label2.Height + 2);
            panel3.Size = new Size(panel1.Width * 3 / 4 - 6, panel1.Height * 1 / 5);

            label7.Location = new Point(label5.Location.X, panel4.Location.Y + panel4.Height + 3);
            panel6.Location = new Point(panel4.Location.X, label7.Location.Y + label7.Height + 3);
            panel6.Size = new Size(panel1.Width / 4 - 4, panel1.Height * 3 / 10);

            //Third
            panel8.Size = new Size(panel1.Width - 10, panel1.Height / 10);
            panel8.Location = new Point(panel3.Location.X, panel3.Location.Y + panel3.Height + 4);

            label3.Location = new Point(panel8.Location.X, panel8.Location.Y + panel8.Height + 2);
            richTextBox1.Size = new Size(panel8.Width, panel1.Height * 3 / 10);
            richTextBox1.Location = new Point(label3.Location.X, label3.Location.Y + label3.Height + 2);
        }

        private void ResizeMenu()
        {
            Size windowSize = new Size(ClientSize.Width, ClientSize.Height);
            panel5.Size = new Size(windowSize.Width / 10, windowSize.Height - 20);

            button4.Size = new Size(panel5.Width - 6, 40);
            button1.Size = new Size(panel5.Width - 6, 40);
            button11.Size = new Size(panel5.Width - 6, 40);
            button2.Size = new Size(panel5.Width - 6, 40);
            button3.Size = new Size(panel5.Width - 6, 40);
        }
    }
}
