using Newtonsoft.Json;
using reddit_bor.dto;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace reddit_bor.repository
{
    public class PoolRepository
    {
        private const string _filePath = "./data/pools.json";
        private readonly JsonSerializer _jsonSerializer;

        public PoolRepository() 
        {
            _jsonSerializer = new JsonSerializer();

            if (!File.Exists(_filePath))
            {
                using (FileStream fs = File.Create(_filePath))
                {
                }
                WriteAll(new List<PoolDto>());
            }
        }

        public PoolDto Save(PoolDto pool)
        {
            List<PoolDto> pools = FindAll();
            pools.Add(pool);
            WriteAll(pools);
            return pool;
        }
            
        public PoolDto FindByName(string name)
        {
            return FindAll()
                .Where(t => t.Name.Equals(name))
                .FirstOrDefault();
        }

        public List<PoolDto> FindAll()
        {
            List<PoolDto> pools = new List<PoolDto>();
            using (StreamReader streamReader = new StreamReader(_filePath))
            {
                pools = (List<PoolDto>)_jsonSerializer.Deserialize(streamReader, typeof(List<PoolDto>));
            }
            return pools;
        }

        private void WriteAll(List<PoolDto> pools)
        {
            using (StreamWriter streamWriter = new StreamWriter(_filePath))
            {
                _jsonSerializer.Serialize(streamWriter, pools);
            }
        }
    }
}
