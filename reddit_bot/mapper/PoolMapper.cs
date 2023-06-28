using reddit_bor.domain.pool;
using reddit_bor.domain.task;
using reddit_bor.dto;
using reddit_bor.repository;
using reddit_bor.service;

namespace reddit_bor.mapper
{
    public class PoolMapper
    {
        private readonly TaskService _taskService;

        public PoolMapper() 
        {
            _taskService = new TaskService();
        }

        public Pool FromDtoToOjbect(PoolDto poolDto)
        {
            if (poolDto == null)
            {
                return null;
            }

            Pool pool = new Pool()
            {
                IsRandom = poolDto.IsRandom,
                Name = poolDto.Name,
                Range = poolDto.Range
            };

            foreach (var poolTaskDto in poolDto.Tasks)
            {
                pool._tasks.Add(
                    new PoolTask(
                        _taskService.FindTaskByName(poolTaskDto.TaskName),
                        poolTaskDto.Count));
            }

            return pool;
        } 

        public PoolDto FromObjectToDto(Pool pool)
        {
            if (pool == null)
            {
                return null;
            }

            PoolDto poolDto = new PoolDto()
            {
                Name = pool.Name,
                Range = pool.Range,
                IsRandom = pool.IsRandom
            };

            foreach (var poolTask in pool._tasks)
            {
                PoolTaskDto poolTaskDto = new PoolTaskDto(poolTask.PostTask.TaskName, poolTask.Count);
                poolDto.Tasks.Add(poolTaskDto);
            }

            return poolDto;
        }
    }
}
