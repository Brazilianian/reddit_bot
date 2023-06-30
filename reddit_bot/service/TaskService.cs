using reddit_bor.domain.pool;
using reddit_bor.repository;
using reddit_bot.domain.task;
using System;
using System.Collections.Generic;

namespace reddit_bor.service
{
    public class TaskService
    {
        private readonly TaskRepository _taskRepository;
        
        public TaskService() 
        {
            _taskRepository = new TaskRepository();
        }

        public RedditPostTask SaveTask(RedditPostTask task)
        {
            throw new NotImplementedException();
        }

        public List<RedditPostTask> GetAllTasks()
        {
            return _taskRepository.FindAll();
        }

        internal RedditPostTask FindTaskByName(string taskName)
        {
            return _taskRepository.FindPostTaskByName(taskName);
        }
    }
}
