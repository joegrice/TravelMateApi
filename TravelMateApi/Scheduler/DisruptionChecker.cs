using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MoreLinq;
using Newtonsoft.Json;
using Quartz;
using TravelMateApi.Connection;
using TravelMateApi.Database;
using TravelMateApi.Models;
using TravelMateApi.Notification;

namespace TravelMateApi.Scheduler
{
    public class DisruptionChecker : IJob
    {
        private DatabaseFactory _databaseFactory;

        private readonly List<string> _modes = new List<string>
        {
            "bus",
            "coach",
            "cycle",
            "dlr",
            "national-rail",
            "overground",
            "replacement-bus",
            "tflrail",
            "tram",
            "tube",
            "walking",
            "taxi"
        };

        public Task Execute(IJobExecutionContext context)
        {
            _databaseFactory = new DatabaseFactory();
            var dbLines = _databaseFactory.GetLines();

            /*var apiConnect = new ApiConnect();
            var url = UrlFactory.DisruptionsForGivenModes(_modes);
            var json = apiConnect.GetJson(url);?*/
            var path = Environment.CurrentDirectory + @"\Data\sample-disruption-data.json";
            var json = File.ReadAllText(path);
            //var result = JsonConvert.DeserializeObject<LineDisruption[]>(json.Result);
            var result = JsonConvert.DeserializeObject<LineDisruption[]>(json);
            var realTimeDisruptions = GetRealTimeDisruptions(dbLines, result);
            var finalTokensForNotifications = GetNotificationTokens(realTimeDisruptions);

            if (finalTokensForNotifications.Any())
            {
               NotifyUsers(finalTokensForNotifications);
            }
            else
            {
                Console.WriteLine("JOB UPDATE: No Notifications Sent");
            }

            return Task.FromResult(0);
        }

        private static void NotifyUsers(List<RealTimeDisruption> finalTokensForNotifications)
        {
            var notificationFactory = new NotificationFactory();
            foreach (var tokenForNotification in finalTokensForNotifications)
            {
                var androidMessage = new AndroidMessage(tokenForNotification.Token,
                    tokenForNotification.Description);
                var sendResult = notificationFactory.SendNotification(androidMessage);
                Console.WriteLine("JOB UPDATE: Notification Result - " + sendResult);
            }
        }

        private List<RealTimeDisruption> GetNotificationTokens(IEnumerable<RealTimeDisruption> realTimeDisruptions)
        {
            var tokensForNotifications = _databaseFactory.GetDisruptionTokens(realTimeDisruptions).ToList();
            foreach (var tokenForNotification in tokensForNotifications)
            {
                var account = _databaseFactory.GetAccountByJourneyId(tokenForNotification.JourneyId);
                tokenForNotification.Token = account.Token;
            }

            return tokensForNotifications.DistinctBy(token => new { token.Token, token.LineId }).ToList();
        }

        private static List<RealTimeDisruption> GetRealTimeDisruptions(IEnumerable<DbLine> dbLines, IEnumerable<LineDisruption> result)
        {
            var realTimeDisruptions = new List<RealTimeDisruption>();
            var dbLinesList = dbLines.ToList();
            foreach (var lineDisruption in result)
            {
                if (lineDisruption.Category.Equals(DisruptionCategories.RealTime))
                {
                    foreach (var dbLine in dbLinesList)
                    {
                        CreateRealTimeDisruption(realTimeDisruptions, lineDisruption, dbLine);
                    }
                }
            }

            return realTimeDisruptions;
        }

        private static void CreateRealTimeDisruption(List<RealTimeDisruption> realTimeDisruptions, LineDisruption lineDisruption, DbLine dbLine)
        {
            if (Regex.IsMatch(lineDisruption.Description, @"\b" + dbLine.Name + @"\b",RegexOptions.IgnoreCase))
            {
                var realTimeDisruption = new RealTimeDisruption
                {
                    LineId = dbLine.Id,
                    Description = lineDisruption.Description
                };
                realTimeDisruptions.Add(realTimeDisruption);
            }
        }
    }
}