using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeatureFlags.Stores
{
    public class InMemoryStore : IStore
    {
        private Dictionary<Type, FeatureStoreData> featureData = new Dictionary<Type, FeatureStoreData>();

        public async Task<FeatureStoreData> Get<T>()
            where T : Feature
        {
            if (featureData.TryGetValue(typeof(T), out var value))
            {
                return await Task.FromResult(value);
            }
            else
            {
                return await Task.FromResult(null as FeatureStoreData);
            }
        }

        public async Task<bool> Enrich<T>(T feature)
            where T : Feature
        {
            if (featureData.TryGetValue(typeof(T), out var value))
            {
                feature.Enabled = value.Enabled;

                feature.Parameters.Clear();

                foreach (var parameter in value.Parameters)
                {
                    feature.Parameters.Add(new Parameter()
                    {
                        Name = parameter.Key,
                        Value = parameter.Value
                    });
                }

                return await Task.FromResult(true);
            }
            else
            {
                return await Task.FromResult(false);
            }
        }

        public async Task<T> Save<T>(T feature)
            where T : Feature
        {
            var featureStoreData = new FeatureStoreData()
            {
                Enabled = feature.Enabled,
                Parameters = feature.Parameters.ToDictionary(x => x.Name, x => x.Value)
            };

            if (featureData.TryGetValue(typeof(T), out var value))
            {
                value = featureStoreData;
            }
            else
            {
                featureData.Add(typeof(T), featureStoreData);
            }

            return await Task.FromResult(feature);
        }
    }
}
