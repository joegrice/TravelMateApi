namespace TravelMateApi.Models
{
    public class DbLine
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IsDelayed { get; set; }
        public string UsersNotified { get; set; }
    }
}