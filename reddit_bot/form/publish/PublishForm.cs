using Reddit.Things;
using reddit_bor.domain.pool;
using reddit_bor.domain.task;
using reddit_bor.form.pool;
using reddit_bor.service;
using reddit_bor.util;
using reddit_bot;
using reddit_bot.domain;
using reddit_bot.domain.task;
using reddit_bot.form.task;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace reddit_bor.form.publish
{
    public partial class PublishForm : Form
    {
        private readonly AccountsForm _accountsForm;
        private readonly RedditAccount _redditAccount;
        
        private readonly PoolService _poolService;
        private PublishService _publishService;

        private List<Pool> _pools;
        private Pool _currentPool;
        private IntervalRange _progressRange = new IntervalRange();

        private bool isWorking;

        public PublishForm(RedditAccount redditAccount, AccountsForm accountsForm)
        {
            InitializeComponent();

            _redditAccount = redditAccount;
            _accountsForm = accountsForm;

            _poolService = new PoolService();

            _pools = _poolService.GetAllPools();

            FillPools();
        }

        private void FillPools()
        {
            List<Pool> notWorkedPools = ExceptCurrentPool();

            Size panelSize = new Size(175, 45);
            int countInRow = panel2.Width / panelSize.Width;

            for (int i = 0; i < notWorkedPools.Count; i++)
            {
                var pool = notWorkedPools[i];

                var panel = new Panel()
                {
                    Size = panelSize,
                    BorderStyle = BorderStyle.FixedSingle,
                    Tag = pool.Name,
                    Location = new Point(
                        5 + i % countInRow * panelSize.Width + i % countInRow * 5,
                        5 + i / countInRow * panelSize.Height + i / countInRow * 5)
                };

                panel.MouseEnter += selectControl;
                panel.MouseLeave += unselectControl;
                panel.Click += choosePool;

                var labelPoolName = new Label()
                {
                    Text = pool.Name,
                    AutoSize = true,
                };

                panel.Controls.Add(labelPoolName);
                labelPoolName.Location = new Point(panel.Width / 2 - labelPoolName.Width / 2, panel.Height / 2 - labelPoolName.Height / 2);

                panel2.Controls.Add(panel);
            }
        }

        private void FillCurrentPool()
        {
            if (_currentPool == null)
            {
                return;
            }

            Size panelSize = new Size(175, 45);
            var panel = new Panel()
            {
                Size = panelSize,
                BorderStyle = BorderStyle.FixedSingle,
                Tag = _currentPool.Name,
                Location = new Point(5, 5)
            };

            panel.MouseEnter += selectControl;
            panel.MouseLeave += unselectControl;
            panel.Click += showPoolInfo;
            
            var labelPoolName = new Label()
            {
                Text = _currentPool.Name,
                AutoSize = true,
            };

            var buttonRemove = new Button()
            {
                Size = new Size(100, 30),
                Text = "Прибрати",
                Tag = _currentPool.Name,
            };
            buttonRemove.Location = new Point(panel.Location.X + panel.Width + 10, panel.Location.X + panel.Height / 2 - buttonRemove.Height / 2);
            buttonRemove.Click += removeCurrentPool;
            panel3.Controls.Add(buttonRemove);

            panel.Controls.Add(labelPoolName);
            labelPoolName.Location = new Point(panel.Width / 2 - labelPoolName.Width / 2, panel.Height / 2 - labelPoolName.Height / 2);
            panel3.Controls.Add(panel);
        }

        private void removeCurrentPool(object sender, EventArgs e)
        {
            _currentPool = null;
            RefillCurrentPoolPanel();
            RefillPoolsPanel();
        }

        private void RefillPoolsPanel()
        {
            panel2.Controls.Clear();
            FillPools();
        }

        private void RefillCurrentPoolPanel()
        {
            panel3.Controls.Clear();
            FillCurrentPool();
        }

        private List<Pool> ExceptCurrentPool()
        {
            if (_currentPool == null)
            {
                return _pools;
            }

            return _pools.FindAll(p => !p.Name.Equals(_currentPool.Name));
        }

        private void unselectControl(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
            ControlsUtil.UnselectTask(sender, e);
        }

        private void selectControl(object sender, EventArgs e)
        {
            Cursor = Cursors.Hand;
            ControlsUtil.SelectTask(sender, e);
        }

        private void showPoolInfo(object sender, EventArgs e)
        {
            var poolName = ((Control)sender).Tag.ToString();
            var pool = _pools.Where(p => p.Name.Equals(poolName)).FirstOrDefault();
            var poolInfoForm = new PoolInfoForm(pool);
            poolInfoForm.ShowDialog();
        }

        private void choosePool(object sender, EventArgs e)
        {
            var poolName = ((Control)sender).Tag.ToString();
            _currentPool = _pools.Where(p => p.Name.Equals(poolName)).FirstOrDefault();

            int to = 0;
            foreach (var task in _currentPool._tasks)
            {
                to += task.Count;
            }
            _progressRange = new IntervalRange(0, to);

            RefillPoolsPanel();
            RefillCurrentPoolPanel();
        }

        #region Menu
        private void button4_Click(object sender, System.EventArgs e)
        {
            AccountInfoForm accountInfoForm = new AccountInfoForm(_redditAccount, _accountsForm);
            accountInfoForm.Show();
            Close();
        }

        private void button3_Click(object sender, System.EventArgs e)
        {
            PoolManagerForm poolManagerForm = new PoolManagerForm(_redditAccount, _accountsForm);
            poolManagerForm.Show();
            Close();
        }

        private void button5_Click(object sender, System.EventArgs e)
        {
            TaskManagerForm taskManagerForm = new TaskManagerForm(_redditAccount, _accountsForm);
            taskManagerForm.Show();
            Close();
        }
        #endregion

        #region Publishing Proccess Control
        
        private void button6_Click(object sender, EventArgs e)
        {
            if (isWorking)
            {
                return;
            }

            if (_currentPool == null)
            {
                MessageBox.Show("Виберіть пул");
                return;
            }

            isWorking = true;

            _publishService = new PublishService(_currentPool, _redditAccount);
            _publishService.MessageReceived += getLogs;

            _publishService.Start();
        }

        private void getLogs(string message)
        {

            _progressRange.From = ++_progressRange.From;

            UpdateProgress(message);
        }

        private void UpdateProgressLabel()
        {
            if (label4.InvokeRequired)
            {
                label4.BeginInvoke(new Action(UpdateProgressLabel));
            } else
            {
                label4.Text = $"{_progressRange.From} / {_progressRange.To}";
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
                progressBar1.Value = (int)((double)_progressRange.From / (double)_progressRange.To * 100);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (_currentPool == null)
            {
                MessageBox.Show("Виберіть пул");
                return;
            }

            isWorking = false;
            _publishService.Pause();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (_currentPool == null)
            {
                MessageBox.Show("Виберіть пул");
                return;
            }

            isWorking = false;
            _progressRange.From = 0;
            _publishService.Stop();
            UpdateProgress("");
        }

        private void UpdateProgress(string message)
        {
            UpdateProgressLabel();
            UpdateProgressBar();
            UpdateRichTextBox(message);
        }

        private void UpdateRichTextBox(string message)
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.BeginInvoke(new Action<string>(UpdateRichTextBox), message);
            }
            else
            {
                richTextBox1.Text += message + "\n";
            }
        }

        #endregion
    }
}
