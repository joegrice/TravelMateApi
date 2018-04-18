using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Quartz;
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
            // Get saved lines to search for
            var dbLines = _databaseFactory.GetLines().ToList();

            /*var apiConnect = new ApiConnect();
            var url = UrlFactory.DisruptionsForGivenModes(_modes);
            var json = apiConnect.GetJson(url);?*/
            var path = Environment.CurrentDirectory + @"\Data\sample-disruption-data.json";
            var json = File.ReadAllText(path);
            //var result = JsonConvert.DeserializeObject<LineDisruption[]>(json.Result);
            var result = JsonConvert.DeserializeObject<LineDisruption[]>(json);
            var realTimeDisruptions = result.Where(lineDisruption =>
                lineDisruption.Category.Equals(DisruptionCategories.RealTime)).ToList();
            var updatedLines = UpdateDelayedLines(dbLines, realTimeDisruptions);
            UpdateGoodStatusLines(updatedLines);
            SendNotificationsForDelayedLines();

            return Task.FromResult(0);
        }

        private void SendNotificationsForDelayedLines()
        {
            var accounts = _databaseFactory.GetDisruptionNotificationsDetails();
            //_databaseFactory.MarkUsersNotifiedForLine(11, 2);
            var notificationFactory = new NotificationFactory();
            foreach (var account in accounts)
            {
                var androidMessage = new AndroidMessage(account.Item1, account.Item4);
                var result = notificationFactory.SendNotification(androidMessage);
                Console.WriteLine(result);
                _databaseFactory.MarkUsersNotifiedForLine(account.Item2, account.Item3);
            }
        }

        private List<DbLine> UpdateDelayedLines(IEnumerable<DbLine> dbLines,
            IEnumerable<LineDisruption> lineDisruptions)
        {
            var updatedLines = new List<DbLine>();
            foreach (var lineDisruption in lineDisruptions)
            {
                foreach (var dbLine in dbLines)
                {
                    if (CheckDisruptionForLine(lineDisruption, dbLine))
                    {
                        dbLine.IsDelayed = JourneyStatus.Delayed;
                        dbLine.Description = lineDisruption.Description;
                        _databaseFactory.UpdateLineDelayDetails(dbLine);
                        updatedLines.Add(dbLine);
                    }
                }
            }

            return updatedLines;
        }

        private void UpdateGoodStatusLines(List<DbLine> dbLines)
        {
            var lineIds = dbLines.Select(line => line.Id).ToList();
            _databaseFactory.UpdateGoodStatusLineDelayDetails(lineIds);
        }

        private bool CheckDisruptionForLine(LineDisruption lineDisruption, DbLine dbLine)
        {
            var isFound = Regex.IsMatch(lineDisruption.Description, @"\b" + dbLine.Name + @"\b",
                RegexOptions.IgnoreCase);
            return isFound;
        }
    }
}