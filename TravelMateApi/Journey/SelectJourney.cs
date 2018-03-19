using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TravelMateApi.Connection;
using TravelMateApi.Database;
using TravelMateApi.Models;

namespace TravelMateApi.Journey
{
    public class SelectJourney
    {
        private readonly int _pos;
        private readonly string _startLocation;
        private readonly string _endLocation;
        private readonly string _uid;

        public SelectJourney(string uid, int pos, string startLocation, string endLocation)
        {
            _uid = uid;
            _pos = pos;
            _startLocation = startLocation;
            _endLocation = endLocation;
        }

        public void Select()
        {
            var apiConnect = new ApiConnect();
            var url = UrlFactory.GetGoogleJourneys(_startLocation, _endLocation);
            var json = apiConnect.GetJson(url);
            var result = JsonConvert.DeserializeObject<GJourney>(json.Result);
            //var selectedResult = result.Journeys[_pos];
            //var serializeObject = JsonConvert.SerializeObject(selectedResult);
            SaveJourney();
            //UpdateLines(result.Lines);
        }

        private void SaveJourney()
        {
            var dbJourney = new DbJourney
            {
                Uid = _uid,
                Position = _pos,
                StartLocation = _startLocation,
                EndLocation = _endLocation
            };
            var databaseFactory = new DatabaseFactory();
            databaseFactory.SaveJourneyToDb(dbJourney);
        }

        private void UpdateLines(IEnumerable<Line> lines)
        {
            var databaseFactory = new DatabaseFactory();
            var dbLines = lines.Select(line => new DbLine { LineId = line.Id, Name = line.Name });
            databaseFactory.SaveLines(dbLines);
            var dbJourneyLines = lines.Select(line => new DbJourneyLine { Uid = _uid, ModeId = line.Name });
            databaseFactory.SaveJourneyLines(dbJourneyLines);
        }
    }
}