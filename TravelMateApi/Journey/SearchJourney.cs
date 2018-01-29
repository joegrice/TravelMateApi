using Newtonsoft.Json;
using TravelMateApi.Connection;
using TravelMateApi.Models;

namespace TravelMateApi.Journey
{
    public class SearchJourney
    {
        private const string BaseUrl = "https://api.tfl.gov.uk";
        private readonly string _startLocation;
        private readonly string _endLocation;

        public SearchJourney(string startLocation, string endLocation)
        {
            _startLocation = startLocation;
            _endLocation = endLocation;
        }

        public string Search()
        {
            var apiConnect = new ApiConnect();
            var url = $@"{BaseUrl}/Journey/JourneyResults/{_startLocation}/to/{_endLocation}";
            var json = apiConnect.GetJson(url);
            var result = JsonConvert.DeserializeObject<JourneySearch>(json.Result);
            var serializeObject = JsonConvert.SerializeObject(result);
            return serializeObject;
        }
    }
}
