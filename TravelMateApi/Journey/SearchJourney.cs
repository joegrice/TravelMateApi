using Newtonsoft.Json;
using TravelMateApi.Connection;
using TravelMateApi.Models;

namespace TravelMateApi.Journey
{
    public class SearchJourney
    {
        private readonly IApiConnect _apiConnect;
        private readonly string _startLocation;
        private readonly string _endLocation;

        public SearchJourney(IApiConnect apiConnect, string startLocation, string endLocation)
        {
            _startLocation = startLocation;
            _endLocation = endLocation;
            _apiConnect = apiConnect;
        }

        public Models.Journey Search()
        {
            var url = UrlFactory.GetGoogleJourneys(_startLocation, _endLocation);
            var json = _apiConnect.GetJson(url);
            return JsonConvert.DeserializeObject<Models.Journey>(json.Result);
        }
    }
}