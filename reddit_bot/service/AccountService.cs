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

        public void Save(RedditAccount account)
        {
            _accountRepository.Save(account);
        }

        public List<RedditAccount> GetAllAccounts()
        {
            return _accountRepository.FindAll();
        }

        public bool IsAccountAlreadyExists(string accountId)
        {
            var redditAccount = _accountRepository.FindByAccountId(accountId);
            return redditAccount != null;
        }

        public void UpdateAccountStatus(RedditAccount account)
        {
            account.Status = RedditAccountStatus.Connected;
            //TODO update status
        }
    }
}