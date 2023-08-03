using Newtonsoft.Json;
using reddit_bor.domain.pool;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace reddit_bor.repository
{
    public class PresetRepository
    {
        private const string _filePath = "./data/presets.json";
        private readonly JsonSerializer _jsonSerializer;

        public PresetRepository() 
        {
            _jsonSerializer = new JsonSerializer();
            if (!File.Exists(_filePath))
            {
                using (FileStream fs = File.Create(_filePath))
                {
                }
                WriteAll(new List<Preset>());
            }
        }

        public Preset FindByName(string name)
        {
            return FindAll()
                .Where(p => p.Name.Equals(name))
                .FirstOrDefault();
        } 

        public Preset Save(Preset preset)
        {
            List<Preset> presets = FindAll();
            presets.Add(preset);
            WriteAll(presets);
            return preset;
        }

        public List<Preset> FindAll()
        {
            List<Preset> presets = new List<Preset>();
            using (StreamReader streamReader = new StreamReader(_filePath))
            {
                presets = (List<Preset>)_jsonSerializer.Deserialize(streamReader, typeof(List<Preset>));
            }
            return presets;
        }

        public void WriteAll(List<Preset> presets)
        {
            using (StreamWriter streamWriter = new StreamWriter(_filePath))
            {
                _jsonSerializer.Serialize(streamWriter, presets);
            }
        }
    }
}
