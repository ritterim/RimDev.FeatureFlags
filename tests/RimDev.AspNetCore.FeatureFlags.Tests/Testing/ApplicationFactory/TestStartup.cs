using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RimDev.AspNetCore.FeatureFlags.UI;

namespace RimDev.AspNetCore.FeatureFlags.Tests.Testing.ApplicationFactory
{
    public class TestStartup
    {
        private readonly IConfiguration configuration;

        public TestStartup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var featureFlagsConnectionString
                = configuration.GetConnectionString("featureFlags");
            var featureFlagsInitializationConnectionString
                = configuration.GetConnectionString("featureFlagsInitialization");

            services.AddRimDevFeatureFlags(
                configuration,
                new[] { typeof(TestStartup).Assembly },
                connectionString: featureFlagsConnectionString,
                initializationConnectionString: featureFlagsInitializationConnectionString
            );

            services.AddRimDevFeatureFlagsUi();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRimDevFeatureFlags();
            app.UseRimDevFeatureFlagsUi();
        }
    }
}
