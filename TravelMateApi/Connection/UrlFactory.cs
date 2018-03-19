using System.Collections.Generic;
using System.Net;
using Microsoft.EntityFrameworkCore.Internal;

namespace TravelMateApi.Connection
{
    public static class UrlFactory
    {
        private const string TflBaseUrl = "https://api.tfl.gov.uk";

        public static string DisruptionsForGivenModes(IEnumerable<string> ids)
        {
            var idsString = ids.Join(",");
            var escapedIds = WebUtility.UrlEncode(idsString);
            return $"{TflBaseUrl}/Line/Mode/{escapedIds}/Disruption";
        }

        public static string DisruptionsForGivenLineIds(IEnumerable<string> ids)
        {
            var idsString = ids.Join(",");
            var escapedIds = WebUtility.UrlEncode(idsString);
            return $"{TflBaseUrl}/Line/{escapedIds}/Disruption";
        }

        public static string GetJourneys(string startLocation, string endLocation)
        {
            return $@"{TflBaseUrl}/Journey/JourneyResults/{startLocation}/to/{endLocation}";
        }

        public static string GetGoogleJourneys(string startLocation, string endLocation)
        {
            const string directionsBaseUrl = "https://maps.googleapis.com/maps/api/directions/";
            return $@"{directionsBaseUrl}json?origin={startLocation}&destination={endLocation}&mode=transit&alternatives=true&key={Credentials.GoogleApiKey}";
        }
    }
}
