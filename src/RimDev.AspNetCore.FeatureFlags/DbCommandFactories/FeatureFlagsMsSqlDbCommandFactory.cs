using System.Data.Common;
using System.Data.SqlClient;
using RimDev.AspNetCore.FeatureFlags.DbCommandFactories.Defaults;

namespace RimDev.AspNetCore.FeatureFlags.DbCommandFactories
{
    public class FeatureFlagsMsSqlDbCommandFactory : IFeatureFlagsDbCommandFactory
    {
        private readonly FeatureFlagsSettings settings;

        public FeatureFlagsMsSqlDbCommandFactory(FeatureFlagsSettings settings)
        {
            this.settings = settings;
        }

        public DbConnection GetConnection()
            => new SqlConnection(settings.ConnectionString);

        public DbConnection GetInitializationConnection()
            => new SqlConnection(settings.InitializationConnectionString);

        public DbCommand GetValue(string featureName)
            => DefaultMsSqlDbCommands.GetValue(featureName);

        public DbCommand SetValue(string featureName, bool enabled)
            => DefaultMsSqlDbCommands.SetValue(featureName, enabled);

        public DbCommand SetNullableValue(string featureName, bool? enabled)
            => DefaultMsSqlDbCommands.SetNullableValue(featureName, enabled);

        public DbCommand CreateDatabaseTable()
            => DefaultMsSqlDbCommands.CreateDatabaseTable();
    }
}
