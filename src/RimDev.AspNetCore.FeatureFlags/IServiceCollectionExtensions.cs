using System;
using Microsoft.Extensions.DependencyInjection;

namespace RimDev.AspNetCore.FeatureFlags
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddFeatureFlags(
            this IServiceCollection service,
            FeatureFlagOptions options = default(FeatureFlagOptions))
        {
            service.AddSingleton(new FeatureFlags(options.Provider)); // TODO: Remove in v2?

            foreach (var featureType in options.FeatureFlagAssemblies.GetFeatureTypes())
            {
                var feature = (Feature)Activator.CreateInstance(featureType);

                service.Add(new ServiceDescriptor(featureType, provider =>
                {
                    var featureFlags = provider.GetRequiredService<FeatureFlags>();

                    var featureFlag = featureFlags.Get(featureType)
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult();

                    return featureFlag;
                }, feature.ServiceLifetime));
            }

            return service;
        }
    }
}
