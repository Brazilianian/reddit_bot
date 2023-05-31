namespace reddit_bot.domain
{
    public class RedditAccount
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public RedditAccountStatus Status { get; set; }

        public override string ToString()
        {
            return $"username - {Username} " +
                   $"password - {Password} " +
                   $"access - {AccessToken} " +
                   $"refresh - {RefreshToken} ";
        }
    }
}