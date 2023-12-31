﻿using System;
using System.Collections.Generic;
using System.Linq;
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
            //TODO update status
            account.Status = RedditAccountStatus.Connected;
        }

        public RedditAccount GetAccountByAccountId(string accountId)
        {
            return _accountRepository.FindByAccountId(accountId);
        }

        internal void DeleteAccountByAccountId(string accountId)
        {
            List<RedditAccount> accounts = GetAllAccounts()
                .Where(a => !a.AccountId.Equals(accountId))
                .ToList();

            _accountRepository.SaveAll(accounts);
        }
    }
}