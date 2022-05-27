using System.Data.Common;
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
        private readonly IFeatureFlagsDbFunctionFactory dbFunctionFactory;
        private readonly CachedSqlSessionManager cachedSqlSessionManager;
        private readonly FeatureFlagsSettings settings;

        /// <summary>Create the session manager.</summary>
        /// <param name="dbFunctionFactory"></param>
        /// <param name="settings"><see cref="FeatureFlagsSettings"/></param>
        /// <param name="cache">An optional <see cref="IAppCache"/> instance.</param>
        public FeatureFlagsSessionManager(
            IFeatureFlagsDbFunctionFactory dbFunctionFactory,
            FeatureFlagsSettings settings = null,
            IAppCache cache = null
            )
        {
            this.dbFunctionFactory = dbFunctionFactory;
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
                GetConnectionFactory = dbFunctionFactory.GetConnection,
                GetValueCommandFactory = dbFunctionFactory.GetValue,
                SetValueCommandFactory = dbFunctionFactory.SetValue,
                SetNullableValueCommandFactory = dbFunctionFactory.SetNullableValue,
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
            await using var conn = dbFunctionFactory.GetInitializationConnection();
            await using var queryCommand = dbFunctionFactory.CreateDatabaseTable();
            queryCommand.Connection = conn;
            await conn.OpenAsync().ConfigureAwait(false);
            await queryCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
            await conn.CloseAsync();
        }
    }
}
