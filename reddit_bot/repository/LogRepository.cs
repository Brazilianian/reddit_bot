using reddit_bor.domain.logs;
using System;
using System.Collections.Generic;
using System.IO;

namespace reddit_bor.repository
{
    public class LogRepository
    {
        private const string _filePath = "./data/logs.txt";

        public LogRepository() 
        {
            if (!File.Exists(_filePath))
            {
                using (FileStream fs = File.Create(_filePath))
                {
                }
            }
        }

        public List<Log> FindAll()
        {
            List<Log> logs = new List<Log>();
            using (StreamReader stream = new StreamReader(_filePath))
            {
                string line;
                while ((line = stream.ReadLine()) != null)
                {
                    // Assuming the log data is stored in a format like "[DateTime] : [LogLevel] : Message"
                    string[] parts = line.Split(new char[] { '|' }, 3, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 3)
                    {
                        if (DateTime.TryParse(parts[0], out DateTime dateTime) && Enum.TryParse(parts[1].Trim(' ', '[', ']'), out LogLevel logLevel))
                        {
                            Log log = new Log(parts[2].Trim(' ', '[', ']'), logLevel)
                            {
                                DateTime = dateTime
                            };
                            logs.Add(log);
                        }
                    }
                }
            }
            return logs;
        }

        public void Write(Log log)
        {
            using (StreamWriter stream = new StreamWriter(_filePath, true))
            {
                stream.WriteLine(log.ToString());
            }
        }
    }
}
