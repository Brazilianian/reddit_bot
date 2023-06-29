using Reddit;
using Reddit.Controllers;
using Reddit.Exceptions;
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

        private List<RedditPostTask> _tasksOrder;

        public delegate void MessageEventHandler(string message);

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
        }

        internal void Start()
        {
            _isWorking = true;
            if (_workedThread != null)
            {
                return;
            }
            _workedThread = new Thread(StartPosting);

            _workedThread.Start();
        }

        internal void Stop()
        {
            _isWorking = false;
            _workedThread.Abort();
            _workedThread = null;
            FillTaskOrder();
        }

        private void StartPosting()
        {
            try
            {
                Random random = new Random();
                for (int i = 0; i < _tasksOrder.Count; i++)
                {
                    var task = _tasksOrder[i];
                    if (!_isWorking)
                    {
                        Thread.Sleep(1000);
                        i--;
                        continue;
                    }
                    if (task is RedditPostTaskPost)
                    {
                        createPost_Post(task as RedditPostTaskPost);
                    }
                    else if (task is RedditPostTaskLink)
                    {
                        createPost_Link(task as RedditPostTaskLink);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }

                    Thread.Sleep(random.Next(_pool.Range.From, _pool.Range.To) * 1000);
                }
            } catch (ThreadAbortException)
            {
                Thread.ResetAbort();
            } catch (Exception ex)
            {
                using (StreamWriter streamWriter = new StreamWriter("./data/errors.txt", true))
                {
                    streamWriter.WriteLine(ex.Message);
                }
            }
        }

        private void FillTaskOrder()
        {
            _tasksOrder = new List<RedditPostTask>();
            foreach (var task in _pool._tasks)
            {
                for (int i = 0; i < task.Count; i++)
                {
                    _tasksOrder.Add(task.PostTask);
                }
            }

            if (_pool.IsRandom)
            {
                ListUtil.Shuffle(_tasksOrder);
            }
        }

        private void createPost_Post(RedditPostTaskPost taskPost)
        {
            var subreddit = _redditClient.Subreddit(taskPost.SubredditName);
            
            if (subreddit == null)
            {
                OnMessageReceived("Сабредіт не знайдено");
            }

            SelfPost post = subreddit
                .SelfPost(title: taskPost.Title, selfText: taskPost.Text);

            //TODO add flair to post
            if (taskPost.PostFlair != null)
            {
                post.Submit(spoiler: taskPost.IsSpoiler, flairText: taskPost.PostFlair.Text, flairId: taskPost.PostFlair.Id);
            }
            else
            {
                post.Submit(spoiler: taskPost.IsSpoiler);
            }

            if (taskPost.IsNSFW)
            {
                post.MarkNSFWAsync();
            }

            OnMessageReceived("Опубліковано");
            //TODO OC tag
        }

        private void createPost_Link(RedditPostTaskLink taskLink)
        {
            var subreddit = _redditClient.Subreddit(taskLink.SubredditName);

            if (subreddit == null)
            {
                OnMessageReceived("Сабредіт не знайдено");
            }

            LinkPost post = subreddit
                .LinkPost(title: taskLink.Title, url: taskLink.Link);

            try
            {
                if (taskLink.PostFlair != null)
                {
                    post.Submit(spoiler: taskLink.IsSpoiler)
                    .SetFlair(taskLink.PostFlair.Text);
                }
                else
                {
                    post.Submit(spoiler: taskLink.IsSpoiler);
                }

                if (taskLink.IsNSFW)
                {
                    post.MarkNSFWAsync();
                }

                OnMessageReceived("Опубліковано");
            }
            catch (RedditAlreadySubmittedException ex)
            {
                OnMessageReceived(ex.Message);
            }
            //TODO OC tag
        }

        protected virtual void OnMessageReceived(string message)
        {
            if (MessageReceived != null)
            {
                MessageReceived.Invoke(message);
            }
        }
    }
}
