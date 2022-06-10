using System;
using System.Threading.Tasks;
using Lussatite.FeatureManagement;
using Lussatite.FeatureManagement.SessionManagers;

namespace RimDev.AspNetCore.FeatureFlags
{
    public class FeatureFlagsSessionManager : ILussatiteSessionManager
    {
        private readonly FeatureFlagsSettings featureFlagsSettings;
        private readonly CachedSqlSessionManager cachedSqlSessionManager;

        public FeatureFlagsSessionManager(
            FeatureFlagsSettings featureFlagsSettings
            )
        {
            this.featureFlagsSettings = featureFlagsSettings
                ?? throw new ArgumentNullException(nameof(featureFlagsSettings));

            var cachedSqlSessionManagerSettings = new CachedSqlSessionManagerSettings
            {
                CacheTime = featureFlagsSettings.CacheTime,
            };

            cachedSqlSessionManager = new CachedSqlSessionManager(
                cacheSettings: cachedSqlSessionManagerSettings,
                settings: featureFlagsSettings.SqlSessionManagerSettings
                );
        }

        public async Task SetAsync(string featureName, bool enabled) =>
            await cachedSqlSessionManager.SetAsync(featureName, enabled).ConfigureAwait(false);

        public async Task<bool?> GetAsync(string featureName) =>
            await cachedSqlSessionManager.GetAsync(featureName);

        public async Task SetNullableAsync(string featureName, bool? enabled) =>
            await cachedSqlSessionManager.SetNullableAsync(featureName, enabled);
    }
}
