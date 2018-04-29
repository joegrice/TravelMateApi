namespace TravelMateApi.Models
{
    public class Journey
    {
        public int id;
        public string name;
        public string from;
        public string to;
        public string time;
        public string period;
        public string status;
        public Route[] routes;
        public DbLine[] disruptedLines;
    }

    public class Route
    {
        public Leg[] legs;
    }

    public class Leg
    {
        public TextValue duration;
        public string end_address;
        public string start_address;
        public Step[] steps;
    }

    public class TransitDetails
    {
        public Line line;
    }

    public class Line
    {
        public string name;
        public string short_name;
    }

    public class Step
    {
        public TextValue duration;
        public string html_instructions;
        public TransitDetails transit_details;
    }

    public class TextValue
    {
        public string text;
        public int value;
    }
}
