using System.Linq;
using System.Threading.Tasks;
using Lussatite.FeatureManagement;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using RimDev.AspNetCore.FeatureFlags;
using RimDev.AspNetCore.FeatureFlags.UI;

namespace FeatureFlags.AspNetCore
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRimDevFeatureFlags(
                configuration,
                new[] { typeof(Startup).Assembly }
                );

            services.AddScoped<IFeatureManagerSnapshot>(serviceProvider =>
            {
                var featureFlagSessionManager = serviceProvider.GetRequiredService<FeatureFlagsSessionManager>();
                var featureFlagsSettings = serviceProvider.GetRequiredService<FeatureFlagsSettings>();
                return new LussatiteLazyCacheFeatureManager(
                    featureFlagsSettings.FeatureFlagTypes.Select(x => x.Name).ToList(),
                    new []
                    {
                        featureFlagSessionManager
                    });
            });

            services.AddRimDevFeatureFlagsUi();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRimDevFeatureFlags();
            app.UseRimDevFeatureFlagsUi();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                var featureFlagUiSettings = app.ApplicationServices.GetService<FeatureFlagUiSettings>();

                endpoints.Map("/test-features", async context =>
                {
                    var testFeature = context.RequestServices.GetService<TestFeature>();
                    var testFeature2 = context.RequestServices.GetService<TestFeature2>();
                    var testFeature3 = context.RequestServices.GetService<TestFeature3>();

                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync($@"
                    {testFeature.GetType().Name}: {testFeature.Enabled}<br />
                    {testFeature2.GetType().Name}: {testFeature2.Enabled}<br />
                    {testFeature3.GetType().Name}: {testFeature3.Enabled}<br />
                    <a href=""{featureFlagUiSettings.UiPath}"">View UI</a>");
                });

                endpoints.Map("", context =>
                {
                    context.Response.Redirect("/test-features");

                    return Task.CompletedTask;
                });

                var featureFlagsSettings = app.ApplicationServices.GetRequiredService<FeatureFlagsSettings>();
                endpoints.MapFeatureFlagsUI(
                    uiSettings: featureFlagUiSettings,
                    settings: featureFlagsSettings
                    );
            });
        }
    }
}
