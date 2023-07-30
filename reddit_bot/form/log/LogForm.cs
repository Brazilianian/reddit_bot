using reddit_bor.form.preset;
using reddit_bor.form.publish;
using reddit_bot;
using reddit_bot.domain;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace reddit_bor.form.log
{
    public partial class LogForm : Form
    {
        private readonly AccountsForm _accountsForm;
        private readonly RedditAccount _redditAccont;

        public LogForm(AccountsForm accountsForm, RedditAccount redditAccount)
        {
            InitializeComponent();

            _accountsForm = accountsForm;
            _redditAccont = redditAccount;
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
            panel1.Size = new Size(windowSize.Width - panel1.Location.X - panel2.Width - 5, panel5.Height);

            panel2.Size = new Size(panel2.Width, panel1.Height);
            panel2.Location = new Point(windowSize.Width - panel2.Width - 5);
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
    }
}
