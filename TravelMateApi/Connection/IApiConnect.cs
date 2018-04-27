using System.Threading.Tasks;

namespace TravelMateApi.Connection
{
    public interface IApiConnect
    {
        Task<string> GetJson(string url);
    }
}