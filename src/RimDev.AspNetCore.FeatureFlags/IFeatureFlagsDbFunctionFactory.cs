using System.Data.Common;
using Microsoft.FeatureManagement;

namespace RimDev.AspNetCore.FeatureFlags
{
    /// <summary>In order to interface with various databases, table names, column name choices,
    /// the <see cref="FeatureFlagsSessionManager"/> needs a class which provides
    /// appropriate <see cref="DbCommand"/> methods.</summary>
    public interface IFeatureFlagsDbFunctionFactory
    {
        DbConnection GetConnection();
        DbConnection GetInitializationConnection();

        DbCommand GetValue(string featureName);

        /// <summary>SetValue is normally not implemented due to how the Microsoft
        /// <see cref="IFeatureManagerSnapshot"/> implementation works.  It writes
        /// back to the <see cref="ISessionManager"/> implementation at the end of
        /// the method.  Unless your feature flags value table is per-user, this is
        /// not something that you would want to do.
        /// </summary>
        DbCommand SetValue(string featureName, bool enabled);

        DbCommand SetNullableValue(string featureName, bool? enabled);

        DbCommand CreateDatabaseTable();
    }
}
