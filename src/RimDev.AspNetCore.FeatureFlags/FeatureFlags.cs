using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace RimDev.AspNetCore.FeatureFlags
{
    public class FeatureFlags
    {
        private readonly IFeatureProvider provider;

        public FeatureFlags(IFeatureProvider provider)
        {
            this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public async Task<TFeature> Get<TFeature>()
            where TFeature : Feature
        {
            var feature = await provider.Get<TFeature>().ConfigureAwait(false);

            if (feature == null)
                throw new ArgumentException($"A feature named {typeof(TFeature).Name} was not found.");

            return (TFeature)feature;
        }

        public async Task<Feature> Get(Type featureType)
        {
            var method = typeof(IFeatureProvider)
                .GetMethod(nameof(IFeatureProvider.Get))
                .MakeGenericMethod(featureType);

            var task = (Task) method.Invoke(provider, null);

            await task.ConfigureAwait(false);

            var resultProperty = task.GetType().GetProperty("Result");
            var feature = resultProperty.GetValue(task) as Feature;

            if (feature == null)
                throw new ArgumentException($"A feature named {featureType.Name} was not found.");

            return feature;
        }

        public async Task Set<TFeature>(TFeature feature)
            where TFeature: Feature
        {
            if (feature == null) throw new ArgumentNullException(nameof(feature));
            await provider.Set(feature).ConfigureAwait(false);
        }
    }
}
