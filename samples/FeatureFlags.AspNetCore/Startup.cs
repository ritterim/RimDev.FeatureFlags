using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureFlags.AspNetCore
{
    public class Startup
    {
        private readonly FeatureFlagOptions options;

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            options = new FeatureFlagOptions()
                .UseInMemoryFeatureProvider();
                // .UseCachedSqlFeatureProvider(Configuration.GetConnectionString("localDb"));
        }

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


        }
    }
}
