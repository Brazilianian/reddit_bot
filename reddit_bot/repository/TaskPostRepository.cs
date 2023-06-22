using Newtonsoft.Json;
using reddit_bot.domain.task;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace reddit_bor.repository
{
    internal class TaskPostRepository
    {
        private const string _filePath = "./data/postTasks.json";
        private readonly JsonSerializer _jsonSerializer;

        public TaskPostRepository()
        {
            _jsonSerializer = new JsonSerializer();
            if (!File.Exists(_filePath))
            {
                using (FileStream fs = File.Create(_filePath))
                {
                }
                WriteAll(new List<RedditPostTaskPost>());
            }
        }

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
                redditPostTasks = (List<RedditPostTaskPost>)_jsonSerializer.Deserialize(streamReader, typeof(List<RedditPostTaskPost>));
            }
            return redditPostTasks;
        }

        private void WriteAll(List<RedditPostTaskPost> tasks)
        {
            using (StreamWriter streamWriter = new StreamWriter(_filePath))
            {
                _jsonSerializer.Serialize(streamWriter, tasks);
            }
        }
    }
}
