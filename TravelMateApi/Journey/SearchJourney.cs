using Newtonsoft.Json;
using TravelMateApi.Connection;
using TravelMateApi.Models;

namespace TravelMateApi.Journey
{
    public class SearchJourney
    {
        private readonly string _startLocation;
        private readonly string _endLocation;

        public SearchJourney(string startLocation, string endLocation)
        {
            _startLocation = startLocation;
            _endLocation = endLocation;
        }

        public GJourney Search()
        {
            var apiConnect = new ApiConnect();
            var url = UrlFactory.GetGoogleJourneys(_startLocation, _endLocation);
            var json = apiConnect.GetJson(url);
            return JsonConvert.DeserializeObject<GJourney>(json.Result);
        }
    }
}