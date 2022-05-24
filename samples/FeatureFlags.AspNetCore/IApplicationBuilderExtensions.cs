using Microsoft.AspNetCore.Builder;
using RimDev.AspNetCore.FeatureFlags;

namespace FeatureFlags.AspNetCore
{
    //TODO: Remove this file
    public static class IApplicationBuilderExtensions
    {
        private static bool providerInitialized;

        public static IApplicationBuilder UseFeatureFlags(
            this IApplicationBuilder builder,
            FeatureFlagOptions options = default(FeatureFlagOptions))
        {
            if (!providerInitialized)
            {
                options.Provider.Initialize()
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();

                providerInitialized = true;
            }

            return builder;
        }
    }
}
