using Newtonsoft.Json;
using reddit_bor.domain.pool;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace reddit_bor.repository
{
    public  class SubredditRepository
    {
        private const string _filePath = "./data/subreddits.json";
        private readonly JsonSerializer _jsonSerializer;
  
        public SubredditRepository()
        {
            _jsonSerializer = new JsonSerializer();

            if (!File.Exists(_filePath))
            {
                using (FileStream fs = File.Create(_filePath))
                {
                }
                WriteAll(new List<PoolSubreddit>());
            }
        }

        public PoolSubreddit Save(PoolSubreddit subreddit)
        {
            List<PoolSubreddit> subreddits = FindAll();
            subreddits.Add(subreddit);
            WriteAll(subreddits);
            return subreddit;
        }

        public PoolSubreddit FindByName(string name)
        {
            return FindAll()
                .Where(t => t.Name.Equals(name))
                .FirstOrDefault();
        }

        public List<PoolSubreddit> FindAll()
        {
            List<PoolSubreddit> subreddits = new List<PoolSubreddit>();
            using (StreamReader streamReader = new StreamReader(_filePath))
            {
                subreddits = (List<PoolSubreddit>)_jsonSerializer.Deserialize(streamReader, typeof(List<PoolSubreddit>));
            }
            return subreddits;
        }

        private void WriteAll(List<PoolSubreddit> subreddits)
        {
            using (StreamWriter streamWriter = new StreamWriter(_filePath))
            {
                _jsonSerializer.Serialize(streamWriter, subreddits);
            }
        }
    }
}
