namespace TravelMateApi.Models
{
    public class AndroidMessage
    {
        public string to;
        public AndroidNotification notification;

        public AndroidMessage(string to, string desc)
        {
            this.to = to;
            notification = new AndroidNotification
            {
                body = desc,
                title = "There is a disruption on one of your saved journeys!"
            };
        }
    }
}