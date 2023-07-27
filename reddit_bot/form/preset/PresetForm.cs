using Reddit.Controllers;
using Reddit;
using reddit_bor.domain.pool;
using reddit_bor.service;
using reddit_bot;
using reddit_bot.domain;
using reddit_bot.domain.forms;
using reddit_bot.domain.task;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using reddit_bot.service;
using reddit_bor.form.publish;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace reddit_bor.form.preset
{
    public partial class PresetForm : Form
    {
        private readonly RedditAccount _redditAccount;
        private readonly AccountsForm _accountsForm;

        private readonly SubredditService _subredditService;
        private readonly RedditService _redditService;
        private readonly PresetService _presetService;

        private Subreddit _subreddit;

        private PoolSubreddit _poolSubreddit;
        private readonly RedditClient _redditClient;

        private readonly List<PoolSubreddit> _poolSubreddits = new List<PoolSubreddit>();
        private List<Preset> _presets;

        public PresetForm(RedditAccount redditAccount, AccountsForm accountsForm) 
        {
            InitializeComponent();

            _redditAccount = redditAccount;
            _accountsForm = accountsForm;

            _subredditService = new SubredditService();
            _redditService = new RedditService();
            _poolSubreddit = new PoolSubreddit();
            _presetService = new PresetService();

            _redditClient = _redditService.GetRedditClient(redditAccount, RequestsUtil.GetUserAgent());

            _presets = _presetService.FindAllPresets();
            FillPresetDataGrid();

            FillForm();
            ResizeForm();
        }

        private void FillForm()
        {
            FillSubredditPanel();
            FillSubredditDataGrid();

            //FillPresetsDataGrid();
        }

        #region Fill Forms
        private void FillSubredditPanel()
        {
            FillSubredditComboBox();
            FillSubredditFlairComboBox();
        }

        private void FillPresetDataGrid()
        {
            foreach (var preset in _presets)
            {
                DataGridViewRow dataGridViewRow = new DataGridViewRow();
                dataGridViewRow.Tag = preset;

                dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell()
                {
                    Value = preset.Name
                });
                dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell()
                {
                    Value = preset.Subreddits.Count
                });
                dataGridView1.Rows.Add(dataGridViewRow);
            }
        }

        private void FillSubredditFlairComboBox()
        {
            comboBox3.SelectedIndexChanged += loadFlair;
            comboBox3.TextChanged += changeFlairName;
        }

        private void FillSubredditComboBox()
        {
            comboBox2.SelectedIndexChanged += loadFlairs;
            comboBox2.TextChanged += loadSubreddit;
            comboBox2.Click += loadValuesToCombobox;

            button10.Click += LoadSubredditsByName;
            button10.Click += loadFlairs;
        }

        private void UpdateSubredditDataGrid()
        {
            dataGridView2.Rows.Clear();
            FillSubredditDataGrid();
        }

        private void FillSubredditDataGrid()
        {
            foreach (var subreddit in _poolSubreddits)
            {
                DataGridViewRow dataGridViewRow = new DataGridViewRow();
                dataGridViewRow.Tag = subreddit;

                dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell()
                {
                    Value = subreddit.Name
                });
                dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell()
                {
                    Value = subreddit.PostFlair == null ? "" : subreddit.PostFlair.Text
                });
                dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell()
                {
                    Value = subreddit.Count
                });
                dataGridView2.Rows.Add(dataGridViewRow);
            }
        }

        private void UpdateSubredditPanel()
        {
            comboBox2.Items.Clear();
            comboBox2.Text = "";

            comboBox3.Items.Clear();
            comboBox3.Text = "";
            comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;

            label10.Text = "";
            label11.Text = "";
        }

        private void RefillPresetsDataGrid()
        {
            dataGridView1.Rows.Clear();
            _presets = _presetService.FindAllPresets();
            FillPresetDataGrid();
        }

        #endregion

        #region Load


        private void loadFlair(object sender, EventArgs e)
        {
            if (_subreddit == null)
            {
                return;
            }

            var flairText = ((ComboBox)sender).Text;

            Reddit.Things.FlairV2 flair = _subreddit.Flairs.GetLinkFlairV2()
                .Where(f => f.Text.Equals(flairText))
                .FirstOrDefault();

            if (flair == null)
            {
                _poolSubreddit.PostFlair = null;
                MessageBox.Show("Flair is null");
                return;
            }
            if (flair.TextEditable)
            {
                _poolSubreddit.PostFlair = new PostFlair(flair.Text, flair.Id);
                comboBox3.DropDownStyle = ComboBoxStyle.DropDown;
            }
            else
            {
                _poolSubreddit.PostFlair = new PostFlair(flair.Text, flair.Id);
                comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;
            }
        }

        private void loadSubreddit(object sender, EventArgs e)
        {
            _subreddit = _redditClient.Subreddit(comboBox2.Text);
        }

        private void loadFlairs(object sender, EventArgs e)
        {
            if (_subreddit == null)
            {
                return;
            }

            Flairs flairs = _subreddit.Flairs;

            comboBox3.Items.Clear();

            try
            {
                label11.Text = "";
                comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;

                List<Reddit.Things.FlairV2> flairList = flairs.LinkFlairV2;
                foreach (var flair in flairList)
                {
                    comboBox3.Items.Add(new ComboBoxItem(flair.Text, flair.Id));
                }

                if (comboBox3.Items.Count == 0)
                {
                    label11.Text = "Для даного сабредіту флаєри не знайдено";
                }
                else
                {
                    label11.Text = "";
                }
            }
            catch (Exception ex)
            {
                label11.Text = "Флаєри не знайдено";
            }
        }

        private void LoadSubredditsByName(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(comboBox2.Text))
            {
                label10.Text = "Жодних сабреддітів не знайдено";
                return;
            }

            List<Subreddit> subreddits = _redditClient.SearchSubreddits(comboBox2.Text, limit: 10);
            comboBox2.Items.Clear();
            foreach (var subreddit in subreddits)
            {
                comboBox2.Items.Add(subreddit.Name);
            }
            comboBox2.DroppedDown = true;

            if (subreddits.Count == 0)
            {
                label10.Text = "Жодних сабреддітів не знайдено";
            }
            else
            {
                label10.Text = "";
            }
        }


        private void loadValuesToCombobox(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(comboBox2.Text))
            {
                List<PoolSubreddit> existedSubreddits = _subredditService.FindAllSubreddits();
                comboBox2.Items.Clear();
                foreach (var subreddit in existedSubreddits)
                {
                    if (!comboBox2.Items.Contains(subreddit.Name))
                    {
                        comboBox2.Items.Add(subreddit.Name);
                    }
                }
            }
        }

        private void changeFlairName(object sender, EventArgs e)
        {
            if (_poolSubreddit.PostFlair != null)
            {
                _poolSubreddit.PostFlair.Text = ((Control)sender).Text;
                return;
            }

            _poolSubreddit.PostFlair = new PostFlair(((Control)sender).Text);
        }
        #endregion

        //Add subreddit and flair
        private void button9_Click(object sender, System.EventArgs e)
        {
            _poolSubreddit.Name = comboBox2.Text;
            _poolSubreddit.Count = (int)numericUpDown3.Value;

            if (string.IsNullOrEmpty(_poolSubreddit.Name))
            {
                MessageBox.Show("Виберіть ім'я сабредіту");
                return;
            }

            _poolSubreddits.Add(_poolSubreddit);
            _subredditService.SaveSubreddit(_poolSubreddit);

            _poolSubreddit = new PoolSubreddit();

            UpdateSubredditDataGrid();
            UpdateSubredditPanel();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NewPublishForm newPublishForm = new NewPublishForm(_redditAccount, _accountsForm);
            newPublishForm.Show();
            Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AccountInfoForm accountInfoForm = new AccountInfoForm(_redditAccount, _accountsForm);
            accountInfoForm.Show();
            Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Preset preset = new Preset();
            preset.Subreddits = _poolSubreddits;
            
            PresetAddNameDialogForm presetAddNameDialogForm = new PresetAddNameDialogForm();
            if (presetAddNameDialogForm.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            preset.Name = presetAddNameDialogForm._name;
            
            if (_presetService.SavePreset(preset) == null)
            {
                MessageBox.Show("Пресет з таким ім'я вже існує");
                return;
            }

            _presets.Add(preset);
            RefillPresetsDataGrid();

            _poolSubreddits.Clear();
            UpdateSubredditDataGrid();
         
            _poolSubreddit = new PoolSubreddit();
            UpdateSubredditPanel();
        }

        #region Resize
        private void PresetForm_SizeChanged(object sender, EventArgs e)
        {
            ResizeForm();
        }

        private void ResizeForm()
        {
            ResizeMenu();
            ResizePreset();
        }

        private void ResizeMenu()
        {
            Size windowSize = new Size(ClientSize.Width, ClientSize.Height);
            panel5.Size = new Size(windowSize.Width / 5 - 10, windowSize.Height - 20);
            panel5.Location = new Point(5, 5);

            button4.Size = new Size(panel5.Width - 6, 40);
            button1.Size = new Size(panel5.Width - 6, 40);
            button11.Size = new Size(panel5.Width - 6, 40);
            button2.Size = new Size(panel5.Width - 6, 40);
        }

        private void ResizePreset()
        {
            //Main
            Size windowSize = new Size(ClientSize.Width, ClientSize.Height);
            panel1.Size = new Size(windowSize.Width * 4 / 5 - 10, windowSize.Height - 20);
            panel1.Location = new Point(panel5.Location.X + panel5.Width + 5, panel5.Location.Y);

            //First
            dataGridView1.Size = new Size(panel1.Width - 10, panel1.Height / 2 - 50);
            label1.Location = new Point(dataGridView1.Location.X + dataGridView1.Width / 2 - label1.Width / 2, label1.Location.Y);

            //Second
            label5.Location = new Point(label1.Location.X + label1.Width / 2 - label5.Width / 2, dataGridView1.Location.Y + dataGridView1.Height + 5);
            dataGridView2.Size = new Size((panel1.Width - 10) / 2, panel1.Height / 2);
            dataGridView2.Location = new Point(dataGridView1.Location.X, label5.Location.Y + label5.Height + 2);

            panel4.Size = new Size((panel1.Width - 12) / 2, panel1.Height / 2 - 50);
            panel4.Location = new Point(dataGridView2.Location.X + dataGridView2.Width + 3, dataGridView2.Location.Y);

            comboBox2.Width = panel4.Width - 8;
            button10.Location = new Point(panel4.Width - button10.Width - 10, comboBox2.Location.Y + Height + 3);

            label9.Location = new Point(3, panel4.Height / 2 - 30);
            comboBox3.Location = new Point(label9.Location.X, label9.Location.Y + label9.Height);
            comboBox3.Width = panel4.Width - 8;

            numericUpDown3.Location = new Point(3, panel4.Height - numericUpDown3.Height - 12);
            label15.Location = new Point(numericUpDown3.Location.X, numericUpDown3.Location.Y - label15.Height - 3);

        }
        #endregion
    }
}
