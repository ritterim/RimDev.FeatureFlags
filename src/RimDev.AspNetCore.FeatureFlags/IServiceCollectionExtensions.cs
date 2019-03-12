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

            return service;
        }
    }
}
