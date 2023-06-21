using Org.BouncyCastle.Bcpg.OpenPgp;

namespace reddit_bor.repository
{
    public class TaskRepository
    {
        private const string _taskPostFilePath = "./postTasks.json";
        private const string _taskLinkFilePath = "./linkTasks.json";

        public TaskRepository() 
        {
        }
    }
}
