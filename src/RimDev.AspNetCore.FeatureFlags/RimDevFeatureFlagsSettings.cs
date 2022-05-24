using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using Microsoft.FeatureManagement;

namespace RimDev.AspNetCore.FeatureFlags
{
    public class RimDevFeatureFlagsSettings
    {
        /// <summary>This is the connection string for talking to the database in order to
        /// perform SELECT/INSERT/UPDATE calls.</summary>
        public string ConnectionString { get; set; }

        /// <summary>This is the connection string for talking to the database in order to
        /// execute the <see cref="DbCommand"/> to create the database table if it does
        /// not exist.</summary>
        public string InitializationConnectionString { get; set; }

        /// <summary>
        /// <para>The <see cref="DbCommand"/> object which defensively create the feature
        /// flag value table.</para>
        /// <para>If not provided, a default CreateDatabaseTableCommandFactory will be used.</para>
        /// </summary>
        public Func<DbCommand> CreateDatabaseTableCommandFactory { get; set; }

        /// <summary>
        /// <para>The <see cref="DbCommand"/> object which will retrieve the correct row from
        /// the feature value database table.  It must be programmed with a parameter named
        /// "@featureName".</para>
        /// <para>If not provided, a default GetValueCommandFactory will be used.</para>
        /// </summary>
        public Func<string, DbCommand> GetValueCommandFactory { get; set; }

        /// <summary>
        /// <para>Note that this should almost always be left null and not specified.
        /// Unless your feature flag value table is per-user or per-session.</para>
        /// <para>The <see cref="DbCommand"/> object which will update the correct row in
        /// the feature value database table. It must be capable of handling both
        /// INSERT/UPDATE operations as required.</para>
        /// <para>If not provided, no default implementation will be used.</para>
        /// </summary>
        public Func<string, bool, DbCommand> SetValueCommandFactory { get; set; }

        /// <summary>
        /// <para></para>
        /// <para>The <see cref="DbCommand"/> object which will update the correct row in
        /// the feature value database table. It must be capable of handling both
        /// INSERT/UPDATE operations as required.</para>
        /// <para>If not provided, a default SetNullableValueCommandFactory will be used.</para>
        /// </summary>
        public Func<string, bool?, DbCommand> SetNullableValueCommandFactory { get; set; }

        /// <summary>Column name in the DbCommand result which contains the feature name.
        /// This needs to match the column name in the <see cref="GetValueCommandFactory"/>
        /// <see cref="DbCommand"/> CommandText.</summary>
        public string NameColumn { get; set; }

        /// <summary>Column name in the DbCommand result which contains the feature value.
        /// This needs to match the column name in the <see cref="GetValueCommandFactory"/>
        /// <see cref="DbCommand"/> CommandText.</summary>
        public string ValueColumn { get; set; }

        /// <summary>How long a cache entry will be valid until it is forced to
        /// refresh from the database.  Defaults to 60 seconds.</summary>
        public TimeSpan CacheTime { get; set; } = TimeSpan.FromSeconds(60);

        public string ApiGetAllPath => UiPath + "/get_all";

        public string ApiGetPath => UiPath + "/get";

        public string ApiSetPath => UiPath + "/set";

        public string UiPath => "/_features";

        public IEnumerable<Assembly> FeatureFlagAssemblies { get; set; }
            = new[] { Assembly.GetEntryAssembly() };
    }
}
