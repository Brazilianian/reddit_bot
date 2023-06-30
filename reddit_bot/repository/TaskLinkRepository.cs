using Newtonsoft.Json;
using reddit_bot.domain.task;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace reddit_bor.repository
{
    internal class TaskLinkRepository
    {
        private const string _filePath = "./data/linkTasks.json";
        private readonly JsonSerializer _jsonSerializer;

        public TaskLinkRepository()
        {
            _jsonSerializer = new JsonSerializer();
            
            if (!File.Exists(_filePath))
            {
                using (FileStream fs = File.Create(_filePath))
                {
                }
                WriteAll(new List<RedditPostTaskLink>());
            }
        }

        public RedditPostTaskLink Save(RedditPostTaskLink task)
        {
            List<RedditPostTaskLink> redditPostTasks = FindAll();
            redditPostTasks.Add(task);
            WriteAll(redditPostTasks);
            return task;
        }

        public RedditPostTaskLink FindTaskByName(string name)
        {
            throw new NotImplementedException();
        }

        public List<RedditPostTaskLink> FindAll()
        {
            List<RedditPostTaskLink> redditPostTasks = new List<RedditPostTaskLink>();
            using (StreamReader streamReader = new StreamReader(_filePath))
            {
                redditPostTasks = (List<RedditPostTaskLink>)_jsonSerializer.Deserialize(streamReader, typeof(List<RedditPostTaskLink>));
            }
            return redditPostTasks;
        }

        private void WriteAll(List<RedditPostTaskLink> tasks)
        {
            using (StreamWriter streamWriter = new StreamWriter(_filePath))
            {
                _jsonSerializer.Serialize(streamWriter, tasks);
            }
        }
    }
}
