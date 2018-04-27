using TravelMateApi.Models;

namespace TravelMateApiTests
{
    public class GJourneyBuilder
    {
        private GJourney _journey;

        public GJourneyBuilder()
        {
            _journey = new GJourney
            {
                name = "Journey Name",
                time = "12:23",
                period = "20",
                disruptedLines = new DbLine[] { },
                status = JourneyStatus.GoodService
            };
        }

        public GJourneyBuilder WithId(int id)
        {
            _journey.id = id;
            return this;
        }

        public GJourneyBuilder WithStart(string startLocation)
        {
            _journey.from = startLocation;
            return this;
        }

        public GJourneyBuilder WithEnd(string endLocation)
        {
            _journey.to = endLocation;
            return this;
        }

        public GJourneyBuilder AddRoute(string instruction, string lineName)
        {
            var transitDetails = new GTransitDetails
            {
                line = new GLine
                {
                    name = lineName,
                    short_name = lineName + "Short"
                }
            };
            var step = new GStep()
            {
                html_instructions = instruction,
                transit_details = transitDetails
            };
            var leg = new GLeg()
            {
                start_address = _journey.from,
                end_address = _journey.to,
                steps = new[] { step }
            };
            var route = new GRoute
            {
                legs = new[] { leg }
            };
            _journey.routes = new[] { route };
            return this;
        }

        public GJourneyBuilder WithDisruptedLine(int id, string name, string description, string isDelayed)
        {
            _journey.disruptedLines = new[]
            {
                new DbLine
                {
                    Id = id,
                    Name = name,
                    Description = description,
                    IsDelayed = isDelayed
                }
            };
            _journey.status = isDelayed;
            return this;
        }

        public GJourney Build()
        {
            return _journey;
        }
    }
}
