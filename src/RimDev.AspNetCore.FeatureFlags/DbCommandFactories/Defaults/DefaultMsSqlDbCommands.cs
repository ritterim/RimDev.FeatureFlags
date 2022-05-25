using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace RimDev.AspNetCore.FeatureFlags.DbCommandFactories.Defaults
{
    public static class DefaultMsSqlDbCommands
    {
        internal const string DefaultSchemaName = "dbo";
        internal const string DefaultTableName = "RimDevAspNetCoreFeatureFlags";
        internal const string DefaultNameColumn = "FeatureName";
        internal const string DefaultValueColumn = "Enabled";

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

            // https://sqlperformance.com/2020/09/locking/upsert-anti-pattern
            queryCommand.CommandText =
                $@"
BEGIN TRANSACTION;

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

COMMIT TRANSACTION;
                ";

            return queryCommand;
        }
    }
}
