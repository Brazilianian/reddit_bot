using MySql.Data.MySqlClient;

namespace reddit_bot.database
{
    public static class DatabaseConnector
    {
        private static string _connectionString = "server=localhost;uid=root;pwd=Master05072003;database=reddit_bot";
        private static MySqlConnection _connection = new MySqlConnection(_connectionString);

        public static MySqlConnection GetConnection()
        {
            return _connection;
        }
    }
}