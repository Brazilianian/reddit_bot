using reddit_bor.domain.pool;
using reddit_bor.domain.task;
using reddit_bor.service;
using reddit_bot.domain;
using reddit_bot.domain.task;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Transactions;
using System.Windows.Forms;

namespace reddit_bot.form.task
{
    public partial class TaskManagerForm : Form
    {
        private readonly TaskService _taskService;

        private readonly RedditAccount _redditAccount;
        private readonly AccountsForm _accountsForm;

        private readonly Pool _pool;
        private readonly List<RedditPostTask> _tasks;

        public TaskManagerForm(RedditAccount redditAccount, AccountsForm accountsForm)
        {
            InitializeComponent();

            _taskService = new TaskService();
            _redditAccount = redditAccount;
            _accountsForm = accountsForm;

            _pool = new Pool();
            _tasks = _taskService.GetAllTasks();

            FillForm();
        }

        private void FillForm()
        {
            UpdateComboBoxTasks();
        }

        private void UpdateComboBoxTasks()
        {
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(_tasks.ToArray());
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
                panel.MouseEnter += setBackgroundColorSelected;
                panel.MouseLeave += setBackgroundColorDefault;

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

                var numericUpDown = new NumericUpDown()
                {
                    Value = poolTask.Count,
                    Size = new Size((panel.Width - 10) / 2 - countLabel.Width, 35),
                    Minimum = 0,
                };
                panel.Controls.Add(numericUpDown);
                numericUpDown.Location = new Point(countLabel.Location.X + countLabel.Width + 5, panel.Height / 2 - numericUpDown.Height / 2);

                panel4.Controls.Add(panel);
            }
        }

        private void setBackgroundColorDefault(object sender, EventArgs e)
        {
            ((Control)sender).BackColor = SystemColors.Control;
            Cursor = Cursors.Default;
        }

        private void setBackgroundColorSelected(object sender, EventArgs e)
        {
            ((Control)sender).BackColor = SystemColors.ActiveCaption;
            Cursor = Cursors.Hand;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AccountInfoForm accountInfoForm = new AccountInfoForm(_redditAccount, _accountsForm);
            accountInfoForm.Show();
            Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            CreateTaskForm createTaskForm = new CreateTaskForm(_redditAccount, _accountsForm);
            createTaskForm.Show();
            Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AccountInfoForm accountInfoForm = new AccountInfoForm(_redditAccount, _accountsForm);
            accountInfoForm.Show();
            Close();
        }

        //On task selected
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RedditPostTask selectedTask = _tasks
                .Where(t => t.TaskName.Equals(((Control)sender).Text))
                .FirstOrDefault();

            PoolTask poolTask = new PoolTask(selectedTask, 0);
            _pool.AddTask(poolTask);
            _tasks.Remove(selectedTask);
            UpdateComboBoxTasks();
            UpdateTasksPanel();
        }
    }
}
