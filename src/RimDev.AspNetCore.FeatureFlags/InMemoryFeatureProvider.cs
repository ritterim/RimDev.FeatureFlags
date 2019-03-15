using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RimDev.AspNetCore.FeatureFlags
{
    public class InMemoryFeatureProvider : IFeatureProvider
    {
        private readonly ConcurrentDictionary<string, object> data =
            new ConcurrentDictionary<string, object>();

        public Task<Feature> Get(string featureName)
        {
            var keyFound = data.TryGetValue(featureName, out var value);

            if (!keyFound)
                return null;

            return Task.FromResult((Feature)value);
        }

        public Task Set<TFeature>(TFeature feature)
        {
            if (feature == null) throw new ArgumentNullException(nameof(feature));

            data.AddOrUpdate(feature.GetType().Name, feature, (_, __) => feature);

            return Task.CompletedTask;
        }
    }
}
