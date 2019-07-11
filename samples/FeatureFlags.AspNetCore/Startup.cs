using FeatureFlags.Attributes;
using FeatureFlags.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace FeatureFlags.AspNetCore
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddFeatureFlags()
                    .AddStore<InMemoryStore>("memory");
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Map("", configuration =>
            {
                configuration.Run(context =>
                {
                    var feature = context.RequestServices.GetService<TestFeature>();

                    return Task.CompletedTask;
                });
            });
        }

        [Feature(Name = nameof(TestFeature), Stores = new[] { "memory" })]
        [Condition(typeof(MyLabelTimeOfDayCondition), Required = new[] { "", "" }, Optional = new[] { "", "" })]
        public class TestFeature : Feature
        { }

        public class MyLabelTimeOfDayCondition : ICondition
        {
            public Task Apply(Feature feature)
            {
                if (feature is TestFeature tv)
                {
                    tv.Enabled = true;

                    return Task.FromResult(true);
                }
                else
                {
                    feature.Enabled = false;

                    return Task.FromResult(false);
                }
            }
        }
    }
}
