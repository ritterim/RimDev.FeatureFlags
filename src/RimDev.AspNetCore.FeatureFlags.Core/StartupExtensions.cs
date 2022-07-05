using System.Data.SqlClient;
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
            if (_sessionManagerInitialized) return app;

            app.CreateFeatureFlagsSchema();
            app.CreateFeatureFlagsTable();
            _sessionManagerInitialized = true;

            return app;
        }

        private static bool _sessionManagerInitialized;

        /// <summary>Create the feature flags schema (must be done via a defensive SQL script).</summary>
        public static IApplicationBuilder CreateFeatureFlagsSchema(
            this IApplicationBuilder app
            )
        {
            var featureFlagsSettings = app
                .ApplicationServices
                .GetRequiredService<FeatureFlagsSettings>();

            if (!string.IsNullOrEmpty(featureFlagsSettings?.SqlSessionManagerSettings?.FeatureSchemaName))
            {
                using var conn = new SqlConnection(featureFlagsSettings.InitializationConnectionString);
                conn.Open();
                using var cmd = conn.CreateCommand();
                var schema = featureFlagsSettings.SqlSessionManagerSettings.FeatureSchemaName;
                cmd.CommandText = @$"
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'{schema}')
EXEC('CREATE SCHEMA [{schema}];');";
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            return app;
        }

        /// <summary>Create the feature flags table (must be done via a defensive SQL script).</summary>
        public static IApplicationBuilder CreateFeatureFlagsTable(
            this IApplicationBuilder app
            )
        {
            var featureFlagsSettings = app
                .ApplicationServices
                .GetRequiredService<FeatureFlagsSettings>();

            featureFlagsSettings.SqlSessionManagerSettings.CreateDatabaseTable(
                featureFlagsSettings.InitializationConnectionString
                );

            return app;
        }
    }
}
