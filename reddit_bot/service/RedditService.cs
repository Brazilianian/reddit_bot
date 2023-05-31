using System.ComponentModel;
using Reddit;
using Reddit.AuthTokenRetriever;
using System.Diagnostics;

namespace reddit_bot.service
{
    public class RedditService
    {
        private string _id = "7coQmKRHRLtwEIf3-jyQCQ";
        private string _secret = "sXD-8Sk-tzvmpwPBk1zAmFyZuplOHQ";

        private RedditClient _redditClient;

        public RedditService()
        {
        }

        public void Login(string username, string password)
        {
            var authTokenRetrieverLib = new AuthTokenRetrieverLib(_id, _secret);
            authTokenRetrieverLib.AwaitCallback();
            OpenBrowser(authTokenRetrieverLib.AuthURL());
        }

        private void OpenBrowser(string authUrl)
        {
            try
            {
                var processStartInfo = new ProcessStartInfo(authUrl);
                Process.Start(processStartInfo);
            }
            catch (Win32Exception)
            {
                var processStartInfo = new ProcessStartInfo()
                {
                    Arguments = authUrl
                };
                Process.Start(processStartInfo);
            }
        }
    }
}