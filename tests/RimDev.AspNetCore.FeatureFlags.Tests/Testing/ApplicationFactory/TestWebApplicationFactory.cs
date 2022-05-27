using System.Collections.Generic;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using RimDev.AspNetCore.FeatureFlags.Tests.Testing.Database;

namespace RimDev.AspNetCore.FeatureFlags.Tests.Testing.ApplicationFactory
{
    public class TestWebApplicationFactory : WebApplicationFactory<TestStartup>
    {
        public EmptyDatabaseFixture DatabaseFixture => new EmptyDatabaseFixture();

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost
                .CreateDefaultBuilder()
                .UseStartup<TestStartup>();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseContentRoot(".");

            builder.ConfigureAppConfiguration(configuration =>
            {
                configuration.AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "connectionStrings:featureFlags", DatabaseFixture.ConnectionString },
                    { "connectionStrings:featureFlagsInitialization", DatabaseFixture.ConnectionString },
                });
            });

            base.ConfigureWebHost(builder);
        }
    }
}
