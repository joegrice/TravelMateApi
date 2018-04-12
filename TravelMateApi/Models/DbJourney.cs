namespace TravelMateApi.Models
{
    public class DbJourney
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string Route { get; set; }
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
        public string Time { get; set; }
        public string Period { get; set; }
    }
}