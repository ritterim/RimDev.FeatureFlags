using Microsoft.Extensions.DependencyInjection;

namespace RimDev.AspNetCore.FeatureFlags
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddFeatureFlags(
            this IServiceCollection service,
            FeatureFlagOptions options = default(FeatureFlagOptions))
        {
            service.AddSingleton(new FeatureFlags(options.Provider));

            foreach (var featureType in options.FeatureFlagAssemblies.GetFeatureTypes())
            {
                service.Add(new ServiceDescriptor(featureType, provider =>
                {
                    var featureFlags = provider.GetRequiredService<FeatureFlags>();

                    var featureFlag = featureFlags.Get(featureType)
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult();

                    return featureFlag;
                }, options.FeatureLifetime));
            }

            return service;
        }
    }
}
