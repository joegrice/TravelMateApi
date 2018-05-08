using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TravelMateApi.Database;
using TravelMateApi.Models;

namespace TravelMateApi.Journey
{
    public class SelectJourney
    {
        private readonly string _uid;
        private readonly DbJourney _inputJourney;
        private DbJourney _returnJourney;
        private readonly IDatabaseFactory _databaseFactory;

        public SelectJourney(IDatabaseFactory databaseFactory, string uid, DbJourney inputJourney)
        {
            _inputJourney = inputJourney;
            _uid = uid;
            _databaseFactory = databaseFactory;
        }

        public void Select()
        {
            SaveJourney();
            var route = JsonConvert.DeserializeObject<Route>(_inputJourney.Route);
            var lines = new List<string>();
            foreach (var step in route.legs[0].steps)
            {
                if (step.transit_details?.line != null)
                {
                    var name = step.transit_details.line.short_name != null
                        && !step.transit_details.line.short_name.Equals("null") ?
                        step.transit_details.line.short_name : step.transit_details.line.name;
                    lines.Add(name);
                }
            }

            UpdateLines(lines);
        }

        private void SaveJourney()
        {
            var dbAccount = _databaseFactory.GetAccountByUid(_uid);
            _returnJourney = new DbJourney
            {
                AccountId = dbAccount.Id,
                Name = _inputJourney.Name,
                Route = _inputJourney.Route,
                StartLocation = _inputJourney.StartLocation,
                EndLocation = _inputJourney.EndLocation,
                Time = _inputJourney.Time,
                Period = _inputJourney.Period
            };
            _databaseFactory.SaveJourneyToDb(_returnJourney);
            // Get journey from database to access journeyId
            _returnJourney = _databaseFactory.GetJourneyFromDb(_returnJourney);
        }

        private void UpdateLines(IEnumerable<string> lines)
        {
            lines = lines.ToList();
            var dbLines = lines.Select(line => new DbLine { Name = line, IsDelayed = JourneyStatus.GoodService });
            _databaseFactory.SaveLines(dbLines);
            var dbJourneyLines = lines.Select(line => new DbJourneyLine
            {
                JourneyId = _returnJourney.Id,
                LineId = _databaseFactory.GetLine(line).Id,
                Notified = false.ToString()
            });
            _databaseFactory.SaveJourneyLines(dbJourneyLines);
        }
    }
}