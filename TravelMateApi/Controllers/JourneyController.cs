using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TravelMateApi.Journey;

namespace TravelMateApi.Controllers
{
    [Route("api/[controller]")]
    public class JourneyController : Controller
    {
        [HttpGet("search")]
        public string Search([FromQuery] string startlocation, [FromQuery] string endlocation)
        {
            var getJourney = new SearchJourney(startlocation, endlocation);
            var resultSearchJourney = getJourney.Search();
            var json = JsonConvert.SerializeObject(resultSearchJourney);
            return json;
        }

        [HttpPut("select")]
        public void Select([FromQuery] string uid, [FromQuery] string name, [FromQuery] string route, [FromQuery] string startlocation, [FromQuery] string endlocation, [FromQuery] string time, [FromQuery] string period)
        {
            var getJourney = new SelectJourney(uid, name, route, startlocation, endlocation, time, period);
            getJourney.Select();
        }

        [HttpGet("saved")]
        public string Journeys([FromQuery] string uid)
        {
            var journeys = new SavedJourney(uid);
            var json = JsonConvert.SerializeObject(journeys.GetUserSavedJourneys());
            return json;
        }

        [HttpDelete("delete")]
        public void Delete([FromQuery] string id)
        {
            var deleteJourney = new DeleteJourney(id);
            deleteJourney.Delete();
        }
    }
}
