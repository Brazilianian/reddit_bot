namespace reddit_bot.domain
{
    public class RedditAccount
    {
        public int Id { get; set; }
        public string AccountId { get; set; }
        public string AppId { get; set; }
        public string Secret { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public RedditAccountStatus Status { get; set; }

        public override string ToString()
        {
            return $"account id - {AccountId} " +
                   $"access - {AccessToken} " +
                   $"refresh - {RefreshToken} ";
        }
    }
}