namespace TravelMateApi.Models
{
    public class DbJourneyLine
    {
        public int Id { get; set; }
        public int JourneyId { get; set; }
        public int LineId { get; set; }
        public string Notified { get; set; }
    }
}
