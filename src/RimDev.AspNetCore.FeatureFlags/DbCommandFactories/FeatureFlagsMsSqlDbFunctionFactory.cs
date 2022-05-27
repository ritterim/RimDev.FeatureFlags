using System.Data.Common;
using System.Data.SqlClient;
using RimDev.AspNetCore.FeatureFlags.DbCommandFactories.Defaults;

namespace RimDev.AspNetCore.FeatureFlags.DbCommandFactories
{
    public class FeatureFlagsMsSqlDbFunctionFactory : IFeatureFlagsDbFunctionFactory
    {
        private readonly string connectionString;
        private readonly string initializationConnectionString;

        /// <summary>Construct the <see cref="IFeatureFlagsDbFunctionFactory"/>.</summary>
        /// <param name="connectionString">Connection string which can perform SELECT/INSERT/UPDATE calls
        /// against the database table.</param>
        /// <param name="initializationConnectionString">Connection string for executing the
        /// <see cref="DbCommand"/> to create the database table if it does not exist.</param>
        public FeatureFlagsMsSqlDbFunctionFactory(
            string connectionString,
            string initializationConnectionString
            )
        {
            this.connectionString = connectionString;
            this.initializationConnectionString = initializationConnectionString;
        }

        /// <inheritdoc cref="IFeatureFlagsDbFunctionFactory.GetConnection"/>
        public virtual DbConnection GetConnection()
            => new SqlConnection(connectionString);

        /// <inheritdoc cref="IFeatureFlagsDbFunctionFactory.GetInitializationConnection"/>
        public virtual DbConnection GetInitializationConnection()
            => new SqlConnection(initializationConnectionString);

        /// <inheritdoc cref="IFeatureFlagsDbFunctionFactory.GetValue"/>
        public virtual DbCommand GetValue(string featureName)
            => DefaultMsSqlDbFunctions.GetValue(featureName);

        /// <inheritdoc cref="IFeatureFlagsDbFunctionFactory.SetValue"/>
        public virtual DbCommand SetValue(string featureName, bool enabled)
            => null;

        /// <inheritdoc cref="IFeatureFlagsDbFunctionFactory.SetNullableValue"/>
        public virtual DbCommand SetNullableValue(string featureName, bool? enabled)
            => DefaultMsSqlDbFunctions.SetNullableValue(featureName, enabled);

        /// <inheritdoc cref="IFeatureFlagsDbFunctionFactory.CreateDatabaseTable"/>
        public virtual DbCommand CreateDatabaseTable()
            => DefaultMsSqlDbFunctions.CreateDatabaseTable();
    }
}
