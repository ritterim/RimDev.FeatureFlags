using System.Threading.Tasks;
using LazyCache;
using Lussatite.FeatureManagement;
using Lussatite.FeatureManagement.SessionManagers;
using Microsoft.FeatureManagement;

namespace RimDev.AspNetCore.FeatureFlags
{
    /// <summary>An implementation of <see cref="ILussatiteSessionManager"/> and <see cref="ISessionManager"/>
    /// which uses short-term caching to reduce the number of calls to the database table.
    /// </summary>
    public class FeatureFlagsSessionManager : ILussatiteSessionManager
    {
        private readonly IFeatureFlagsDbCommandFactory dbCommandFactory;
        private readonly CachedSqlSessionManager cachedSqlSessionManager;
        private readonly FeatureFlagsSettings settings;

        public FeatureFlagsSessionManager(
            IFeatureFlagsDbCommandFactory dbCommandFactory,
            FeatureFlagsSettings settings = null,
            IAppCache cache = null
            )
        {
            this.dbCommandFactory = dbCommandFactory;
            this.settings = settings ?? new FeatureFlagsSettings();

            cachedSqlSessionManager = new CachedSqlSessionManager(
                cache: cache,
                settings: GetCachedSqlSessionManagerSettings()
                );
        }

        private CachedSqlSessionManagerSettings GetCachedSqlSessionManagerSettings()
        {
            return new CachedSqlSessionManagerSettings
            {
                CacheTime = settings.CacheTime,
                FeatureNameColumn = settings.NameColumn,
                FeatureValueColumn = settings.ValueColumn,
                GetConnectionFactory = dbCommandFactory.GetConnection,
                GetValueCommandFactory = dbCommandFactory.GetValue,
                SetValueCommandFactory = dbCommandFactory.SetValue,
                SetNullableValueCommandFactory = dbCommandFactory.SetNullableValue,
            };
        }

        public async Task<bool?> GetAsync(string featureName)
        {
            return await cachedSqlSessionManager.GetAsync(featureName).ConfigureAwait(false);
        }

        public async Task SetAsync(string featureName, bool enabled)
        {
            await cachedSqlSessionManager.SetAsync(featureName, enabled).ConfigureAwait(false);
        }

        public async Task SetNullableAsync(string featureName, bool? enabled)
        {
            await cachedSqlSessionManager.SetNullableAsync(featureName, enabled).ConfigureAwait(false);
        }

        public async Task CreateDatabaseTable()
        {
            await using var conn = dbCommandFactory.GetInitializationConnection();
            await using var queryCommand = dbCommandFactory.CreateDatabaseTable();
            queryCommand.Connection = conn;
            await conn.OpenAsync().ConfigureAwait(false);
            await queryCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
            await conn.CloseAsync();
        }
    }
}
