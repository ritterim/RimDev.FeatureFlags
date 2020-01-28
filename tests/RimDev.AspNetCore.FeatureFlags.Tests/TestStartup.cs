using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace RimDev.AspNetCore.FeatureFlags.Tests
{
    public class TestStartup
    {
        public static readonly FeatureFlagOptions Options = new FeatureFlagOptions
        {
            FeatureFlagAssemblies = new[] { typeof(TestStartup).Assembly },
        }.UseInMemoryFeatureProvider();

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFeatureFlags(Options);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseFeatureFlags(Options);

            /**
             * Intentionally included to confirm feature-flag provider runs outside request-pipeline.
             * Otherwise, this will fail during app. startup since provider is not initialized.
             */
            app.ApplicationServices.GetRequiredService<TestFeature>();

            app.UseFeatureFlagsUI(Options);
        }
    }
}
