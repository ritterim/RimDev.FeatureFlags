using System;
using System.Collections.Generic;
using System.Reflection;
using LazyCache;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RimDev.AspNetCore.FeatureFlags.DbCommandFactories;
using RimDev.AspNetCore.FeatureFlags.UI;

namespace RimDev.AspNetCore.FeatureFlags.Tests.Testing.ApplicationFactory
{
    public class TestStartup
    {
        private static readonly IEnumerable<Assembly> FeatureFlagAssemblies = new[] { typeof(TestStartup).Assembly };

        internal static readonly FeatureFlagUiSettings FeatureFlagUiSettings = new FeatureFlagUiSettings
        {
            FeatureFlagAssemblies = FeatureFlagAssemblies,
        };

        public IConfiguration Configuration { get; }

        public TestStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(FeatureFlagUiSettings);
            services.AddSingleton<IAppCache>(new CachingService());
            services.AddSingleton(new FeatureFlagsSettings());

            services.AddSingleton(provider =>
            {
                var settings = provider.GetRequiredService<FeatureFlagsSettings>();

                const string connectionStringName = "featureFlags";
                var connectionString = Configuration.GetConnectionString(connectionStringName);
                if (string.IsNullOrEmpty(connectionString))
                    throw new Exception($"Failed to retrieve connection string: {connectionStringName}");

                const string initializationConnectionStringName = "featureFlagsInitialization";
                var initializationConnectionString = Configuration.GetConnectionString(initializationConnectionStringName);
                if (string.IsNullOrEmpty(connectionString))
                    throw new Exception($"Failed to retrieve initialization connection string: {initializationConnectionString}");

                return new FeatureFlagsSessionManager(
                    cache: provider.GetService<IAppCache>(),
                    dbFunctionFactory: new FeatureFlagsMsSqlDbFunctionFactory(
                        connectionString: connectionString,
                        initializationConnectionString: initializationConnectionString
                        ),
                    settings: settings
                    );
            });

            services.AddFeatureFlags(
                featureFlagAssemblies: FeatureFlagAssemblies
                );
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseFeatureFlags();
            app.UseFeatureFlagsUI();
        }
    }
}
