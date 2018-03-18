using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Quartz;
using TravelMateApi.Connection;
using TravelMateApi.Database;
using TravelMateApi.Models;

namespace TravelMateApi.Scheduler
{
    public class DisruptionChecker : IJob
    {
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
            var databaseFactory = new DatabaseFactory();
            var dbLines = databaseFactory.GetLines();

            var apiConnect = new ApiConnect();
            var savedIds = dbLines.Select(line => line.LineId).ToList();
            //var url = UrlFactory.DisruptionsForGivenLineIds(ids);
            //var json = apiConnect.GetJson(url);
            //var result = JsonConvert.DeserializeObject<LineDisruption[]>(json.Result);
            var url = UrlFactory.DisruptionsForGivenModes(_modes);
            var json = apiConnect.GetJson(url);
            var result = JsonConvert.DeserializeObject<LineDisruption[]>(json.Result);



            /*var realTimeDisruptions =
                result.Where(disruption => disruption.Category.Equals(DisruptionCategories.RealTime)
                && savedIds.Exists(id => Regex.IsMatch(disruption.Description, @"\b" + id + @"\b", RegexOptions.IgnoreCase))).ToList();*/


            var realTimeDisruptions = new List<RealTimeDisruption>();
            foreach (var lineDisruption in result)
            {
                if (lineDisruption.Category.Equals(DisruptionCategories.RealTime))
                {
                    foreach (var savedId in savedIds)
                    {
                        if (Regex.IsMatch(lineDisruption.Description, @"\b" + savedId + @"\b", RegexOptions.IgnoreCase))
                        {
                            var realTimeDisruption = new RealTimeDisruption
                            {
                                ModeId = savedId,
                                Description = lineDisruption.Description
                            };
                            realTimeDisruptions.Add(realTimeDisruption);
                        }
                    }
                }
            }

            var tokensForNotifications = databaseFactory.GetDisruptionTokens(realTimeDisruptions);
            foreach (var tokenForNotification in tokensForNotifications)
            {
                var account = databaseFactory.GetAccountByUid(tokenForNotification.Uid);
                tokenForNotification.Token = account.Token;
            }

            if (tokensForNotifications.Any())
            {
                foreach (var tokenForNotification in tokensForNotifications)
                {
                    var sendResult = SendNotificationFromFirebaseCloud(tokenForNotification.Token, tokenForNotification.Description,
                        realTimeDisruptions[0].Description);
                    Console.WriteLine(sendResult);
                }
            }

            Console.WriteLine(DateTime.Now.ToString(CultureInfo.InvariantCulture));

            return Task.FromResult(0);
        }

        private string SendNotificationFromFirebaseCloud(string token, string title, string desc)
        {
            var result = "-1";
            const string webAddr = "https://fcm.googleapis.com/fcm/send";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "key=" + Credentials.FirebaseServerKey);
            httpWebRequest.Method = "POST";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                var data = new AData(desc, desc);
                var notification = new ANotification(title);
                var androidNotification = new AndroidNotification(token, data, notification);
                var androidNotificationJson = JsonConvert.SerializeObject(androidNotification);
                streamWriter.Write(androidNotificationJson);
                streamWriter.Flush();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            return result;
        }
    }
}
