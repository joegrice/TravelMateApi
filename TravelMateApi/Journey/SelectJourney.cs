using Newtonsoft.Json;
using TravelMateApi.Connection;
using TravelMateApi.Models;

namespace TravelMateApi.Journey
{
    public class SelectJourney {

        private readonly string BaseUrl = "https://api.tfl.gov.uk";
        private readonly int _pos;
        private readonly string _startLocation;
        private readonly string _endLocation;

        public SelectJourney(int pos, string startLocation, string endLocation) {
            _pos = pos;
            _startLocation = startLocation;
            _endLocation = endLocation;
        }

        public string Select() {
            var apiConnect = new ApiConnect();
            var url = $@"{BaseUrl}/Journey/JourneyResults/{_startLocation}/to/{_endLocation}";
            var json = apiConnect.GetJson(url);
            var result = JsonConvert.DeserializeObject<JourneySearch>(json.Result);
            var selectedResult = result.Journeys[_pos];
            var serializeObject = JsonConvert.SerializeObject(selectedResult);
            return serializeObject;
        }
    }
}
