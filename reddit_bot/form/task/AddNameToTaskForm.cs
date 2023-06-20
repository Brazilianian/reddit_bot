using System;
using System.Windows.Forms;

namespace reddit_bor.form.task
{
    public partial class AddNameToTaskForm : Form
    {
        public string _taskName;

        public AddNameToTaskForm()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _taskName = textBox1.Text;
            if (string.IsNullOrWhiteSpace(_taskName))
            {
                MessageBox.Show("Введіть назву задачі");
                return;
            }
            DialogResult = DialogResult.OK;
        }
    }
}
