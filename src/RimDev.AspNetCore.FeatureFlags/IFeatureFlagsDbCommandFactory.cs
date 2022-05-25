using System.Data.Common;

namespace RimDev.AspNetCore.FeatureFlags
{
    /// <summary>In order to interface with various databases, table names, column name choices,
    /// the <see cref="FeatureFlagsSessionManager"/> needs a class which provides
    /// appropriate <see cref="DbCommand"/> methods.</summary>
    public interface IFeatureFlagsDbCommandFactory
    {
        DbConnection GetConnection();
        DbConnection GetInitializationConnection();

        DbCommand GetValue(string featureName);
        DbCommand SetValue(string featureName, bool enabled);
        DbCommand SetNullableValue(string featureName, bool? enabled);

        DbCommand CreateDatabaseTable();
    }
}
