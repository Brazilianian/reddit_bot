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
using reddit_bor.form.log;
using System.IO;
using reddit_bor.domain.logs;

namespace reddit_bor.form.preset
{
    public partial class PresetForm : Form
    {
        private readonly RedditAccount _redditAccount;
        private readonly AccountsForm _accountsForm;
        private NewPublishForm _newPublishForm;

        private readonly SubredditService _subredditService;
        private readonly RedditService _redditService;
        private readonly PresetService _presetService;
        private readonly LogService _logService;

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
            _logService = new LogService();

            _redditClient = _redditService.GetRedditClient(redditAccount, RequestsUtil.GetUserAgent());

            _presets = _presetService.FindAllPresets();
            FillPresetDataGrid();

            FillForm();
            ResizeForm();
        }

        public PresetForm(RedditAccount redditAccount, AccountsForm accountsForm, NewPublishForm newPublishForm) 
            : this(redditAccount, accountsForm)
        {
            _newPublishForm = newPublishForm;
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
                dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell()
                {
                    Value = subreddit.Trigger == null ? "" : subreddit.Trigger.ToString()
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
            _poolSubreddit.PostFlair = new PostFlair(flair.Text, flair.Id);

            if (flair.TextEditable)
            {
                comboBox3.DropDownStyle = ComboBoxStyle.DropDown;
            }
            else
            {
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

            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                Trigger trigger = new Trigger(textBox1.Text);
                if (radioButton1.Checked)
                {
                    trigger.Place = Place.Start;
                } else if (radioButton2.Checked)
                {
                    trigger.Place = Place.Middle;
                } else if (radioButton3.Checked)
                {
                    trigger.Place = Place.End;
                } else
                {
                    MessageBox.Show("Виберіть місце для тригеру");
                    return;
                }
                _poolSubreddit.Trigger = trigger;
            }

            _poolSubreddits.Add(_poolSubreddit);
            _subredditService.SaveSubreddit(_poolSubreddit);

            _poolSubreddit = new PoolSubreddit();

            UpdateSubredditDataGrid();
            UpdateSubredditPanel();
        }

        #region Menu Panel
        private void button1_Click(object sender, EventArgs e)
        {
            if (_newPublishForm == null)
            {
                _newPublishForm = new NewPublishForm(_redditAccount, _accountsForm, this);
            }
            _newPublishForm.Show();
            Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AccountInfoForm accountInfoForm = new AccountInfoForm(_redditAccount, _accountsForm);
            accountInfoForm.Show();
            Close();
            _newPublishForm?.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _accountsForm.Show();
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            LogForm logForm = new LogForm(_accountsForm, _redditAccount);
            logForm.Show();
            Close();
            _newPublishForm?.Close();
        }
        #endregion

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
            button3.Size = new Size(panel5.Width - 6, 40);
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
            button10.Location = new Point(comboBox2.Location.X + comboBox2.Width - button10.Width, comboBox2.Location.Y + button10.Height + 3);
            label10.Location = new Point(comboBox2.Location.X, comboBox2.Location.Y + comboBox2.Height);

            label9.Location = new Point(0, panel4.Height * 2 / 5 - 30);
            comboBox3.Location = new Point(label9.Location.X, label9.Location.Y + label9.Height);
            comboBox3.Width = panel4.Width;
            label11.Location = new Point(comboBox3.Location.X, comboBox3.Location.Y + comboBox3.Height);

            label2.Location = new Point(0, panel4.Height * 3 / 5 - 30);
            textBox1.Location = new Point(label2.Location.X, label2.Location.Y + label2.Height);
            textBox1.Width = panel4.Width;

            radioButton1.Location = new Point(textBox1.Location.X, textBox1.Location.Y + textBox1.Height + 3);
            radioButton2.Location = new Point(radioButton1.Location.X + radioButton1.Width + 2, radioButton1.Location.Y);
            radioButton3.Location = new Point(radioButton2.Location.X + radioButton2.Width + 2, radioButton2.Location.Y);

            numericUpDown3.Location = new Point(3, panel4.Height - numericUpDown3.Height - 12);
            label15.Location = new Point(numericUpDown3.Location.X, numericUpDown3.Location.Y - label15.Height - 3);

            pictureBox1.Location = new Point(panel4.Location.X, button6.Location.Y);
        }
        #endregion

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    List<PoolSubreddit> subreddits = new List<PoolSubreddit>();

                    using (StreamReader reader = new StreamReader(openFileDialog1.FileName))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            string[] parts = line.Split(',');

                            if (parts.Length < 1 || parts.Length > 4) 
                            {
                                Log log = new Log($"Error reading the file: invalid format near '{line}'", LogLevel.Error);
                                _logService.WriteLog(log);
                                MessageBox.Show(log.Message);
                                return;
                            }

                            //Just Subreddit Name
                            string subredditStr = parts[0].Trim();

                            if (subredditStr.StartsWith("r/"))
                            {
                                subredditStr = subredditStr.Replace("r/", "");
                            }

                            PoolSubreddit poolSubreddit = new PoolSubreddit()
                            {
                                Name = subredditStr
                            };

                            Subreddit subreddit = _redditClient.Subreddit(subredditStr);

                            //Flair + Trigger
                            if (parts.Length == 4)
                            {
                                string flairText = parts[1].Trim();
                                string placeStr = parts[2].Trim();
                                string triggerText = parts[3].Trim();

                                //Flair
                                if (!string.IsNullOrEmpty(flairText))
                                {
                                    Reddit.Things.FlairV2 flair = subreddit.Flairs.GetLinkFlairV2()
                                        .Where(f => f.Text.Equals(flairText))
                                        .FirstOrDefault();

                                    if (flair == null)
                                    {
                                        _poolSubreddit.PostFlair = null;
                                        Log log = new Log($"Error parse flair near {line}", LogLevel.Error);
                                        _logService.WriteLog(log);
                                        MessageBox.Show(log.Message);
                                    } else
                                    {
                                        poolSubreddit.PostFlair = new PostFlair(flair.Text, flair.Id);
                                    }
                                }

                                //Trigger
                                if (!string.IsNullOrEmpty(placeStr)) 
                                {
                                    if (Enum.TryParse(placeStr, out Place place))
                                    {
                                        poolSubreddit.Trigger = new Trigger(triggerText, place);
                                    }
                                    else
                                    {
                                        Log log = new Log($"Error reading the file: invalid Trigger Place. Must be 'start', 'middle' or 'end'", LogLevel.Error);
                                        _logService.WriteLog(log);
                                        MessageBox.Show(log.Message);
                                        return;
                                    }   
                                }
                                subreddits.Add(poolSubreddit);
                            }
                        }
                    }

                    _poolSubreddits.AddRange(subreddits);
                    UpdateSubredditDataGrid();
                }
                catch (Exception ex)
                {
                    Log log = new Log("Error reading the file: " + ex.Message, LogLevel.Error);
                    _logService.WriteLog(log);
                    MessageBox.Show(log.Message);
                }
            }
        }

        private void clear_button_click(object sender, EventArgs e)
        {
            _poolSubreddits.Clear();
            _poolSubreddit = new PoolSubreddit();
            UpdateSubredditDataGrid();
            UpdateSubredditPanel();
        }

        #region Forbid Close Button
        private const int CP_DISABLE_CLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle = cp.ClassStyle | CP_DISABLE_CLOSE_BUTTON;
                return cp;
            }
        }
        #endregion
    }
}
