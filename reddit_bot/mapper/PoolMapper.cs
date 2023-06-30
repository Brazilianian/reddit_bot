using reddit_bor.domain.pool;
using reddit_bor.domain.task;
using reddit_bor.dto;
using reddit_bor.service;
using System;

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
            throw new NotImplementedException();
        } 

        public PoolDto FromObjectToDto(Pool pool)
        {
            throw new NotImplementedException();
        }
    }
}
