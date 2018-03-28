using Microsoft.AspNetCore.Mvc;
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
            return resultSearchJourney;
        }

        [HttpPut("select")]
        public void Search([FromQuery] string uid, [FromQuery] string route, [FromQuery] string startlocation, [FromQuery] string endlocation)
        {
            var getJourney = new SelectJourney(uid, route, startlocation, endlocation);
            getJourney.Select();
        }
    }
}
