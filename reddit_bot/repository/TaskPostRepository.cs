using Newtonsoft.Json;
using reddit_bot.domain.task;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace reddit_bor.repository
{
    internal class TaskPostRepository
    {
        private const string _filePath = "./postTasks.json";
        private readonly JsonSerializer jsonSerializer = new JsonSerializer();

        public RedditPostTaskPost Save(RedditPostTaskPost task)
        {
            List<RedditPostTaskPost> redditPostTasks = FindAll();
            redditPostTasks.Add(task);
            WriteAll(redditPostTasks);
            return task;
        }

        public RedditPostTaskPost FindTaskByName(string name)
        {
            return FindAll()
                .Where(t => t.TaskName.Equals(name))
                .FirstOrDefault();
        }

        public List<RedditPostTaskPost> FindAll()
        {
            List<RedditPostTaskPost> redditPostTasks = new List<RedditPostTaskPost>();
            using (StreamReader streamReader = new StreamReader(_filePath))
            {
                redditPostTasks = (List<RedditPostTaskPost>)jsonSerializer.Deserialize(streamReader, typeof(List<RedditPostTaskPost>));
            }
            return redditPostTasks;
        }

        private void WriteAll(List<RedditPostTaskPost> tasks)
        {
            using (StreamWriter streamWriter = new StreamWriter(_filePath))
            {
                jsonSerializer.Serialize(streamWriter, tasks);
            }
        }
    }
}
