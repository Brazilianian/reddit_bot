using Reddit;
using Reddit.Inputs.LinksAndComments;
using reddit_bor.domain.logs;
using reddit_bor.domain.pool;
using reddit_bor.domain.task;
using reddit_bor.util;
using reddit_bot.domain;
using reddit_bot.domain.task;
using reddit_bot.service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace reddit_bor.service
{
    public class PublishService
    {
        private readonly Pool _pool;
        private readonly RedditAccount _redditAccount;
        private readonly RedditClient _redditClient;

        private readonly RedditService _redditService;
        private readonly LogService _logService;
        private readonly ReportService _reportService;

        private Thread _workedThread;
        public bool _isWorking;
        private bool _isPause;

        private List<RedditPostTask> _tasksOrder;
        private List<PoolSubreddit> _subreddits;
        private List<Reddit.Things.PostResultShortContainer> _results;

        public delegate void MessageEventHandler(Log log, bool isIncrement);
        public delegate void ProccessFinishingEventHandler();

        public event MessageEventHandler MessageReceived;
        public event ProccessFinishingEventHandler ProccessFinishing;

        public PublishService(Pool pool, RedditAccount redditAccount)
        {
            _pool = pool;
            _redditAccount = redditAccount;

            _redditService = new RedditService();
            _logService = new LogService();
            _reportService = new ReportService();

            _redditClient = _redditService.GetRedditClient(_redditAccount, RequestsUtil.GetUserAgent());
            
            FillTaskOrder();
        }

        #region Controls
        internal void Pause()
        {
            _isWorking = false;
            _isPause = true;
        }

        internal void Start()
        {
            _isWorking = true;
            if (_isPause)
            {
                _isPause = false;
                return;
            }
            _workedThread = new Thread(StartPosting);

            _workedThread.Start();
        }

        internal void Stop()
        {
            _isWorking = false;
            _isPause = false;
            _workedThread?.Abort();
            _workedThread = null;
            FillTaskOrder();
        }
        #endregion

        private void StartPosting()
        {
            _results = new List<Reddit.Things.PostResultShortContainer> ();
            try
            {
                Random random = new Random();
                for (int i = 0; i < _subreddits.Count; i++)
                {
                    if (i == _tasksOrder.Count)
                    {
                        break;
                    }

                    var task = _tasksOrder[i];

                    var poolSubreddit = _subreddits[i];

                    if (!_isWorking)
                    {
                        Thread.Sleep(1000);
                        i--;
                        continue;
                    }
                    if (task is RedditPostTaskPost)
                    {
                        CreatePost_Post(task as RedditPostTaskPost, poolSubreddit);
                    }
                    else if (task is RedditPostTaskLink)
                    {
                        CreatePost_Link(task as RedditPostTaskLink, poolSubreddit);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                    Thread.Sleep(random.Next(_pool.Range.From, _pool.Range.To) * 1000);
                }

                OnMessageReceived(new Log("Posting finished", LogLevel.Info), false);
                FinishTheProccess();

            }
            catch (ThreadAbortException) { } 
            catch (Exception ex)
            {
                Log log = new Log($"Unexpected Error - {ex.Message}", LogLevel.Error);
                _logService.WriteLog(log);
            }
        }

        private void CreateReport()
        {
            _reportService.CreateReport(_results);
        }

        private void FillTaskOrder()
        {
            _tasksOrder = new List<RedditPostTask>();
            _subreddits = new List<PoolSubreddit>();
         
            foreach (var task in _pool._tasks)
            {
                for (int i = 0; i < task.Count; i++)
                {
                    _tasksOrder.Add(task.PostTask);
                }
            }

            foreach (var subreddit in _pool._subreddits)
            {
                for (int i = 0; i < subreddit.Count; i++)
                {
                    _subreddits.Add(subreddit);
                }
            }

            if (_pool.IsRandom)
            {
                ListUtil.Shuffle(_tasksOrder);
                ListUtil.Shuffle(_subreddits);
            }
        }

        #region Creation
        private void CreatePost_Post(RedditPostTaskPost taskPost, PoolSubreddit poolSubreddit)
        {
            var subreddit = _redditClient.Subreddit(poolSubreddit.Name);

            if (subreddit == null)
            {
                OnMessageReceived(new Log($"Failed to post {taskPost} - subreddit does not specified", LogLevel.Warn), false);
            }

            string title = taskPost.Text;

            if (poolSubreddit.Trigger != null)
            {
                switch (poolSubreddit.Trigger.Place)
                {
                    case Place.Start:
                        title = poolSubreddit.Trigger.Text + " " + taskPost.Title;
                        break;
                    case Place.Middle:
                        var words = taskPost.Title.Split(' ');

                        Random random = new Random();
                        int randomIndex = random.Next(1, words.Length - 1);
                        Array.Resize(ref words, words.Length + 1);
                        for (int i = words.Length - 1; i > randomIndex; i--)
                        {
                            words[i] = words[i - 1];
                        }
                        words[randomIndex] = poolSubreddit.Trigger.Text;

                        title = string.Join(" ", words);
                        break;
                    case Place.End:
                        title = taskPost.Title + " " + poolSubreddit.Trigger.Text;
                        break;
                }
            }

            try
            {
                Reddit.Things.PostResultShortContainer result = _redditClient.Models.LinksAndComments.Submit(new LinksAndCommentsSubmitInput(
                    sr: poolSubreddit.Name,
                    title: title,
                    kind: "self",
                    text: taskPost.Text,
                    nsfw: taskPost.IsNSFW,
                    spoiler: taskPost.IsSpoiler,
                    flairId: poolSubreddit.PostFlair == null ? "" : poolSubreddit.PostFlair.Id,
                    flairText: poolSubreddit.PostFlair == null ? "" : poolSubreddit.PostFlair.Text
                    ));

                if (result.JSON.Errors.Count > 0)
                {
                    string errors = string.Join(" - ", result.JSON.Errors[0]);
                    OnMessageReceived(new Log(errors, LogLevel.Error), true);
                    return;
                }

                string mess = $"Posted succesfully: Subreddit: {poolSubreddit}. Post: {taskPost}. ";
                if (poolSubreddit.PostFlair != null)
                {
                    mess += $"Flair: Text: {poolSubreddit.PostFlair.Text}; Id: {poolSubreddit.PostFlair.Id}. ";
                }
                mess += $"See the link: {result.JSON.Data.URL}";
                OnMessageReceived(new Log(mess, LogLevel.Info), true);

                _results.Add(result);
            } catch(Exception ex) {
                string mess = $"Failed to post task {taskPost} ";
                if (poolSubreddit.PostFlair != null)
                {
                    mess += $"Flair: Text: {poolSubreddit.PostFlair.Text}; Id: {poolSubreddit.PostFlair.Id}";
                }
                OnMessageReceived(new Log(mess + $" - Unexpected error - {ex.Message}", LogLevel.Error), true);
        }
            //TODO OC tag
        }

        private void CreatePost_Link(RedditPostTaskLink taskLink, PoolSubreddit poolSubreddit)
        {
            var subreddit = _redditClient.Subreddit(poolSubreddit.Name);

            if (subreddit == null)
            {
                OnMessageReceived(new Log($"Failed to post {taskLink} - subreddit does not specified", LogLevel.Warn), false);
            }

            string title = taskLink.Title;

            if (poolSubreddit.Trigger != null)
            {
                switch (poolSubreddit.Trigger.Place) 
                {
                    case Place.Start:
                        title = poolSubreddit.Trigger.Text + " " + taskLink.Title;
                        break;
                    case Place.Middle:
                        var words = taskLink.Title.Split(' ');

                        Random random = new Random();
                        int randomIndex = random.Next(1, words.Length - 1);
                        Array.Resize(ref words, words.Length + 1);
                        for (int i = words.Length - 1; i > randomIndex; i--)
                        {
                            words[i] = words[i - 1];
                        }
                        words[randomIndex] = poolSubreddit.Trigger.Text;

                        title = string.Join(" ", words);
                        break;
                    case Place.End:
                        title = taskLink.Title + " " + poolSubreddit.Trigger.Text;
                        break;
                }
            }

            try
            {
                Reddit.Things.PostResultShortContainer result = _redditClient.Models.LinksAndComments.Submit(new LinksAndCommentsSubmitInput(
                    sr: poolSubreddit.Name,
                    title: title,
                    kind: "link",
                    url: taskLink.Link,
                    nsfw: taskLink.IsNSFW,
                    spoiler: taskLink.IsSpoiler,
                    flairId: poolSubreddit.PostFlair == null ? "" : poolSubreddit.PostFlair.Id,
                    flairText: poolSubreddit.PostFlair == null ? "" : poolSubreddit.PostFlair.Text
                    ));

                if (result.JSON.Errors.Count > 0 )
                {
                    string errors = string.Join(" - ", result.JSON.Errors[0]);
                    OnMessageReceived(new Log(errors, LogLevel.Error), true);
                    return;
                }

                string mess = $"Posted succesfully: Subreddit: {poolSubreddit}. Post: {taskLink} ";
                if (poolSubreddit.PostFlair != null)
                {
                    mess += $"Flair. Text: {poolSubreddit.PostFlair.Text}; Id: {poolSubreddit.PostFlair.Id}. ";
                }
                mess += $"See the link: {result.JSON.Data.URL}";
                OnMessageReceived(new Log(mess, LogLevel.Info), true);

                _results.Add(result);
            }
            catch (Exception ex)
            {
                string mess = $"Failed to post task {taskLink} ";
                if (poolSubreddit.PostFlair != null)
                {
                    mess += $"Flair: Text: {poolSubreddit.PostFlair.Text}; Id: {poolSubreddit.PostFlair.Id}";
                }
                OnMessageReceived(new Log (mess + $" - Unexpected error - {ex.Message}", LogLevel.Error), true);
            }
            //TODO OC tag
        }
        #endregion

        protected virtual void OnMessageReceived(Log log,  bool isIncrement)
        {
            if (MessageReceived != null)
            {
                MessageReceived.Invoke(log, isIncrement);
                _logService.WriteLog(log);
            }
        }

        private void FinishTheProccess()
        {
            CreateReport();
            ProccessFinishing.Invoke();
        }
    }
}
