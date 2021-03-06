using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace RimDev.AspNetCore.FeatureFlags
{
    public class CachedSqlFeatureProvider : IFeatureProvider
    {
        private static readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        private static bool databaseInitialized;

        private static readonly SemaphoreSlim initializeDatabaseSemaphore = new SemaphoreSlim(1, 1);
        private static readonly SemaphoreSlim hydrateCacheIfNeededSemaphore = new SemaphoreSlim(1, 1);

        private readonly ConcurrentDictionary<string, object> cache =
            new ConcurrentDictionary<string, object>();

        private readonly IEnumerable<Assembly> featureFlagAssemblies;

        private readonly string connectionString;

        private readonly string schema;

        private readonly string tableName;

        private readonly TimeSpan cacheLifetime;

        private DateTime? cacheLastUpdatedAt;

        public CachedSqlFeatureProvider(
            IEnumerable<Assembly> featureFlagAssemblies,
            string connectionString,
            string schema = "dbo",
            string tableName = "RimDevAspNetCoreFeatureFlags",
            TimeSpan? cacheLifetime = null)
        {
            this.featureFlagAssemblies = featureFlagAssemblies ?? throw new ArgumentNullException(nameof(featureFlagAssemblies));
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            this.schema = schema ?? throw new ArgumentNullException(nameof(schema));
            this.tableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
            this.cacheLifetime = cacheLifetime ?? TimeSpan.FromMinutes(1);
        }

        public async Task Initialize()
        {
            if (!databaseInitialized)
            {
                await InitializeDatabase().ConfigureAwait(false);
            }

            await HydrateCacheIfNeeded().ConfigureAwait(false);
        }

        public async Task<Feature> Get(string featureName)
        {
            await HydrateCacheIfNeeded().ConfigureAwait(false);

            var valueExists = cache.TryGetValue(featureName, out object cacheValue);

            if (!valueExists)
                throw new ArgumentException($"{featureName} does not exist.");

            return (Feature)cacheValue;
        }

        public async Task Set<TFeature>(TFeature feature)
        {
            if (feature == null) throw new ArgumentNullException(nameof(feature));

            var featureName = feature.GetType().Name;

            cache.AddOrUpdate(featureName, feature, (_, __) => feature);

            var serializedFeature = JsonConvert.SerializeObject(feature, jsonSerializerSettings);

            await SetFeatureInDatabase(featureName, serializedFeature).ConfigureAwait(false);
        }

        private async Task HydrateCacheIfNeeded()
        {
            Func<bool> cacheStale = () => !cacheLastUpdatedAt.HasValue
                || cacheLastUpdatedAt < DateTime.UtcNow.Subtract(cacheLifetime);

            if (cacheStale())
            {
                // Hydrate with all defined types
                foreach (var featureType in featureFlagAssemblies.GetFeatureTypes())
                {
                    var feature = Activator.CreateInstance(featureType);

                    var featureName = feature.GetType().Name;

                    cache.AddOrUpdate(featureName, feature, (_, __) => feature);
                }

                try
                {
                    await hydrateCacheIfNeededSemaphore.WaitAsync().ConfigureAwait(false);

                    // Another thread could have refreshed the cache since the previous statement
                    if (!cacheStale())
                    {
                        return;
                    }

                    // Overwrite cached items based on existing database items
                    var features = await GetAllFeaturesFromDatabase().ConfigureAwait(false);

                    // Filter out features that no longer exist but were persisted in the database previously
                    // to avoid JsonSerializationException on `JsonConvert.DeserializeObject` for missing type.
                    features = features
                        .Where(x => cache.TryGetValue(x.Key, out var cachedFeature) && cachedFeature != default)
                        .ToDictionary(x => x.Key, x => x.Value);

                    foreach (var feature in features)
                    {
                        var value = JsonConvert.DeserializeObject(feature.Value, jsonSerializerSettings);

                        cache.AddOrUpdate(feature.Key, value, (_, __) => value);
                    }

                    cacheLastUpdatedAt = DateTime.UtcNow;
                }
                finally
                {
                    hydrateCacheIfNeededSemaphore.Release();
                }
            }
        }

        private async Task InitializeDatabase()
        {
            await initializeDatabaseSemaphore.WaitAsync().ConfigureAwait(false);

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync().ConfigureAwait(false);

                    var sql = $@"
if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA = '{schema}' and TABLE_NAME = '{tableName}')
begin
  create table [{schema}].[{tableName}] (FeatureName varchar(255), Feature varchar(4000))
end";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                }

                databaseInitialized = true;
            }
            finally
            {
                initializeDatabaseSemaphore.Release();
            }
        }

        private async Task<Dictionary<string, string>> GetAllFeaturesFromDatabase()
        {
            var data = new Dictionary<string, string>();

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                var sql = $"select FeatureName, Feature from [{schema}].[{tableName}]";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        data.Add(reader.GetString(0), reader.GetString(1));
                    }

                    reader.Close();
                }
            }

            return data;
        }

        private async Task SetFeatureInDatabase(string featureName, string serializedFeature)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                // Write SQL to support as many databases as possible
                // (avoid any sort of upsert)

                var transaction = conn.BeginTransaction();

                try
                {
                    var sql = $@"
delete from [{schema}].[{tableName}] where FeatureName = @featureName;
insert into [{schema}].[{tableName}] (FeatureName, Feature) values (@featureName, @feature);";

                    var cmd = new SqlCommand(sql, conn, transaction);

                    cmd.Parameters.AddWithValue("@featureName", featureName);
                    cmd.Parameters.AddWithValue("@feature", serializedFeature);

                    await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                }
            }
        }
    }
}
