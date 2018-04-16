using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Configuration;
using ConsoleApp1.Helpers;
using System.Text;

namespace ConsoleApp1
{
    class Program
    {
        private static int NumberOfLastTweetsForStatistic = 5;

        static void Main(String[] args)
        {
            var consumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
            var consumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];
            var userAccessToken = ConfigurationManager.AppSettings["UserAccessToken"];
            var userAccessSecret = ConfigurationManager.AppSettings["UserAccessSecret"];

            var twitterHelper = new TweetterHelper(NumberOfLastTweetsForStatistic);
            twitterHelper.SetCredentials(consumerKey, consumerSecret, userAccessToken, userAccessSecret);
            if (!twitterHelper.IsCredentialsCorrect())
            {
                Console.WriteLine("Incorrect credentials!");
                return;
            }

            //at least one letter, nubers and "_" allowed, min 5 and max 15 length
            const string twitterNameRegex = @"(?=.*[a-zA-Z])[a-zA-Z0-9](\w){4,14}$";
            var usernameRegex = new Regex(twitterNameRegex);

            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Enter usernames: ");

            while (true)
            {
                var username = Console.ReadLine();
                if (string.IsNullOrEmpty(username))
                    break;

                if (!IsUsernameCorrect(usernameRegex, username))
                {
                    Console.WriteLine("Incorrect username!");
                    continue;
                }
                
                PostStatisticResult(twitterHelper, username);
            };

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static void PostStatisticResult(TweetterHelper twitterHelper, string name)
        {
            var statisticResult = twitterHelper.GetStatisticForAUser(name);

            if (!statisticResult.IsSuccess)
            {
                Console.WriteLine(statisticResult.ErrorMessage);
                return;
            }

            var displayUsername = name.StartsWith("@") ? name : "@" + name;

            Console.WriteLine(displayUsername + ", статистика для последних " + NumberOfLastTweetsForStatistic + " твитов: " + statisticResult.Statistic);
        }

        private static bool IsUsernameCorrect(Regex regex, string name)
        {
            var match = regex.Match(name);
            return match.Success;
        }
    }    
}
