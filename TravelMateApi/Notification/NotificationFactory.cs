using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using TravelMateApi.Models;

namespace TravelMateApi.Notification
{
    public class NotificationFactory
    {
        public string SendNotification(AndroidMessage androidMessage)
        {
            string result;

            try
            {
                const string webAddr = "https://fcm.googleapis.com/fcm/send";
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Headers.Add(string.Format("Authorization: key={0}", Credentials.FirebaseServerKey));
                httpWebRequest.Headers.Add(string.Format("Sender: id={0}", Credentials.GoogleProjectNumber));
                httpWebRequest.Method = "POST";
                var androidNotificationJson = JsonConvert.SerializeObject(androidMessage);
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(androidNotificationJson);
                    streamWriter.Flush();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream() ?? throw new InvalidOperationException()))
                {
                    result = streamReader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }
    }
}
