using System.Diagnostics;
using Reddit;
using reddit_bot.domain;
using Reddit.AuthTokenRetriever;
using Reddit.AuthTokenRetriever.EventArgs;

namespace reddit_bot.service
{
    public class RedditService
    {
        private AuthTokenRetrieverLib _authTokenRetrieverLib;
        private string _accessToken;
        private string _refreshToken;

        public RedditClient GetRedditClient(RedditAccount redditAccount, string userAgent)
        {
            return new RedditClient(
                redditAccount.AppId,
                redditAccount.RefreshToken,
                redditAccount.Secret,
                redditAccount.AccessToken,
                userAgent);
        }

        public void Login(string redditId, string secret)
        {
            _authTokenRetrieverLib = new AuthTokenRetrieverLib(redditId, secret, 8080);
            _authTokenRetrieverLib.AwaitCallback();
            _authTokenRetrieverLib.AuthSuccess += LoginSuccess;
            OpenBrowser(_authTokenRetrieverLib.AuthURL());
        }

        private void LoginSuccess(object sender, AuthSuccessEventArgs e)
        {
            _accessToken = e.AccessToken;
            _refreshToken = e.RefreshToken;
        }

        //TODO open new browser and close it
        private void OpenBrowser(string authUrl)
        {
            var processStartInfo = new ProcessStartInfo(authUrl);
            Process.Start(processStartInfo);
        }

        public string GetAccessToken()
        {
            return _authTokenRetrieverLib.AccessToken;
        }

        public string GetRefreshToken()
        {
            return _authTokenRetrieverLib.RefreshToken;
        }
    }
}