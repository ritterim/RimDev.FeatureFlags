using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RimDev.AspNetCore.FeatureFlags
{
    public class InMemoryFeatureProvider : IFeatureProvider
    {
        private readonly ConcurrentDictionary<string, object> data =
            new ConcurrentDictionary<string, object>();

        private readonly IEnumerable<Assembly> featureFlagAssemblies;

        public InMemoryFeatureProvider(IEnumerable<Assembly> featureFlagAssemblies)
        {
            this.featureFlagAssemblies = featureFlagAssemblies ?? throw new ArgumentNullException(nameof(featureFlagAssemblies));
        }

        public async Task Initialize()
        {
            foreach (var featureType in featureFlagAssemblies.GetFeatureTypes())
            {
                var feature = (Feature) Activator.CreateInstance(featureType);
                await Set(feature).ConfigureAwait(false);
            }
        }

        public Task<TFeature> Get<TFeature>()
            where TFeature: Feature
        {
            var keyFound = data.TryGetValue(typeof(TFeature).Name, out var value);

            if (!keyFound)
                return null;

            return Task.FromResult((TFeature)value);
        }

        public Task Set<TFeature>(TFeature feature)
            where TFeature: Feature
        {
            if (feature == null) throw new ArgumentNullException(nameof(feature));

            data.AddOrUpdate(feature.GetType().Name, feature, (_, __) => feature);

            return Task.CompletedTask;
        }
    }
}
