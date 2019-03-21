using System.Linq;
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

            var featuresTypes = options.FeatureFlagAssemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(Feature)));

            foreach (var featureType in featuresTypes)
            {
                service.AddTransient(featureType, provider =>
                {
                    var featureFlags = provider.GetRequiredService<FeatureFlags>();

                    var featureFlag = featureFlags.Get(featureType)
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult();

                    return featureFlag;
                });
            }

            return service;
        }
    }
}
