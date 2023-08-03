using reddit_bor.domain.pool;
using System;
using System.Windows.Forms;

namespace reddit_bor.form.preset
{
    public partial class PoolSubredditNewCountForm : Form
    {
        private readonly PoolSubreddit _poolSubreddit;
        public int _count;

        public PoolSubredditNewCountForm(PoolSubreddit poolSubreddit)
        {
            InitializeComponent();

            _poolSubreddit = poolSubreddit;
            _count = poolSubreddit.Count;
            numericUpDown1.Value = _count;
            label2.Text = _poolSubreddit.Name;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _count = (int)numericUpDown1.Value;
            DialogResult = DialogResult.OK;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
