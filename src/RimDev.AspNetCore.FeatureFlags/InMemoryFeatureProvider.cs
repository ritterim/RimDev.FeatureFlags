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
            var featureTypes = featureFlagAssemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(Feature)));

            foreach (var featureType in featureTypes)
            {
                var feature = Activator.CreateInstance(featureType);

                await Set(feature).ConfigureAwait(false);
            }
        }

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
