using System;
using System.Threading.Tasks;
using LazyCache;
using Lussatite.FeatureManagement;
using Lussatite.FeatureManagement.SessionManagers;

namespace RimDev.AspNetCore.FeatureFlags
{
    public class FeatureFlagsSessionManager : ILussatiteSessionManager
    {
        private readonly FeatureFlagsSettings _featureFlagsSettings;
        private readonly CachedSqlSessionManager _cachedSqlSessionManager;

        public FeatureFlagsSessionManager(
            FeatureFlagsSettings featureFlagsSettings,
            IAppCache appCache = null
            )
        {
            _featureFlagsSettings = featureFlagsSettings
                ?? throw new ArgumentNullException(nameof(featureFlagsSettings));

            var cachedSqlSessionManagerSettings = new CachedSqlSessionManagerSettings
            {
                CacheTime = featureFlagsSettings.CacheTime,
            };

            _cachedSqlSessionManager = new CachedSqlSessionManager(
                cacheSettings: cachedSqlSessionManagerSettings,
                settings: featureFlagsSettings.SqlSessionManagerSettings,
                appCache: appCache
                );
        }

        public async Task SetAsync(string featureName, bool enabled) =>
            await _cachedSqlSessionManager.SetAsync(featureName, enabled).ConfigureAwait(false);

        public async Task<bool?> GetAsync(string featureName) =>
            await _cachedSqlSessionManager.GetAsync(featureName);

        public async Task SetNullableAsync(string featureName, bool? enabled) =>
            await _cachedSqlSessionManager.SetNullableAsync(featureName, enabled);
    }
}
