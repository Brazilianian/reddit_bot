using reddit_bor.domain.logs;
using reddit_bor.form.preset;
using reddit_bor.form.publish;
using reddit_bor.service;
using reddit_bot;
using reddit_bot.domain;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace reddit_bor.form.log
{
    public partial class LogForm : Form
    {
        private readonly AccountsForm _accountsForm;
        private readonly RedditAccount _redditAccont;
        private readonly LogService _logService;

        private List<Log> _logs;

        public LogForm(AccountsForm accountsForm, RedditAccount redditAccount)
        {
            _accountsForm = accountsForm;
            _redditAccont = redditAccount;

            _logService = new LogService();

            _logs = _logService.FindAllLogs();

            InitializeComponent();

            SetDefaultValuesToFilters();
        }

        private void SetDefaultValuesToFilters()
        {
            DateTime earliest = GetEarliest();
            DateTime latest = GetLatest();

            dateTimePicker1.Value = earliest;
            dateTimePicker2.Value = latest;

            UpdateMinAndMaxDateTimePickers();
        }

        private DateTime GetLatest()
        {
            return _logs.Max(log => log.DateTime);
        }

        private DateTime GetEarliest()
        {
            return _logs.Min(log => log.DateTime);
        }

        private void UpdateMinAndMaxDateTimePickers()
        {
            dateTimePicker1.MinDate = GetEarliest();
            dateTimePicker1.MaxDate = dateTimePicker2.Value;

            dateTimePicker2.MinDate = dateTimePicker1.Value;
            dateTimePicker2.MaxDate = GetLatest();
        }

        private void FillLogs(List<Log> logs)
        {
            foreach (Log log in logs)
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

        private void FilterLogs()
        {
            bool isInfoPresent = checkBox1.Checked;
            bool isWarnPresent = checkBox2.Checked;
            bool isErrorPresent = checkBox3.Checked;

            DateTime from = dateTimePicker1.Value;
            DateTime to = dateTimePicker2.Value;

            List<Log> filteredLogs = _logs.Where(log =>
            ((isInfoPresent && log.LogLevel == LogLevel.Info) || 
            (isWarnPresent && log.LogLevel == LogLevel.Warn) || 
            (isErrorPresent && log.LogLevel == LogLevel.Error)) && 
            log.DateTime >= from && log.DateTime <= to
            ).ToList();

            UpdateLogs(filteredLogs);
        }

        private void UpdateLogs(List<Log> filteredLogs)
        {
            richTextBox1.Clear();
            FillLogs(filteredLogs);
        }

        #region Menu Panel
        private void button2_Click(object sender, System.EventArgs e)
        {
            _accountsForm.Show();
        }

        private void button4_Click(object sender, System.EventArgs e)
        {
            AccountInfoForm accountInfoForm = new AccountInfoForm(_redditAccont, _accountsForm);
            accountInfoForm.Show();
            Close();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            NewPublishForm newPublishForm = new NewPublishForm(_redditAccont, _accountsForm);
            newPublishForm.Show();
            Close();
        }

        private void button11_Click(object sender, System.EventArgs e)
        {
            PresetForm presetForm = new PresetForm(_redditAccont, _accountsForm);
            presetForm.Show();
            Close();
        }
        #endregion

        #region Resize
        private void LogForm_SizeChanged(object sender, System.EventArgs e)
        {
            ResizeForm();
        }

        private void ResizeForm()
        {
            ResizeMenu();
            ResizeControls();
        }

        private void ResizeControls()
        {
            Size windowSize = new Size(ClientSize.Width, ClientSize.Height);
            panel1.Location = new Point(panel5.Location.X + panel5.Width + 5, panel1.Location.Y);
            panel1.Size = new Size(windowSize.Width - panel1.Location.X - panel2.Width - 5, panel5.Height);

            panel2.Location = new Point(panel1.Location.X + panel1.Width + 2, panel1.Location.Y);
        }

        private void ResizeMenu()
        {
            Size windowSize = new Size(ClientSize.Width, ClientSize.Height);
            panel5.Size = new Size(windowSize.Width / 10, windowSize.Height - 22);

            button4.Size = new Size(panel5.Width - 6, 40);
            button1.Size = new Size(panel5.Width - 6, 40);
            button11.Size = new Size(panel5.Width - 6, 40);
            button2.Size = new Size(panel5.Width - 6, 40);
            button3.Size = new Size(panel5.Width - 6, 40);
        }
        #endregion

        #region Filter Option On Value Change Methods
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            FilterLogs();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            FilterLogs();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            FilterLogs();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            UpdateMinAndMaxDateTimePickers();

            FilterLogs();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            UpdateMinAndMaxDateTimePickers();

            FilterLogs();
        }
        #endregion
    }
}
