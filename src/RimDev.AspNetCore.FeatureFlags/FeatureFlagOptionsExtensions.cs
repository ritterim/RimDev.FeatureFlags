using System;

namespace RimDev.AspNetCore.FeatureFlags
{
    public static class FeatureFlagOptionsExtensions
    {
        public static FeatureFlagOptions UseCachedSqlFeatureProvider(
            this FeatureFlagOptions options,
            string connectionString)
        {
            if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));

            options.Provider = new CachedSqlFeatureProvider(
                options.FeatureFlagAssemblies,
                connectionString);

            return options;
        }

        /// <summary>
        /// This should only be used for testing as values
        /// are not persisted to any long term storage.
        /// </summary>
        public static FeatureFlagOptions UseInMemoryFeatureProvider(
            this FeatureFlagOptions options)
        {
            options.Provider = new InMemoryFeatureProvider(
                options.FeatureFlagAssemblies);

            return options;
        }
    }
}
