using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LazyCache;
using Lussatite.FeatureManagement.SessionManagers.SqlClient;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace RimDev.AspNetCore.FeatureFlags
{
    public static class StartupExtensions
    {
        /// <summary><para>Register RimDev Feature Flags in the dependency system.</para>
        /// <para>The <see cref="IFeatureManagerSnapshot"/> implementation must be registered in order to
        /// construct individual <see cref="Feature"/> instances.</para>
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        /// <param name="configuration"><see cref="IConfiguration"/></param>
        /// <param name="featureFlagAssemblies">A list of assemblies to scan for classes inheriting from
        /// <see cref="Feature"/>.</param>
        /// <param name="connectionString">Connection string for SELECT/INSERT/DELETE operations.</param>
        /// <param name="initializationConnectionString">Connection string for the CREATE TABLE operation.</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        /// <exception cref="Exception">When the connection strings are missing/empty.</exception>
        public static IServiceCollection AddRimDevFeatureFlags(
            this IServiceCollection services,
            IConfiguration configuration,
            ICollection<Assembly> featureFlagAssemblies,
            string connectionString,
            string initializationConnectionString
            )
        {
            services.AddFeatureFlagSettings(
                configuration,
                featureFlagAssemblies: featureFlagAssemblies,
                connectionString: connectionString,
                initializationConnectionString: initializationConnectionString
                );
            services.AddStronglyTypedFeatureFlags(
                featureFlagAssemblies: featureFlagAssemblies
                );
            services.AddFeatureFlagsSessionManager();

            return services;
        }

        /// <summary>Add <see cref="FeatureFlagsSettings"/> to the DI container.  The two
        /// connection strings are hardcoded as "featureFlags" and "featureFlagsInitialization".
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/></param>
        /// <param name="configuration"><see cref="IConfiguration"/></param>
        /// <param name="featureFlagAssemblies">A list of assemblies to scan for classes inheriting from
        /// <see cref="Feature"/>.</param>
        /// <param name="connectionString">Connection string for SELECT/INSERT/DELETE operations.</param>
        /// <param name="initializationConnectionString">Connection string for the CREATE TABLE operation.</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        /// <exception cref="Exception">When the connection strings are missing/empty.</exception>
        public static IServiceCollection AddFeatureFlagSettings(
            this IServiceCollection services,
            IConfiguration configuration,
            IEnumerable<Assembly> featureFlagAssemblies,
            string connectionString,
            string initializationConnectionString
            )
        {
            services.AddSingleton(serviceProvider =>
            {
                if (string.IsNullOrEmpty(connectionString))
                    throw new ArgumentNullException(nameof(connectionString));

                if (string.IsNullOrEmpty(initializationConnectionString))
                    throw new ArgumentNullException(nameof(initializationConnectionString));

                return new FeatureFlagsSettings(featureFlagAssemblies)
                {
                    ConnectionString = connectionString,
                    InitializationConnectionString = initializationConnectionString,
                    SqlSessionManagerSettings = new SQLServerSessionManagerSettings
                    {
                        FeatureSchemaName = "dbo",
                        FeatureTableName = "RimDevAspNetCoreFeatureFlags",
                        FeatureNameColumn = "FeatureName",
                        FeatureValueColumn = "Enabled",
                        ConnectionString = connectionString,
                        EnableSetValueCommand = false,
                    }
                };
            });

            return services;
        }

        /// <summary>
        /// <para>Define the DI recipes for construction of strongly-typed <see cref="Feature"/> instances.</para>
        /// <para>A <see cref="IFeatureManagerSnapshot"/> implementation must be registered in order to
        /// construct individual <see cref="Feature"/> instances.</para>
        /// </summary>
        public static IServiceCollection AddStronglyTypedFeatureFlags(
            this IServiceCollection services,
            IEnumerable<Assembly> featureFlagAssemblies = null
            )
        {
            featureFlagAssemblies ??= new List<Assembly>();
            var featureTypes = featureFlagAssemblies.GetFeatureTypesInAssemblies().ToList();
            foreach (var featureType in featureTypes)
            {
                services.AddScoped(featureType, serviceProvider
                    => serviceProvider.GetFeatureFromFeatureManager(featureType));
            }

            return services;
        }

        public static IServiceCollection AddFeatureFlagsSessionManager(
            this IServiceCollection services
            )
        {
            services.AddSingleton(serviceProvider =>
            {
                var featureFlagsSettings = serviceProvider.GetRequiredService<FeatureFlagsSettings>();
                var appCache = serviceProvider.GetService<IAppCache>();

                return new FeatureFlagsSessionManager
                (
                    featureFlagsSettings: featureFlagsSettings,
                    appCache: appCache
                );
            });

            return services;
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

        public static IApplicationBuilder UseRimDevFeatureFlags(
            this IApplicationBuilder app
            )
        {
            app.CreateFeatureFlagsTable();
            return app;
        }

        private static bool _sessionManagerInitialized;

        /// <summary>Create the feature flags table (must be done via a defensive SQL script).</summary>
        public static IApplicationBuilder CreateFeatureFlagsTable(
            this IApplicationBuilder app
            )
        {
            if (_sessionManagerInitialized) return app;

            var featureFlagsSettings = app
                .ApplicationServices
                .GetRequiredService<FeatureFlagsSettings>();
            featureFlagsSettings.SqlSessionManagerSettings.CreateDatabaseTable(
                featureFlagsSettings.InitializationConnectionString
                );
            _sessionManagerInitialized = true;

            return app;
        }
    }
}
