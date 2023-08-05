using Reddit;
using Reddit.Controllers;
using Reddit.Exceptions;
using reddit_bor.domain.pool;
using reddit_bor.service;
using reddit_bot.domain.forms;
using reddit_bot.domain.task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace reddit_bor.form.preset
{
    public partial class PoolSubredditChangeInfoForm : Form
    {
        private readonly RedditClient _redditClient;
        private readonly SubredditService _subredditService;

        public PoolSubreddit _poolSubreddit;

        private Subreddit _subreddit;

        public PoolSubredditChangeInfoForm(PoolSubreddit poolSubreddit, RedditClient redditClient)
        {
            InitializeComponent();

            _redditClient = redditClient;

            _poolSubreddit = poolSubreddit;

            FillForm();
        }

        private void FillForm()
        {
            comboBox2.Text = _poolSubreddit.Name;
            if (_poolSubreddit.Trigger != null)
            {
                textBox1.Text = _poolSubreddit.Trigger.Text;
                switch(_poolSubreddit.Trigger.Place)
                {
                    case Place.Start:
                        radioButton1.Checked = true;
                        break;
                    case Place.Middle:
                        radioButton2.Checked = true;
                        break;
                    case Place.End:
                        radioButton3.Checked = true;
                        break;
                }
            }
            numericUpDown3.Value = _poolSubreddit.Count;
            richTextBox1.Text = _poolSubreddit.AdditionalInfo;

            LoadSubreddit();
            LoadFlairs();
            LoadFlair(_poolSubreddit.PostFlair.Text);

            comboBox3.Text = _poolSubreddit.PostFlair.Text;

            FillSubredditComboBox();
            FillSubredditFlairComboBox();
        }

        private void Ok_Button_Click(object sender, EventArgs e)
        {
            _poolSubreddit.Name = comboBox2.Text;
            _poolSubreddit.Count = (int)numericUpDown3.Value;
            _poolSubreddit.AdditionalInfo = richTextBox1.Text;

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
                }
                else if (radioButton2.Checked)
                {
                    trigger.Place = Place.Middle;
                }
                else if (radioButton3.Checked)
                {
                    trigger.Place = Place.End;
                }
                else
                {
                    MessageBox.Show("Виберіть місце для тригеру");
                    return;
                }
                _poolSubreddit.Trigger = trigger;
            }
            DialogResult = DialogResult.OK;
        }

        private void Cancel_Button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void FillSubredditFlairComboBox()
        {
            comboBox3.SelectedIndexChanged += LoadFlairEvent;
            comboBox3.TextChanged += ChangeFlairNameEvent;
        }

        private void FillSubredditComboBox()
        {
            comboBox2.SelectedIndexChanged += LoadFlairsEvent;
            comboBox2.TextChanged += LoadSubredditEvent;
            comboBox2.Click += LoadValuesToComboBoxEvent;

            button10.Click += LoadSubredditsByNameEvent;
            button10.Click += LoadFlairsEvent;
        }

        #region events
        private void LoadSubredditsByNameEvent(object sender, EventArgs e)
        {
            LoadSubredditsByName();
        }

        private void LoadValuesToComboBoxEvent(object sender, EventArgs e)
        {
            LoadValuesToComboBox();
        }

        private void LoadSubredditEvent(object sender, EventArgs e)
        {
            LoadSubreddit();
        }

        private void LoadFlairsEvent(object sender, EventArgs e)
        {
            LoadFlairs();
        }

        private void LoadFlairEvent(object sender, EventArgs e)
        {
            LoadFlair(((ComboBox)sender).Text);
        }

        private void ChangeFlairNameEvent(object sender, EventArgs e)
        {
            ChangeFlairName(((Control)sender).Text);
        }
        #endregion

        private void LoadSubredditsByName()
        {
            string subredditName = comboBox2.Text;
            if (string.IsNullOrEmpty(subredditName))
            {
                label10.Text = "Введіть назву сабредіту";
                return;
            }

            List<string> subreddits = new List<string>();
            try
            {
                subreddits.AddRange(_redditClient.SearchSubredditNames(subredditName, exact: true).Select(s => s.Name).ToList());
                _redditClient.SearchSubredditNames(subredditName, exact: false).Select(s => s.Name)
                    .ToList().ForEach(s =>
                    {
                        if (!subreddits.Contains(s))
                        {
                            subreddits.Add(s);
                        }
                    });
            }
            catch (RedditNotFoundException)
            {
            }

            comboBox2.Items.Clear();

            if (subreddits.Count == 0)
            {
                Subreddit subredditToSearch = _redditClient.Subreddit(subredditName);
                try
                {
                    subredditToSearch.About();
                    label10.Text = "";
                }
                catch (Exception)
                {
                    label10.Text = "Жодних сабреддітів не знайдено";
                }
            }
            else
            {
                foreach (var subreddit in subreddits)
                {
                    comboBox2.Items.Add(subreddit);
                }
                comboBox2.DroppedDown = true;

                label10.Text = "";
            }
        }

        private void LoadValuesToComboBox()
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

        private void LoadSubreddit()
        {
            _subreddit = _redditClient.Subreddit(comboBox2.Text);
        }

        private void LoadFlairs()
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
            catch (Exception)
            {
                label11.Text = "Флаєри не знайдено";
            }
        }

        private void LoadFlair(string flairText)
        {
            if (_subreddit == null)
            {
                return;
            }

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

        private void ChangeFlairName(string postFlair)
        {
            if (_poolSubreddit.PostFlair != null)
            {
                _poolSubreddit.PostFlair.Text = postFlair;
                return;
            }

            _poolSubreddit.PostFlair = new PostFlair(postFlair);
        }
    }
}
