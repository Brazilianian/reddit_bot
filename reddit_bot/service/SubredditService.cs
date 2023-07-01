using reddit_bor.domain.pool;
using reddit_bor.repository;
using System.Collections.Generic;

namespace reddit_bor.service
{
    public class SubredditService
    {
        private readonly SubredditRepository _subredditRepository;

        public SubredditService()
        {
            _subredditRepository = new SubredditRepository();
        }

        public PoolSubreddit SaveSubreddit(PoolSubreddit poolSubreddit)
        {
            if (_subredditRepository.FindByName(poolSubreddit.Name) != null)
            {
                return null;
            }

            return _subredditRepository.Save(poolSubreddit);
        }

        public List<PoolSubreddit> FindAllSubreddits()
        {
            return _subredditRepository.FindAll();
        }
    }
}
