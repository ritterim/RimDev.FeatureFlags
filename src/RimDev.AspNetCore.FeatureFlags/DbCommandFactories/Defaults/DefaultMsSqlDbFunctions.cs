using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace RimDev.AspNetCore.FeatureFlags.DbCommandFactories.Defaults
{
    /// <summary>These are provided for interfacing with MS SQL (T-SQL).</summary>
    public static class DefaultMsSqlDbFunctions
    {
        // Use string constants to avoid SQL injection vulnerability.

        private const string DefaultSchemaName = "dbo";
        private const string DefaultTableName = "RimDevAspNetCoreFeatureFlags";
        private const string DefaultNameColumn = "FeatureName";
        private const string DefaultValueColumn = "Enabled";

        public static DbCommand GetValue(string featureName)
        {
            var queryCommand = new SqlCommand();
            queryCommand.CommandText =
                $@"
SELECT [{DefaultNameColumn}], [{DefaultValueColumn}]
FROM [{DefaultSchemaName}].[{DefaultTableName}]
WHERE {DefaultNameColumn} = @featureName;
                ";
            queryCommand.Parameters.Add(new SqlParameter("featureName", featureName));
            return queryCommand;
        }

        public static DbCommand SetValue(string featureName, bool enabled)
        {
            var queryCommand = new SqlCommand();

            // https://sqlperformance.com/2020/09/locking/upsert-anti-pattern
            queryCommand.CommandText =
                $@"
BEGIN TRANSACTION;

UPDATE [{DefaultSchemaName}].[{DefaultTableName}] WITH (UPDLOCK, SERIALIZABLE)
SET [{DefaultValueColumn}] = @featureEnabled
WHERE [{DefaultNameColumn}] = @featureName;

IF @@ROWCOUNT = 0
BEGIN
  INSERT [{DefaultSchemaName}].[{DefaultTableName}]
  ([{DefaultNameColumn}], [{DefaultValueColumn}])
  VALUES (@featureName, @featureEnabled);
END

COMMIT TRANSACTION;
                ";

            queryCommand.Parameters.Add(new SqlParameter("featureName", featureName));
            queryCommand.Parameters.Add(new SqlParameter("featureEnabled", enabled));

            return queryCommand;
        }

        public static DbCommand SetNullableValue(string featureName, bool? enabled)
        {
            var queryCommand = new SqlCommand();

            // https://sqlperformance.com/2020/09/locking/upsert-anti-pattern
            queryCommand.CommandText =
                $@"
BEGIN TRANSACTION;

UPDATE [{DefaultSchemaName}].[{DefaultTableName}] WITH (UPDLOCK, SERIALIZABLE)
SET [{DefaultValueColumn}] = @featureEnabled
WHERE [{DefaultNameColumn}] = @featureName;

IF @@ROWCOUNT = 0
BEGIN
  INSERT [{DefaultSchemaName}].[{DefaultTableName}]
  ([{DefaultNameColumn}], [{DefaultValueColumn}])
  VALUES (@featureName, @featureEnabled);
END

COMMIT TRANSACTION;
                ";

            queryCommand.Parameters.Add(new SqlParameter("featureName", featureName));
            queryCommand.Parameters.Add(new SqlParameter("featureEnabled", SqlDbType.Bit)
            {
                Value = (object) enabled ?? DBNull.Value,
                IsNullable=true
            });

            return queryCommand;
        }

        public static DbCommand CreateDatabaseTable()
        {
            var queryCommand = new SqlCommand();

            //TODO: This should also create the schema, if it does not exist

            // https://sqlperformance.com/2020/09/locking/upsert-anti-pattern
            queryCommand.CommandText =
                $@"
if not exists
    (select * from INFORMATION_SCHEMA.TABLES
    where TABLE_SCHEMA = '{DefaultSchemaName}'
    and TABLE_NAME = '{DefaultTableName}')
begin
  create table [{DefaultSchemaName}].[{DefaultTableName}]
  (
    [{DefaultNameColumn}] nvarchar(255) not null,
    [{DefaultValueColumn}] bit,
    CONSTRAINT PK_{DefaultTableName}_{DefaultNameColumn} PRIMARY KEY CLUSTERED ({DefaultNameColumn})
  )
end
                ";

            return queryCommand;
        }
    }
}
