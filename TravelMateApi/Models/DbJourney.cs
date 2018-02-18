namespace TravelMateApi.Models
{
    public class DbJourney
    {
        public int Id { get; set; }
        public string Uid { get; set; }
        public int Position { get; set; }
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
    }
}