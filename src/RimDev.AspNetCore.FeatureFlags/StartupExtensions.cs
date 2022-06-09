using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lussatite.FeatureManagement.SessionManagers;
using Lussatite.FeatureManagement.SessionManagers.SqlClient;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace RimDev.AspNetCore.FeatureFlags
{
    public static class StartupExtensions
    {
        /// <summary>Add <see cref="FeatureFlagsSettings"/> to the DI container.  The two
        /// connection strings are hardcoded as "featureFlags" and "featureFlagsInitialization".
        /// </summary>
        /// <param name="service"><see cref="IServiceCollection"/></param>
        /// <param name="configuration"><see cref="IConfiguration"/></param>
        /// <param name="featureFlagAssemblies">A list of assemblies to scan for classes inheriting from
        /// <see cref="Feature"/>.</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        /// <exception cref="Exception">When the connection strings are missing/empty.</exception>
        public static IServiceCollection AddFeatureFlagSettings(
            this IServiceCollection service,
            IConfiguration configuration,
            IEnumerable<Assembly> featureFlagAssemblies
            )
        {
            service.AddScoped(services =>
            {
                const string connectionStringName = "featureFlags";
                var connectionString = configuration.GetConnectionString(connectionStringName);
                if (string.IsNullOrEmpty(connectionString))
                    throw new Exception($"Failed to retrieve connection string: {connectionStringName}");

                const string initializationConnectionStringName = "featureFlagsInitialization";
                var initializationConnectionString =
                    configuration.GetConnectionString(initializationConnectionStringName);
                if (string.IsNullOrEmpty(connectionString))
                    throw new Exception(
                        $"Failed to retrieve initialization connection string: {initializationConnectionString}");

                return new FeatureFlagsSettings
                {
                    ConnectionString = connectionString,
                    InitializationConnectionString = initializationConnectionString,
                    FeatureFlagAssemblies = featureFlagAssemblies,
                };
            });

            return service;
        }

        /// <summary>
        /// <para>Define the DI recipes for construction of strongly-typed <see cref="Feature"/> instances.</para>
        /// <para>A <see cref="IFeatureManagerSnapshot"/> implementation must be registered in order to
        /// construct individual <see cref="Feature"/> instances.</para>
        /// </summary>
        public static IServiceCollection AddFeatureFlags(
            this IServiceCollection service,
            IEnumerable<Assembly> featureFlagAssemblies = null
            )
        {
            featureFlagAssemblies ??= new List<Assembly>();
            var featureTypes = featureFlagAssemblies.GetFeatureTypesInAssemblies().ToList();
            foreach (var featureType in featureTypes)
            {
                service.AddScoped(featureType, serviceProvider
                    => serviceProvider.GetFeatureFromFeatureManager(featureType));
            }

            return service;
        }

        public static IServiceCollection AddSqlSessionManagerSettings(
            this IServiceCollection service
            )
        {
            service.AddScoped<SqlSessionManagerSettings>(services =>
            {
                var settings = services.GetRequiredService<FeatureFlagsSettings>();
                return new SQLServerSessionManagerSettings
                {
                    FeatureSchemaName = "dbo",
                    FeatureTableName = "RimDevAspNetCoreFeatureFlags",
                    FeatureNameColumn = "FeatureName",
                    FeatureValueColumn = "Enabled",
                    ConnectionString = settings.ConnectionString,
                    EnableSetValueCommand = false,
                };
            });

            return service;
        }

        public static IServiceCollection AddFeatureFlagsSessionManager(
            this IServiceCollection service
            )
        {
            service.AddScoped(services =>
            {
                var cachedSqlSessionManagerSettings = services.GetService<CachedSqlSessionManagerSettings>();
                var sqlSessionManagerSettings = services.GetRequiredService<SqlSessionManagerSettings>();

                return new FeatureFlagsSessionManager
                (
                    cachedSqlSessionManagerSettings: cachedSqlSessionManagerSettings,
                    sqlSessionManagerSettings: sqlSessionManagerSettings
                );
            });

            return service;
        }

        private static Feature GetFeatureFromFeatureManager(
            this IServiceProvider serviceProvider,
            Type featureType
            )
        {
            var featureManager = serviceProvider.GetRequiredService<IFeatureManagerSnapshot>();
            var value = featureManager.IsEnabledAsync(featureType.Name)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
            var feature = (Feature)Activator.CreateInstance(featureType)
                ?? throw new Exception($"Unable to create instance of {featureType.Name}.");
            feature.Enabled = value;
            return feature;
        }

        private static IEnumerable<Type> GetFeatureTypesInAssemblies(
            this IEnumerable<Assembly> featureFlagAssemblies)
        {
            return featureFlagAssemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(Feature)));
        }

        private static bool _sessionManagerInitialized;

        /// <summary>Create the feature flags table (must be done via a defensive SQL script)
        /// using <see cref="FeatureFlagsSettings"/> and <see cref="SqlSessionManagerSettings"/>
        /// which must have been registered in DI.
        /// </summary>
        public static IApplicationBuilder CreateFeatureFlagsTable(
            this IApplicationBuilder builder
            )
        {
            if (_sessionManagerInitialized) return builder;

            var settings = builder
                .ApplicationServices
                .GetRequiredService<FeatureFlagsSettings>();
            var sqlSettings = builder
                .ApplicationServices
                .GetRequiredService<SqlSessionManagerSettings>();
            sqlSettings.CreateDatabaseTable(settings.InitializationConnectionString);
            _sessionManagerInitialized = true;

            return builder;
        }
    }
}
