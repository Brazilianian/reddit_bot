﻿using Reddit;
using Reddit.Controllers;
using Reddit.Exceptions;
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
            if (_workedThread != null)
            {
                _workedThread.Abort();
            }
            _workedThread = null;
            FillTaskOrder();
        }
        #endregion

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

        #region Creation
        private void createPost_Post(RedditPostTaskPost taskPost, PoolSubreddit poolSubreddit)
        {
            var subreddit = _redditClient.Subreddit(poolSubreddit.Name);

            if (subreddit == null)
            {
                OnMessageReceived(new Log($"Failed to post {taskPost} - subreddit does not specified", LogLevel.Warning), false);
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
                    mess += $"Flair: Text: {poolSubreddit.PostFlair.Text}; Id: {poolSubreddit.PostFlair.Id}. See the link: {result.JSON.Data.URL}";
                }
                OnMessageReceived(new Log(mess, LogLevel.Info), true);
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

        private void createPost_Link(RedditPostTaskLink taskLink, PoolSubreddit poolSubreddit)
        {
            var subreddit = _redditClient.Subreddit(poolSubreddit.Name);

            if (subreddit == null)
            {
                OnMessageReceived(new Log($"Failed to post {taskLink} - subreddit does not specified", LogLevel.Warning), false);
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
                    mess += $"Flair. Text: {poolSubreddit.PostFlair.Text}; Id: {poolSubreddit.PostFlair.Id}. See the link: {result.JSON.Data.URL}";
                }
                OnMessageReceived(new Log(mess, LogLevel.Info), true);
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
            }
        }
    }
}
