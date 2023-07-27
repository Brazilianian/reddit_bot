using System;
using System.Windows.Forms;

namespace reddit_bor.form.preset
{
    public partial class PresetAddNameDialogForm : Form
    {
        public string _name;

        public PresetAddNameDialogForm()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _name = textBox1.Text;
            DialogResult = DialogResult.OK;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
