using Microsoft.Extensions.DependencyInjection;

namespace RimDev.AspNetCore.FeatureFlags.UI
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddFeatureFlagUiSettings(
            this IServiceCollection service
            )
        {
            service.AddSingleton<FeatureFlagUiSettings>();
            return service;
        }
    }
}
