namespace TravelMateApi.Models
{
    public class JourneySearch
    {
        public Journey[] Journeys;
        public Line[] Lines;
    }

    public class Journey
    {
        public string StartDateTime;
        public int Duration;
        public string ArrivalDateTime;
        public Leg[] Legs;
        public Line[] Lines;
    }

    public class Line
    {
        public string Id;
        public string Name;
        public string ModeName;
        public LineStatus[] LineStatuses;
    }

    public class LineStatus
    {
        public string StatusSeverityDescription;
    }

    public class Instruction
    {
        public string Summary;
        public string Detailed;
        public Step[] Steps;
    }

    public class Step
    {
        public string DescriptionHeading;
        public string Description;
        public string TurnDirection;
        public string StreetName;
        public int Distance;
    }

    public class Leg
    {
        public int Duration;
        public Instruction Instruction;
    }
}