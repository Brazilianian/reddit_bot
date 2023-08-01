using System;

namespace reddit_bor.domain.logs
{
    public class Log
    {
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
        public LogLevel LogLevel { get; set; }

        public Log(string message, LogLevel logLevel)
        {
            Message = message;
            DateTime = DateTime.Now;
            LogLevel = logLevel;
        }

        public override string ToString()
        {
            return $"{DateTime} | [{LogLevel}] | {Message}";
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
