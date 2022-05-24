using System;
using System.Data.Common;
using System.Data.SqlClient;
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
    public class RimDevFeatureFlagsSessionManager : ILussatiteSessionManager
    {
        private readonly CachedSqlSessionManager cachedSqlSessionManager;
        private readonly RimDevFeatureFlagsSettings settings;

        public RimDevFeatureFlagsSessionManager(
            RimDevFeatureFlagsSettings settings = null,
            IAppCache cache = null
            )
        {
            this.settings = settings ?? new RimDevFeatureFlagsSettings();
            this.settings.CreateDatabaseTableCommandFactory ??= DefaultCreateDatabaseTableCommandFactory;
            this.settings.GetValueCommandFactory ??= DefaultGetValueCommandFactory;
            this.settings.SetNullableValueCommandFactory ??= DefaultSetNullableValueCommandFactory;

            cachedSqlSessionManager = new CachedSqlSessionManager(
                cache: cache,
                settings: GetCachedSqlSessionManagerSettings()
                );
        }

        internal const string DefaultSchemaName = "dbo";
        internal const string DefaultTableName = "RimDevAspNetCoreFeatureFlags";
        internal const string DefaultTableSchemaName = DefaultSchemaName + "." + DefaultTableName;
        internal const string DefaultNameColumn = "FeatureName";
        internal const string DefaultValueColumn = "Feature";

        private DbCommand DefaultGetValueCommandFactory(string featureName)
        {
            var queryCommand = new SqlCommand();
            queryCommand.CommandText =
                $@"
SELECT {DefaultNameColumn}, {DefaultValueColumn}
FROM {DefaultTableSchemaName}
WHERE {DefaultNameColumn} = @featureName;
                ";
            queryCommand.Parameters.Add(new SqlParameter("featureName", featureName));
            return queryCommand;
        }

        private DbCommand DefaultSetNullableValueCommandFactory(string featureName, bool? enabled)
        {
            var queryCommand = new SqlCommand();

            // https://sqlperformance.com/2020/09/locking/upsert-anti-pattern
            queryCommand.CommandText =
                $@"
BEGIN TRANSACTION;

UPDATE {DefaultTableSchemaName} WITH (UPDLOCK, SERIALIZABLE)
SET [{DefaultValueColumn}] = @featureEnabled
WHERE [{DefaultNameColumn}] = @featureName;

IF @@ROWCOUNT = 0
BEGIN
  INSERT {DefaultTableSchemaName}
  ([{DefaultNameColumn}], [{DefaultValueColumn})]
  VALUES(@featureName, @featureEnabled);
END

COMMIT TRANSACTION;
                ";

            queryCommand.Parameters.Add(new SqlParameter("featureName", featureName));
            queryCommand.Parameters.Add(new SqlParameter("featureEnabled", enabled));

            return queryCommand;
        }

        private CachedSqlSessionManagerSettings GetCachedSqlSessionManagerSettings()
        {
            return new CachedSqlSessionManagerSettings
            {
                CacheTime = settings.CacheTime,
                FeatureNameColumn = settings.NameColumn,
                FeatureValueColumn = settings.ValueColumn,
                GetConnectionFactory = GetConnectionFactory,
                GetValueCommandFactory = settings.GetValueCommandFactory,
                SetValueCommandFactory = settings.SetValueCommandFactory,
                SetNullableValueCommandFactory = settings.SetNullableValueCommandFactory,
            };
        }

        private DbConnection GetConnectionFactory() => new SqlConnection(settings.ConnectionString);

        /// <inheritdoc cref="CachedSqlSessionManager.GetAsync"/>
        public async Task<bool?> GetAsync(string featureName)
        {
            return await cachedSqlSessionManager.GetAsync(featureName).ConfigureAwait(false);
        }

        /// <summary>
        /// <para>Does nothing unless <see cref="RimDevFeatureFlagsSettings.SetValueCommandFactory"/>
        /// is defined (not null).</para>
        /// </summary>
        public async Task SetAsync(string featureName, bool enabled)
        {
            if (settings.SetValueCommandFactory is null) return;
            await cachedSqlSessionManager.SetAsync(featureName, enabled).ConfigureAwait(false);
        }

        /// <summary>
        /// <para>Does nothing unless <see cref="RimDevFeatureFlagsSettings.SetNullableValueCommandFactory"/>
        /// is defined (not null).</para>
        /// </summary>
        public async Task SetNullableAsync(string featureName, bool? enabled)
        {
            if (settings.SetNullableValueCommandFactory is null) return;
            await cachedSqlSessionManager.SetNullableAsync(featureName, enabled).ConfigureAwait(false);
        }

        public async Task CreateDatabaseTable()
        {
            await using var conn = new SqlConnection(settings.InitializationConnectionString);
            await using var queryCommand = conn.CreateCommand();
            throw new NotImplementedException();
        }

        private DbCommand DefaultCreateDatabaseTableCommandFactory()
        {
            var queryCommand = new SqlCommand();

            // https://sqlperformance.com/2020/09/locking/upsert-anti-pattern
            queryCommand.CommandText =
                $@"
BEGIN TRANSACTION;


COMMIT TRANSACTION;
                ";

            throw new NotImplementedException();
            //return queryCommand;
        }
    }
}
