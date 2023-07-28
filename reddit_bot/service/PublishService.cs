using Reddit;
using Reddit.Controllers;
using Reddit.Exceptions;
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

        private Thread _workedThread;
        public bool _isWorking;
        private bool _isPause;
        private List<RedditPostTask> _tasksOrder;
        private List<PoolSubreddit> _subreddits;

        public delegate void MessageEventHandler(Log log, bool isIncrement);

        public event MessageEventHandler MessageReceived;

        public PublishService(Pool pool, RedditAccount redditAccount)
        {
            _pool = pool;
            _redditAccount = redditAccount;

            _redditService = new RedditService();
            _redditClient = _redditService.GetRedditClient(_redditAccount, RequestsUtil.GetUserAgent());
            
            FillTaskOrder();
        }

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
            if (_workedThread != null)
            {
                _workedThread.Abort();
            }
            _workedThread = null;
            FillTaskOrder();
        }

        private void StartPosting()
        {
            try
            {
                Random random = new Random();
                for (int i = 0; i < _subreddits.Count; i++)
                {
                    if (i == _tasksOrder.Count)
                    {
                        OnMessageReceived(new Log("Posting finished", LogLevel.Info), false);
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
                        createPost_Post(task as RedditPostTaskPost, poolSubreddit);
                    }
                    else if (task is RedditPostTaskLink)
                    {
                        createPost_Link(task as RedditPostTaskLink, poolSubreddit);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                    Thread.Sleep(random.Next(_pool.Range.From, _pool.Range.To) * 1000);
                }
            } catch(ThreadAbortException)
            {
            } catch (Exception ex)
            {
                using (StreamWriter streamWriter = new StreamWriter("./data/errors.txt", true))
                {
                    streamWriter.WriteLine(ex.StackTrace);
                }
            }
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

        private void createPost_Post(RedditPostTaskPost taskPost, PoolSubreddit poolSubreddit)
        {
            var subreddit = _redditClient.Subreddit(poolSubreddit.Name);

            if (subreddit == null)
            {
                OnMessageReceived(new Log($"Failed to post - subreddit does not specified", LogLevel.Warning), false);
            }

            SelfPost post = subreddit
                .SelfPost(title: taskPost.Title, selfText: taskPost.Text);

            if (poolSubreddit.PostFlair != null)
            {
                post.Submit(spoiler: taskPost.IsSpoiler, flairText: poolSubreddit.PostFlair.Text, flairId: poolSubreddit.PostFlair.Id);
            }
            else
            {
                post.Submit(spoiler: taskPost.IsSpoiler);
            }

            if (taskPost.IsNSFW)
            {
                post.MarkNSFWAsync();
            }

            string mess = $"Posted succesfully: Subreddit: {poolSubreddit}. Post: {taskPost}. ";
            if (poolSubreddit.PostFlair != null)
            {
                mess += $"Flair: Text: {poolSubreddit.PostFlair.Text}; Id: {poolSubreddit.PostFlair.Id}";
            }
            OnMessageReceived(new Log(mess, LogLevel.Info), true);
            //TODO OC tag
        }

        private void createPost_Link(RedditPostTaskLink taskLink, PoolSubreddit poolSubreddit)
        {
            var subreddit = _redditClient.Subreddit(poolSubreddit.Name);

            if (subreddit == null)
            {
                OnMessageReceived(new Log($"Failed to post Task {taskLink} - subreddit does not specified", LogLevel.Warning), false);
            }

            LinkPost post = subreddit
                .LinkPost(title: taskLink.Title, url: taskLink.Link);

            try
            {
                if (poolSubreddit.PostFlair != null)
                {
                    post.Submit(spoiler: taskLink.IsSpoiler)
                    .SetFlair(poolSubreddit.PostFlair.Text);
                }
                else
                {
                    post.Submit(spoiler: taskLink.IsSpoiler);
                }

                if (taskLink.IsNSFW)
                {
                    post.MarkNSFWAsync();
                }

                string mess = $"Posted succesfully: Subreddit: {poolSubreddit}. Post: {taskLink} ";
                if (poolSubreddit.PostFlair != null)
                {
                    mess += $"Flair. Text: {poolSubreddit.PostFlair.Text}; Id: {poolSubreddit.PostFlair.Id}";
                }
                OnMessageReceived(new Log(mess, LogLevel.Info), true);
            }
            catch (RedditAlreadySubmittedException ex)
            {
                OnMessageReceived(new Log($"Failed to post task {taskLink} - {ex.Message}", LogLevel.Warning), true);
            }
            catch (Exception ex)
            {
                OnMessageReceived(new Log($"Failed to post task {taskLink} - Unexpected error - {ex.Message}", LogLevel.Error), true);
            }
            //TODO OC tag
        }

        protected virtual void OnMessageReceived(Log log,  bool isIncrement)
        {
            if (MessageReceived != null)
            {
                MessageReceived.Invoke(log, isIncrement);
            }
        }
    }
}
