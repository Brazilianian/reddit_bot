
using reddit_bor.domain.pool;
using System.Collections.Generic;

namespace reddit_bor.dto
{
    public class PoolDto
    {
        public string Name { get; set; }
        public List<PoolTaskDto> Tasks {  get; set; }
        public IntervalRange Range { get; set; } 
        public bool IsRandom { get; set; }

        public PoolDto()
        {
            Tasks = new List<PoolTaskDto>();
        }

        public PoolDto(string name, IntervalRange range, bool isRandom)
        {
            Tasks = new List<PoolTaskDto>();

            Name = name;
            Range = range;
            IsRandom = isRandom;
        }
    }
}
