using reddit_bot.domain.task;
using System;
using System.Collections.Generic;
using System.Linq;

namespace reddit_bor.repository
{
    public class TaskRepository
    {
        private readonly TaskLinkRepository _taskLinkRepository;
        private readonly TaskPostRepository _taskPostRepository;

        public TaskRepository() 
        {
            _taskLinkRepository = new TaskLinkRepository();
            _taskPostRepository = new TaskPostRepository();
        }

        public RedditPostTask SaveTask(RedditPostTask task)
        {
            if (task is RedditPostTaskPost)
            {
                return _taskPostRepository.Save(task as RedditPostTaskPost);
            } else if (task is RedditPostTaskLink)
            {
                return _taskLinkRepository.Save(task as RedditPostTaskLink);
            } else
            {
                throw new NotImplementedException();
            }
        }

        public List<RedditPostTask> FindAll()
        {
            List<RedditPostTask> redditPostTasks = new List<RedditPostTask>();
            redditPostTasks.AddRange(_taskLinkRepository.FindAll());
            redditPostTasks.AddRange(_taskPostRepository.FindAll());
            return redditPostTasks;
        }

        public RedditPostTask FindPostTaskByName(string name)
        {
            return FindAll()
                .Where(t => t.TaskName.Equals(name))
                .FirstOrDefault();
        }
    }
}
