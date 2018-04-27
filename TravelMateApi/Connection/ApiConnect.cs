using System.Net.Http;
using System.Threading.Tasks;

namespace TravelMateApi.Connection
{
    public class ApiConnect : IApiConnect
    {
        public async Task<string> GetJson(string url)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();

            return result;
        }
    }
}