using reddit_bor.domain.pool;
using reddit_bor.domain.task;
using reddit_bor.form.pool;
using reddit_bor.form.publish;
using reddit_bor.service;
using reddit_bor.util;
using reddit_bot.domain;
using reddit_bot.domain.task;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace reddit_bot.form.task
{
    public partial class PoolManagerForm : Form
    {
        private readonly TaskService _taskService;

        private readonly RedditAccount _redditAccount;
        private readonly AccountsForm _accountsForm;
        private readonly PoolService _poolService;

        private Pool _pool = new Pool();
        private readonly List<RedditPostTask> _tasks;
        private readonly List<Pool> _existedPools;

        public PoolManagerForm(RedditAccount redditAccount, AccountsForm accountsForm)
        {
            InitializeComponent();

            _taskService = new TaskService();
            _poolService = new PoolService();

            _redditAccount = redditAccount;
            _accountsForm = accountsForm;

            _tasks = _taskService.GetAllTasks();
            _existedPools = _poolService.GetAllPools();

            FillForm();
        }

        private void FillForm()
        {
            FillPools();
            UpdateComboBoxTasks();
        }

        private void FillPools()
        {
            Size panelSize = new Size(175, 45);
            int countInRow = panel2.Width / panelSize.Width;

            for (int i = 0; i < _existedPools.Count; i++)
            {
                var pool = _existedPools[i];

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
                panel.Click += showPoolInfo;

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

        private void RefillPoolsPanel()
        {
            panel2.Controls.Clear();
            FillPools();
        }

        private void ClearControls()
        {
            textBox1.Text = "";
            checkBox1.Checked = false;
            numericUpDown1.Value = 0;
            numericUpDown2.Value = 0;
            panel4.Controls.Clear();
        }

        private void UpdateComboBoxTasks()
        {
            comboBox1.Items.Clear();

            comboBox1.Items.AddRange(_tasks.Where(t => !_pool._tasks.Any(t2 => t2.PostTask.TaskName.Equals(t.TaskName))).ToArray());
               
        }

        private void UpdateTasksPanel()
        {
            panel4.Controls.Clear();
            for (int i = 0; i < _pool._tasks.Count; i++)
            {
                var poolTask = _pool._tasks[i];

                var panel = new Panel()
                {
                    Size = new Size(panel4.Width - 10, 50),
                    BorderStyle = BorderStyle.FixedSingle,
                    Tag = poolTask.PostTask.TaskName
                };
                panel.MouseEnter += selectControl;
                panel.MouseLeave += unselectControl;
                panel.Click += showPoolInfo;

                panel.Location = new Point(5, i * panel.Height + i * 5 + 5);

                var labelTaskName = new Label()
                {
                    AutoSize = true,
                    MaximumSize = new Size((panel.Width - 10) / 2, 0),
                    Text = $"{i + 1}) {poolTask.PostTask.TaskName}",
                };
                
                panel.Controls.Add(labelTaskName);
                labelTaskName.Location = new Point(5, panel.Height / 2 - labelTaskName.Height / 2);

                var countLabel = new Label()
                {
                    AutoSize = true,
                    Text = "Кількість постів:"
                };

                panel.Controls.Add(countLabel);
                countLabel.Location = new Point((panel.Width - 10) / 2 + 5, panel.Height / 2 - countLabel.Height / 2);

                var numericUpDownCount = new NumericUpDown()
                {
                    Tag = poolTask.PostTask.TaskName,
                    Value = poolTask.Count,
                    Size = new Size((panel.Width - 10) / 2 - countLabel.Width, 35),
                    Minimum = 0,
                };
                panel.Controls.Add(numericUpDownCount);
                numericUpDownCount.Location = new Point(countLabel.Location.X + countLabel.Width + 5, panel.Height / 2 - numericUpDownCount.Height / 2);

                numericUpDownCount.ValueChanged += changePoolTaskCount;

                panel4.Controls.Add(panel);
            }
        }

        private void changePoolTaskCount(object sender, EventArgs e)
        {
            var numericUpDownCount = (NumericUpDown)sender;
            var poolTaskName = numericUpDownCount.Tag.ToString();
            var poolTask = _pool._tasks
                .Where(t => t.PostTask.TaskName.Equals(poolTaskName))
                .FirstOrDefault();
            poolTask.Count = (int)numericUpDownCount.Value;
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
            var pool = _existedPools.Where(p => p.Name.Equals(poolName)).FirstOrDefault();
            var poolInfoForm = new PoolInfoForm(pool);
            poolInfoForm.ShowDialog();
        }

        #region Menu Panel
        private void button2_Click(object sender, EventArgs e)
        {
            AccountInfoForm accountInfoForm = new AccountInfoForm(_redditAccount, _accountsForm);
            accountInfoForm.Show();
            Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            TaskManagerForm createTaskForm = new TaskManagerForm(_redditAccount, _accountsForm);
            createTaskForm.Show();
            Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AccountInfoForm accountInfoForm = new AccountInfoForm(_redditAccount, _accountsForm);
            accountInfoForm.Show();
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PublishForm publishForm = new PublishForm(_redditAccount, _accountsForm);
            publishForm.Show();
            Close();
        }
        #endregion

        //On task selected
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RedditPostTask selectedTask = _tasks
                .Where(t => t.TaskName.Equals(((Control)sender).Text))
                .FirstOrDefault();

            PoolTask poolTask = new PoolTask(selectedTask, 0);
            _pool._tasks.Add(poolTask);
            UpdateComboBoxTasks();
            UpdateTasksPanel();
        }

        //Save pool
        private void button6_Click(object sender, EventArgs e)
        {
            _pool.Name = textBox1.Text;
            _pool.IsRandom = checkBox1.Checked;
            int from = (int)numericUpDown1.Value;
            int to = (int)numericUpDown2.Value;

            _pool.Range = new IntervalRange(from, to);

            var savedPool = _poolService.SavePool(_pool);
            _existedPools.Add(savedPool);
            _pool = new Pool();
            ClearControls();
            RefillPoolsPanel();
            UpdateComboBoxTasks();
        }
    }
}
