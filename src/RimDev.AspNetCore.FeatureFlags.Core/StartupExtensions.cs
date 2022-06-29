using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace RimDev.AspNetCore.FeatureFlags.Core
{
    public static class StartupExtensions
    {
        public static IApplicationBuilder UseRimDevFeatureFlags(
            this IApplicationBuilder app
            )
        {
            app.CreateFeatureFlagsTable();
            return app;
        }

        private static bool _sessionManagerInitialized;

        /// <summary>Create the feature flags table (must be done via a defensive SQL script).</summary>
        public static IApplicationBuilder CreateFeatureFlagsTable(
            this IApplicationBuilder app
            )
        {
            if (_sessionManagerInitialized) return app;

            var featureFlagsSettings = app
                .ApplicationServices
                .GetRequiredService<FeatureFlagsSettings>();
            featureFlagsSettings.SqlSessionManagerSettings.CreateDatabaseTable(
                featureFlagsSettings.InitializationConnectionString
            );
            _sessionManagerInitialized = true;

            return app;
        }
    }
}
