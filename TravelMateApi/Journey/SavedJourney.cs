using System.Collections.Generic;
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

        public List<GJourney> GetUserSavedJourneys()
        {
            var databaseFactory = new DatabaseFactory();
            var dbJourneys = databaseFactory.GetJourneysForUid(_uid);

            var journeys = new List<GJourney>();
            foreach (var dbJourney in dbJourneys)
            {
                var journey = BuildSavedJourney(dbJourney, databaseFactory);
                journeys.Add(journey);
            }

            return journeys;
        }

        private static GJourney BuildSavedJourney(DbJourney dbJourney, DatabaseFactory databaseFactory)
        {
            var journey = new GJourney
            {
                from = dbJourney.StartLocation,
                to = dbJourney.EndLocation,
                time = dbJourney.Time,
                period = dbJourney.Period
            };
            var deserializeObject = JsonConvert.DeserializeObject<GRoute>(dbJourney.Route);
            if (deserializeObject != null)
            {
                var arr = new[] { deserializeObject };
                journey.routes = arr;
            }

            // TODO: USE TO POPULATE STATUS
            var x = databaseFactory.GetLinesForJourneyId(dbJourney.Id);

            return journey;
        }
    }
}
