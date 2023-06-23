using reddit_bor.repository;
using reddit_bot.domain.task;
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
            if (_taskRepository.FindPostTaskByName(task.TaskName) != null)
            {
                return null;
            }

            return _taskRepository.SaveTask(task);   
        }

        public List<RedditPostTask> GetAllTasks()
        {
            return _taskRepository.FindAll();
        }
    }
}
