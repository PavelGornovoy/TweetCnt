using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace ConsoleApp1.Helpers
{
    public class TweetterHelper
    {
        private int NuberOfLastTweetsToGet { get; set; }

        public TweetterHelper(int numberOfLastTweetsToGet)
        {
            NuberOfLastTweetsToGet = numberOfLastTweetsToGet;
        }

        private TwitterCredentials Credentials { get; set; }

        public void SetCredentials(string consumerKey, string consumerSecret, string userAccessToken, string userAccessSecret)
        {
            Credentials = new TwitterCredentials(consumerKey, consumerSecret, userAccessToken, userAccessSecret);
            Auth.SetCredentials(Credentials);
        }

        public bool IsCredentialsCorrect()
        {
            var authenticatedUser = User.GetAuthenticatedUser(Credentials);
            return authenticatedUser != null;
        }

        public StatisticResult GetStatisticForAUser(string username)
        {
            var user = User.GetUserFromScreenName(username);
            
            if (user == null)
            {
                return new StatisticResult
                {
                    IsSuccess = false,
                    ErrorMessage = "User not found!"
                };
            }
            
            var searchResult = Timeline.GetUserTimeline(user, NuberOfLastTweetsToGet);

            if (searchResult == null || !searchResult.Any())
            {
                return new StatisticResult
                {
                    IsSuccess = false,
                    ErrorMessage = "No Results found!"
                };
            }

            var lastTweets = searchResult
                .Select(t => t.FullText.ToLower())
                .Aggregate((current, next) => current + next);

            var lastTweetsLettersOnly = lastTweets.Where(Char.IsLetter);
            var allLettersCount = (double)lastTweetsLettersOnly.Count();

            var statistic = JsonConvert.SerializeObject(lastTweetsLettersOnly
                .GroupBy(c => c)
                .OrderBy(t => t.Key)
                .ToDictionary(
                    c => c.Key,
                    c => (c.Count() / allLettersCount)
                    ));

            return new StatisticResult
            {
                IsSuccess = true,
                Statistic = statistic
            };            
        }
    }
}
