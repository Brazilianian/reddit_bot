
namespace reddit_bor.dto
{
    public class PoolTaskDto
    {
        public string TaskName { get; set; }
        public int Count { get; set; }

        public PoolTaskDto()
        {
        }

        public PoolTaskDto(string postTaskName, int count)
        {
            TaskName = postTaskName;
            Count = count;
        }
    }
}
