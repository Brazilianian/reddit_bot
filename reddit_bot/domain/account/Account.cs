namespace reddit_bor.domain
{
    public class Account
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public AccountStatus Status { get; set; }
    }
}