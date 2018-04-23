using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using Newtonsoft.Json;
using TravelMateApi.Database;
using TravelMateApi.Models;

namespace TravelMateApi.Journey
{
    public class SelectJourney
    {
        private readonly string _uid;
        private readonly string _name;
        private readonly string _route;
        private readonly string _startLocation;
        private readonly string _endLocation;
        private readonly string _time;
        private readonly string _period;
        private DbJourney _dbJourney;

        public SelectJourney(string uid, string name, string route, string startLocation, string endLocation,
            string time,
            string period)
        {
            _uid = uid;
            _name = name;
            _route = route;
            _startLocation = startLocation;
            _endLocation = endLocation;
            _time = time;
            _period = period;
        }

        public void Select()
        {
            SaveJourney();
            var route = JsonConvert.DeserializeObject<GRoute>(_route);
            var lines = new List<string>();
            foreach (var step in route.legs[0].steps)
            {
                if (step.transit_details?.line != null)
                {
                    var name = !step.transit_details.line.short_name.Equals("null") ?
                        step.transit_details.line.short_name : step.transit_details.line.name;
                    lines.Add(name);
                }
            }

            UpdateLines(lines);
        }

        private void SaveJourney()
        {
            var databaseFactory = new DatabaseFactory();
            var dbAccount = databaseFactory.GetAccountByUid(_uid);
            _dbJourney = new DbJourney
            {
                AccountId = dbAccount.Id,
                Name = _name,
                Route = _route,
                StartLocation = _startLocation,
                EndLocation = _endLocation,
                Time = _time,
                Period = _period
            };
            databaseFactory.SaveJourneyToDb(_dbJourney);
            // Get journey from database to access journeyId
            _dbJourney = databaseFactory.GetJourneyFromDb(_dbJourney);
        }

        private void UpdateLines(IEnumerable<string> lines)
        {
            lines = lines.ToList();
            var databaseFactory = new DatabaseFactory();
            var dbLines = lines.Select(line => new DbLine { Name = line, IsDelayed = JourneyStatus.GoodService });
            databaseFactory.SaveLines(dbLines);
            var dbJourneyLines = lines.Select(line => new DbJourneyLine
            {
                JourneyId = _dbJourney.Id,
                LineId = databaseFactory.GetLine(line).Id,
                Notified = false.ToString()
            });
            databaseFactory.SaveJourneyLines(dbJourneyLines);
        }
    }
}