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
            services.AddFeatureFlagSettings(
                configuration,
                new[] { typeof(TestStartup).Assembly }
                );
            services.AddFeatureFlagUiSettings();
            services.AddFeatureFlags();
            services.AddSqlSessionManagerSettings();
            services.AddFeatureFlagsSessionManager();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.CreateFeatureFlagsTable();
            app.UseFeatureFlagsUI();
        }
    }
}
