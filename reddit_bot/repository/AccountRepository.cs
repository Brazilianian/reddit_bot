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
                $"(username, password, access_token, refresh_token) " +
                $"VALUES (@username, @password, @access_token, @refresh_token)";

            var command = new MySqlCommand(query, _mySqlConnection);

            command.Parameters.AddWithValue("username", account.Username);
            command.Parameters.AddWithValue("password", account.Password);
            command.Parameters.AddWithValue("access_token", account.AccessToken);
            command.Parameters.AddWithValue("refresh_token", account.RefreshToken);

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

        public RedditAccount FindByUsername(string username)
        {
            var query = $"SELECT * FROM {AccountTableName} WHERE username=@username";
            var command = new MySqlCommand(query, _mySqlConnection);
            command.Parameters.AddWithValue("username", username);
          
            _mySqlConnection.Open();
            var reader = command.ExecuteReader();
            reader.Read();
            var account = GetRedditAccountFromMySqlDataReader(reader);
            _mySqlConnection.Close();
         
            return account;
        }

        private static RedditAccount GetRedditAccountFromMySqlDataReader(IDataRecord mySqlDataReader)
        {
            var redditAccount = new RedditAccount
            {
                Id = int.Parse(mySqlDataReader["id"].ToString()),
                Username = mySqlDataReader["username"].ToString(),
                Password = mySqlDataReader["password"].ToString(),
                AccessToken = mySqlDataReader["access_token"].ToString(),
                RefreshToken = mySqlDataReader["refresh_token"].ToString()
            };
            return redditAccount;
        }
    }
}