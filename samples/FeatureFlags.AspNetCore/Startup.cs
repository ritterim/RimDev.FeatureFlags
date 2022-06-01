using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RimDev.AspNetCore.FeatureFlags;
using System.Threading.Tasks;
using LazyCache;
using RimDev.AspNetCore.FeatureFlags.DbCommandFactories;
using RimDev.AspNetCore.FeatureFlags.UI;

namespace FeatureFlags.AspNetCore
{
    public class Startup
    {
        private static readonly IEnumerable<Assembly> FeatureFlagAssemblies = new[] { typeof(Startup).Assembly };

        private static readonly FeatureFlagUiSettings FeatureFlagUiSettings = new FeatureFlagUiSettings
        {
            FeatureFlagAssemblies = FeatureFlagAssemblies,
        };

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
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
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseFeatureFlags();
            app.UseFeatureFlagsUI();

            app.UseRouting();
        }
    }
}
