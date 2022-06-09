using System;
using System.Threading.Tasks;
using Lussatite.FeatureManagement;
using Lussatite.FeatureManagement.SessionManagers;

namespace RimDev.AspNetCore.FeatureFlags
{
    public class FeatureFlagsSessionManager : ILussatiteSessionManager
    {
        private readonly CachedSqlSessionManager cachedSqlSessionManager;

        public FeatureFlagsSessionManager(
            SqlSessionManagerSettings sqlSessionManagerSettings,
            CachedSqlSessionManagerSettings cachedSqlSessionManagerSettings = null
            )
        {
            if (sqlSessionManagerSettings is null)
                throw new ArgumentNullException(nameof(sqlSessionManagerSettings));
            cachedSqlSessionManagerSettings ??= new CachedSqlSessionManagerSettings();
            cachedSqlSessionManager = new CachedSqlSessionManager(
                cacheSettings: cachedSqlSessionManagerSettings,
                settings: sqlSessionManagerSettings
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
