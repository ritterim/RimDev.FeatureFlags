using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        }
    }
}
