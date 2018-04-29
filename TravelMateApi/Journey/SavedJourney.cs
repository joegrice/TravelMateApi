using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TravelMateApi.Database;
using TravelMateApi.Models;

namespace TravelMateApi.Journey
{
    public class SavedJourney
    {
        private readonly string _uid;

        public SavedJourney(string uid)
        {
            _uid = uid;
        }

        public List<Models.Journey> GetUserSavedJourneys()
        {
            var databaseFactory = new DatabaseFactory();
            var dbJourneys = databaseFactory.GetJourneysForUid(_uid);

            var journeys = new List<Models.Journey>();
            foreach (var dbJourney in dbJourneys)
            {
                var journey = BuildSavedJourney(dbJourney, databaseFactory);
                journeys.Add(journey);
            }

            return journeys;
        }

        private static Models.Journey BuildSavedJourney(DbJourney dbJourney, DatabaseFactory databaseFactory)
        {
            var journey = new Models.Journey
            {
                id = dbJourney.Id,
                name = dbJourney.Name,
                from = dbJourney.StartLocation,
                to = dbJourney.EndLocation,
                time = dbJourney.Time,
                period = dbJourney.Period
            };
            var deserializeObject = JsonConvert.DeserializeObject<Route>(dbJourney.Route);
            if (deserializeObject != null)
            {
                var arr = new[] { deserializeObject };
                journey.routes = arr;
            }

            var delayedLines = databaseFactory.GetJourneyDelayedLines(dbJourney.Id);
            if (delayedLines.Any())
            {
                journey.status = JourneyStatus.Delayed;
                journey.disruptedLines = delayedLines.ToArray();
                /*var searchJourney = new SearchJourney(dbJourney.StartLocation, dbJourney.EndLocation);
                journey.routes = searchJourney.Search().routes;*/
            }
            else
            {
                journey.status = JourneyStatus.GoodService;
            }

            return journey;
        }
    }
}
