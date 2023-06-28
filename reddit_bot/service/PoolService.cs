using reddit_bor.domain.task;
using reddit_bor.dto;
using reddit_bor.mapper;
using reddit_bor.repository;
using System.Collections.Generic;

namespace reddit_bor.service
{
    public class PoolService
    {
        private readonly PoolRepository _poolRepository;
        private readonly PoolMapper _poolMapper;

        public PoolService()
        {
            _poolRepository = new PoolRepository();
            _poolMapper = new PoolMapper();
        }

        public Pool SavePool(Pool pool)
        {
            if (GetPoolByName(pool.Name) != null)
            {
                return null;
            }

            var poolDto = _poolMapper.FromObjectToDto(pool);
            var savedPoolDto = _poolRepository.Save(poolDto);
            
            return _poolMapper.FromDtoToOjbect(savedPoolDto);
        }

        public List<Pool> GetAllPools()
        {
            List<PoolDto> poolDtoList = _poolRepository.FindAll();
            List<Pool> pools = new List<Pool>();
            foreach (var poolDto in poolDtoList)
            {
                pools.Add(_poolMapper.FromDtoToOjbect(poolDto));
            }

            return pools;
        }

        private Pool GetPoolByName(string name)
        {
            var poolDto = _poolRepository.FindByName(name);
            return _poolMapper.FromDtoToOjbect(poolDto);
        }
    }
}
