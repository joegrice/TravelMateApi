namespace TravelMateApi.Models
{
    public class AndroidNotification
    {
        private string token;
        private AData data;
        private ANotification notification;

        public AndroidNotification(string token, AData data, ANotification notification)
        {
            this.token = token;
            this.data = data;
            this.notification = notification;
        }
    }

    public class AData
    {
        private string ShortDesc;
        public string IncidentNo = "0";
        private string Description;

        public AData(string shortDesc, string description)
        {
            ShortDesc = shortDesc;
            Description = description;
        }
    }

    public class ANotification
    {
        private string title;
        public string text = "This is a Notification Text3";
        public string sound = "default";

        public ANotification(string title)
        {
            this.title = title;
        }
    }
}
