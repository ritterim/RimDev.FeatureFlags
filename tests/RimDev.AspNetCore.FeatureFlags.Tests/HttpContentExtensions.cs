using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace RimDev.AspNetCore.FeatureFlags.Tests
{
    public static class HttpContentExtensions
    {
        public static async Task<T> ReadAsJson<T>(this HttpContent content)
        {
            var str = await content.ReadAsStringAsync().ConfigureAwait(false);

            return (T)JsonConvert.DeserializeObject(str, typeof(T));
        }
    }
}
