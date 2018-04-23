namespace TravelMateApi.Models
{
    public class GJourney
    {
        public string name;
        public string from;
        public string to;
        public string time;
        public string period;
        public string status;
        public GRoute[] routes;
        public DbLine[] disruptedLines;
    }

    public class GRoute
    {
        public GLeg[] legs;
    }

    public class GLeg
    {
        public string end_address;
        public string start_address;
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
