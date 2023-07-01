using reddit_bot.domain.task;
using System;
using System.Windows.Forms;

namespace reddit_bor.form.task
{
    public partial class TaskInfoForm : Form
    {
        private readonly RedditPostTask _task;
        public TaskInfoForm(RedditPostTask task)
        {
            InitializeComponent();

            _task = task;

            FillForm();
        }

        private void FillForm()
        {
            throw new NotImplementedException();
            //label1.Text = _task.TaskName;
            //textBox1.Text = _task.SubredditName;
            //textBox2.Text = _task.Title;
            //checkBox2.Checked = _task.IsSpoiler;
            //checkBox3.Checked = _task.IsNSFW;

            //if (_task.PostFlair != null)
            //{
            //    textBox3.Text = _task.PostFlair.Text;
            //}

            //if (_task is RedditPostTaskPost) 
            //{
            //    label4.Text = "Текст";
            //    richTextBox1.Text = (_task as RedditPostTaskPost).Text;
            //    label6.Text = "Пост";
            //}
            //else if (_task is RedditPostTaskLink)
            //{
            //    label4.Text = "Посилання";
            //    richTextBox1.Text = (_task as RedditPostTaskLink).Link;
            //    label6.Text = "Посилання";
            //} else
            //{
            //    throw new NotImplementedException();
            //}
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
