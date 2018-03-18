using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;

namespace TravelMateApi.Models
{
    public class GJourney
    {
        public GRoute[] routes;
    }

    public class GRoute
    {
        public GJourneyLeg[] legs;
    }

    public class GJourneyLeg
    {
        public GStep steps;
    }

    public class GTransitDetails
    {
        public GLine line;
    }

    public class GLine
    {
        public string name;
        public string short_name;
    }

    public class GStep
    {
        public GTransitDetails transit_details;
    }
}
