namespace TravelMateApi.Models
{
    public class GJourney
    {
        public GRoute[] routes;
    }

    public class GRoute
    {
        public GLeg[] legs;
    }

    public class GLeg
    {
        public GStep[] steps;
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
        public string html_instructions;
        public GTransitDetails transit_details;
    }
}
