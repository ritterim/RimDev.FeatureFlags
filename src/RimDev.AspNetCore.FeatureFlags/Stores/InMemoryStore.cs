using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeatureFlags.Stores
{
    public class InMemoryStore : IStore
    {
        public List<Feature> Features
            = new List<Feature>();

        public async Task<T> Save<T>(T feature)
            where T : Feature
        {
            if (await Get<T>() == null)
            {
                Features.Add(feature);
            }

            return feature;
        }

        public Task<T> Get<T>() where T : Feature
        {
            return Task.FromResult(
                Features.FirstOrDefault(f => f.GetType() == typeof(T)) as T
            );
        }
    }
}
