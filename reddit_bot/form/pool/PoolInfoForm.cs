using reddit_bor.domain.task;
using reddit_bor.form.task;
using System.Linq;
using System.Windows.Forms;

namespace reddit_bor.form.pool
{
    public partial class PoolInfoForm : Form
    {
        private readonly Pool _pool;
        public PoolInfoForm(Pool pool)
        {
            InitializeComponent();

            _pool = pool;
            FillForm();
        }

        private void FillForm()
        {
            label1.Text = _pool.Name;
            checkBox1.Checked = _pool.IsRandom;
            numericUpDown1.Value = _pool.Range.From;
            numericUpDown2.Value = _pool.Range.To;

            dataGridView1.Columns.Add(new DataGridViewColumn()
            {
                Name = "Назва задачі"
            });
            dataGridView1.Columns.Add(new DataGridViewColumn()
            {
                Name = "Кількість постів"
            });

            foreach (var poolTask in _pool._tasks)
            {
                DataGridViewRow dataGridViewRow = new DataGridViewRow()
                {
                    Tag = poolTask.PostTask.TaskName
                };

                dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell()
                {
                    Value = poolTask.PostTask.TaskName
                });
                dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell()
                {
                    Value = poolTask.Count
                });

                dataGridView1.Rows.Add(dataGridViewRow);
            }
        }

        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var taskName = ((DataGridView)sender).SelectedRows[0].Tag.ToString();
            var taskInfoForm = new TaskInfoForm(
                _pool._tasks
                .Where(t => t.PostTask.TaskName.Equals(taskName))
                .FirstOrDefault().PostTask);
            taskInfoForm.ShowDialog();
        }
    }
}
