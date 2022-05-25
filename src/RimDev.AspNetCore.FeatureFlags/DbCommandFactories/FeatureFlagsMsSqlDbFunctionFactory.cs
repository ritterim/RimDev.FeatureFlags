using System.Data.Common;
using System.Data.SqlClient;
using RimDev.AspNetCore.FeatureFlags.DbCommandFactories.Defaults;

namespace RimDev.AspNetCore.FeatureFlags.DbCommandFactories
{
    public class FeatureFlagsMsSqlDbFunctionFactory : IFeatureFlagsDbFunctionFactory
    {
        private readonly FeatureFlagsSettings settings;

        public FeatureFlagsMsSqlDbFunctionFactory(FeatureFlagsSettings settings)
        {
            this.settings = settings;
        }

        /// <inheritdoc cref="IFeatureFlagsDbFunctionFactory.GetConnection"/>
        public virtual DbConnection GetConnection()
            => new SqlConnection(settings.ConnectionString);

        /// <inheritdoc cref="IFeatureFlagsDbFunctionFactory.GetInitializationConnection"/>
        public virtual DbConnection GetInitializationConnection()
            => new SqlConnection(settings.InitializationConnectionString);

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
