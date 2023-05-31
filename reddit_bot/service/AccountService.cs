using System.Collections.Generic;
using reddit_bot.domain;
using reddit_bot.repository;

namespace reddit_bot.service
{
    public class AccountService
    {
        private readonly AccountRepository _accountRepository;
        public AccountService()
        {
            _accountRepository = new AccountRepository();
        }
        
        public RedditAccount Save(RedditAccount account)
        {
           _accountRepository.Save(account);
           return _accountRepository.FindByUsername(account.Username);
        }

        public List<RedditAccount> GetAllAccounts()
        {
            return _accountRepository.FindAll();
        }

        public bool IsAccountAlreadyExists(RedditAccount account)
        {
            var redditAccount = _accountRepository.FindByUsername(account.Username);
            return redditAccount != null;
        }
    }
}