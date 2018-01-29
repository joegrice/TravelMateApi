using System.Net.Http;
using System.Threading.Tasks;

namespace TravelMateApi.Connection
{
    public class ApiConnect
    {
        public async Task<string> GetJson(string url)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url + Credentials.Tfl);
            var result = await response.Content.ReadAsStringAsync();

            return result;
        }
    }
}