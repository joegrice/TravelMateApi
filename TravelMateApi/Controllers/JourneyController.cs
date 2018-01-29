using Microsoft.AspNetCore.Mvc;
using TravelMateApi.Journey;

namespace TravelMateApi.Controllers
{
    [Route("api/[controller]")]
    public class JourneyController : Controller
    {
        [HttpGet("search/{startlocation}/{endlocation}")]
        public string Search(string startlocation, string endlocation)
        {
            var getJourney = new GetJourney(startlocation, endlocation);
            var resultSearchJourney = getJourney.SearchJourney();
            return resultSearchJourney;
        }
    }
}
