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
            var getJourney = new SearchJourney(startlocation, endlocation);
            var resultSearchJourney = getJourney.Search();
            return resultSearchJourney;
        }

        [HttpGet("select/{pos}/{startlocation}/{endlocation}")]
        public string Search(int pos, string startlocation, string endlocation)
        {
            var getJourney = new SelectJourney(pos, startlocation, endlocation);
            var resultSearchJourney = getJourney.Select();
            return resultSearchJourney;
        }
    }
}
