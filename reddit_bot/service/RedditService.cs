﻿using System.Diagnostics;
using Reddit;
using reddit_bot.domain;
using Reddit.AuthTokenRetriever;
using Reddit.AuthTokenRetriever.EventArgs;
using System;
using reddit_bot.util;
using System.IO;

namespace reddit_bot.service
{
    public class RedditService
    {
        private AuthTokenRetrieverLib _authTokenRetrieverLib;

        private const int port = 8080;

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
            _accessToken = null;
            _refreshToken = null;

            _authTokenRetrieverLib = new AuthTokenRetrieverLib(redditId, secret, 8080);

            //FIXME and this shoud be removed after fixing the proccess killing
            try
            {
                _authTokenRetrieverLib.AwaitCallback();
                _authTokenRetrieverLib.AuthSuccess += LoginSuccess;
                OpenBrowser(_authTokenRetrieverLib.AuthURL());
            }
            catch (Exception ex)
            {
                using (StreamWriter streamWriter = new StreamWriter("./data/errors.txt", true))
                {
                    streamWriter.WriteLine(ex.Message);
                }
            }

        }

        private void LoginSuccess(object sender, AuthSuccessEventArgs e)
        {
            _accessToken = e.AccessToken;
            _refreshToken = e.RefreshToken;
        }


        //FIXME im not working. Killing the proccess stops the whole program. why?
        public void StopListen()
        {
            if (_authTokenRetrieverLib != null)
            {
                _authTokenRetrieverLib.StopListening();
            }

            //ProccessUtil.KillByPort(port);

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