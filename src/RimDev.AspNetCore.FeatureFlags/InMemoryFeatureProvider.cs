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

        public Task<TFeature> Get<TFeature>()
        {
            var keyFound = data.TryGetValue(typeof(TFeature).Name, out var value);

            if (!keyFound)
                return null;

            return Task.FromResult((TFeature)value);
        }

        public Task Set<TFeature>(TFeature feature)
        {
            if (feature == null) throw new ArgumentNullException(nameof(feature));

            data.AddOrUpdate(typeof(TFeature).Name, feature, (_, __) => feature);

            return Task.CompletedTask;
        }
    }
}
