using System;
using System.Windows.Forms;

namespace reddit_bot
{
    public partial class AddAccountCredentialsForm : Form
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public AddAccountCredentialsForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AccessToken = textBox1.Text;
            RefreshToken = textBox2.Text;
        }
    }
}