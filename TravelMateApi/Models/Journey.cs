namespace TravelMateApi.Models
{
    public class Journey
    {
        public string StartDateTime;
        public int Duration;
        public string ArrivalDateTime;
        public Leg[] Legs;
        public Line[] Lines;
    }
}