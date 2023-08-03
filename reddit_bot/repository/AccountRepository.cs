using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using reddit_bot.domain;

namespace reddit_bot.repository
{
    public class AccountRepository
    {
        private const string _filePath = "./data/accounts.json";
        private readonly JsonSerializer jsonSerializer;

        public AccountRepository() 
        {
            jsonSerializer = new JsonSerializer();

            if (!File.Exists(_filePath))
            {
                using (FileStream fs = File.Create(_filePath)) 
                {
                }
                SaveAll(new List<RedditAccount>());
            }
        }

        public RedditAccount Save(RedditAccount account)
        {
            List<RedditAccount> accounts = FindAll();
            if (accounts.Where(a => a.AccountId == account.AccountId).Count() != 0)
            {
                return null;
            }

            accounts.Add(account);
            SaveAll(accounts);
            return account;
        }

        public List<RedditAccount> FindAll()
        {
            List<RedditAccount> accounts = new List<RedditAccount>();
            using (StreamReader streamReader = new StreamReader(_filePath))
            {
                accounts = (List<RedditAccount>)jsonSerializer.Deserialize(streamReader, typeof(List<RedditAccount>));
            }
            return accounts;
        }

        public void SaveAll(List<RedditAccount> accounts)
        {
            using (StreamWriter streamWriter = new StreamWriter(_filePath)) 
            {
                jsonSerializer.Serialize(streamWriter, accounts);
            }
        }

        public RedditAccount FindByAccountId(string accountId)
        {
            return FindAll()
                .Where(a => a.AccountId == accountId)
                .FirstOrDefault();
        }
    }
}