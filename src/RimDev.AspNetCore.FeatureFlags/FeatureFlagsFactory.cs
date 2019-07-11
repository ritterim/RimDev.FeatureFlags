using FeatureFlags.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FeatureFlags
{
    public class FeatureFlagsFactory
    {
        public async Task<T> Get<T>(IServiceProvider serviceProvider)
            where T : Feature
        {
            var feature = Activator.CreateInstance(typeof(T)) as T;
            var metadata = FeatureAttribute.GetMetadata<T>();
            var conditions = serviceProvider.GetServices<ICondition>();
            var storePointers = serviceProvider.GetServices<IStorePointer>().ToList();
            var stores = serviceProvider.GetServices<IStore>().ToList();

            foreach (var metadataStore in metadata.Stores)
            {
                var storePointer = storePointers.FirstOrDefault(x => x.Name.Equals(metadataStore.Key, StringComparison.OrdinalIgnoreCase));
                var store = stores.FirstOrDefault(x => x.GetType() == storePointer?.Type);

                if (stores == null)
                {
                    throw new Exception(
                        $"The store ({metadataStore.Key}) has not been registered and cannot be used with feature ({typeof(T).Name}).");
                }

                if ((await store.Enrich(feature)) == true)
                {
                    break;
                }
            }

            var conditionTypes = metadata.Conditions.Select(x => x.Type).ToList();

            foreach (var condition in conditions
                .Where(x => conditionTypes.Contains(x.GetType())))
            {
                await condition.Apply(feature);
            }

            return feature;
        }
    }
}
