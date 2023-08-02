using reddit_bor.domain.logs;
using reddit_bor.repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace reddit_bor.service
{
    public  class LogService
    {
        private readonly LogRepository _logRepository;

        public LogService() 
        {
            _logRepository = new LogRepository();
        }

        public List<Log> FindAllLogs()
        {
            return _logRepository
                .FindAll()
                .OrderBy(log => log.DateTime)
                .ToList();
        }

        public void WriteLog(Log log)
        {
            _logRepository.Write(log);
        }
    }
}
