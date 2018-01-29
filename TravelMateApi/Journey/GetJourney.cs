using Newtonsoft.Json;
using TravelMateApi.Connection;
using TravelMateApi.Models;

namespace TravelMateApi.Journey
{
    public class GetJourney {

        private readonly string BaseUrl = "https://api.tfl.gov.uk";
        private readonly string _startLocation;
        private readonly string _endLocation;

        public GetJourney(string startLocation, string endLocation) {
            this._startLocation = startLocation;
            this._endLocation = endLocation;
        }

        public string SearchJourney() {
            var apiConnect = new ApiConnect();
            var url = $@"{BaseUrl}/Journey/JourneyResults/{_startLocation}/to/{_endLocation}";
            var json = apiConnect.GetJson(url);
            var result = JsonConvert.DeserializeObject<JourneySearch>(json.Result);
            var serializeObject = JsonConvert.SerializeObject(result);
            return serializeObject;
        }
    }
}
