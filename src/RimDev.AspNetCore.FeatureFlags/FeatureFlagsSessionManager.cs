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
            FeatureFlagsSettings featureFlagsSettings,
            SqlSessionManagerSettings sqlSessionManagerSettings,
            CachedSqlSessionManagerSettings cachedSqlSessionManagerSettings = null
            )
        {
            if (sqlSessionManagerSettings is null)
                throw new ArgumentNullException(nameof(sqlSessionManagerSettings));
            this.featureFlagsSettings = featureFlagsSettings;
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

        public void CreateDatabaseTable() =>
            cachedSqlSessionManager.Settings.CreateDatabaseTable(featureFlagsSettings.InitializationConnectionString);
    }
}
