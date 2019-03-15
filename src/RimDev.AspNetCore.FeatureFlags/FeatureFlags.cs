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

        public async Task<TFeature> Get<TFeature>() where TFeature : Feature
        {
            var feature = await provider.Get(typeof(TFeature).Name).ConfigureAwait(false);

            if (feature == null)
                throw new ArgumentException($"A feature named {typeof(TFeature).Name} was not found.");

            return (TFeature)feature;
        }

        public async Task<Feature> Get(Type featureType)
        {
            var feature = await provider.Get(featureType.Name).ConfigureAwait(false);

            if (feature == null)
                throw new ArgumentException($"A feature named {(feature.GetType().Name)} was not found.");

            return feature;
        }

        public async Task Set<TFeature>(TFeature feature)
        {
            if (feature == null) throw new ArgumentNullException(nameof(feature));

            await provider.Set(feature).ConfigureAwait(false);
        }
    }
}
