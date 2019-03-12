using Microsoft.AspNetCore.Builder;

namespace RimDev.AspNetCore.FeatureFlags
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseFeatureFlags(
            this IApplicationBuilder builder,
            FeatureFlagOptions options = default(FeatureFlagOptions))
        {
            builder.Map(options.UiPath, x =>
            {
                // Display UI
            });

            return builder;
        }
    }
}
