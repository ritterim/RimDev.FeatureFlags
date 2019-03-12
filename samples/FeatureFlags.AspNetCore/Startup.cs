using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using RimDev.AspNetCore.FeatureFlags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeatureFlags.AspNetCore
{
    public class Startup
    {
        private static readonly FeatureFlagOptions options = new FeatureFlagOptions
        {
            Provider = new InMemoryFeatureProvider()
            // Provider = new CachedSqlFeatureProvider(
            //     @"Data Source=(LocalDB)\v13.0;Database=FeatureFlags.AspNetCore;Integrated Security=True")
        };

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFeatureFlags(options);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseFeatureFlags(options);

            app.Run(async (context) =>
            {
                var featureFlags = context.RequestServices.GetService<RimDev.AspNetCore.FeatureFlags.FeatureFlags>();

                // TODO: Automatically wire up features
                var testToggleFeature = new TestToggleFeature
                {
                    Value = true
                };

                var testStringFeature = new TestStringFeature
                {
                    Value = "abc"
                };

                await featureFlags.Set(testToggleFeature);
                await featureFlags.Set(testStringFeature);

                var testToggleFeatureGet = await featureFlags.Get<TestToggleFeature>();
                var testStringFeatureGet = await featureFlags.Get<TestStringFeature>();

                await context.Response.WriteAsync(
                    $"{testToggleFeatureGet.GetType().Name}: {testToggleFeatureGet.Value}{Environment.NewLine}" +
                    $"{testStringFeatureGet.GetType().Name}: {testStringFeatureGet.Value}{Environment.NewLine}");
            });
        }
    }
}
