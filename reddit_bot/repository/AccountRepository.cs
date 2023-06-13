using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using reddit_bot.database;
using reddit_bot.domain;

namespace reddit_bot.repository
{
    public class AccountRepository
    {
        private readonly MySqlConnection _mySqlConnection;
        private const string AccountTableName = "accounts";

        public AccountRepository()
        {
            _mySqlConnection = DatabaseConnector.GetConnection();
        }

        public void Save(RedditAccount account)
        {
            var query =
                $"INSERT INTO {AccountTableName} " +
                $"(secret, access_token, refresh_token, app_id, account_id) " +
                $"VALUES (@secret, @access_token, @refresh_token, @app_id, @account_id)";
        
            var command = new MySqlCommand(query, _mySqlConnection);
        
            command.Parameters.AddWithValue("secret", account.Secret);
            command.Parameters.AddWithValue("access_token", account.AccessToken);
            command.Parameters.AddWithValue("refresh_token", account.RefreshToken);
            command.Parameters.AddWithValue("app_id", account.AppId);
            command.Parameters.AddWithValue("account_id", account.AccountId);
        
            _mySqlConnection.Open();
            command.ExecuteNonQuery();
            _mySqlConnection.Close();
        }

        public List<RedditAccount> FindAll()
        {
            var query = $"SELECT * FROM {AccountTableName}";
            var command = new MySqlCommand(query, _mySqlConnection);
        
            var redditAccounts = new List<RedditAccount>();
        
            _mySqlConnection.Open();
            var mySqlDataReader = command.ExecuteReader();
            while (mySqlDataReader.Read())
            {
                redditAccounts.Add(GetRedditAccountFromMySqlDataReader(mySqlDataReader));
            }
        
            _mySqlConnection.Close();
        
            return redditAccounts;
        }

        public RedditAccount FindByAccountId(string accountId)
        {
            var query = $"SELECT * FROM {AccountTableName} WHERE account_id=@account_id";
            var command = new MySqlCommand(query, _mySqlConnection);
            command.Parameters.AddWithValue("account_id", accountId);
          
            _mySqlConnection.Open();
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var redditAccount = GetRedditAccountFromMySqlDataReader(reader);
                _mySqlConnection.Close();
                return redditAccount;
            }
            
            _mySqlConnection.Close();
            return null;
        }

        private static RedditAccount GetRedditAccountFromMySqlDataReader(IDataRecord mySqlDataReader)
        {
            var redditAccount = new RedditAccount
            {
                Id = int.Parse(mySqlDataReader["id"].ToString()),
                AccountId = mySqlDataReader["account_id"].ToString(),
                Secret = mySqlDataReader["secret"].ToString(),
                AppId = mySqlDataReader["app_id"].ToString(),
                AccessToken = mySqlDataReader["access_token"].ToString(),
                RefreshToken = mySqlDataReader["refresh_token"].ToString()
            };
            return redditAccount;
        }
    }
}