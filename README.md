# RimDev.FeatureFlags

A library for strongly typed feature flags in ASP.NET Core.

![Screenshot](https://raw.githubusercontent.com/ritterim/RimDev.FeatureFlags/master/screenshot.png)

| Package                                                                                               | Version                                                                                                                  |
|-------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------|
| [RimDev.AspNetCore.FeatureFlags](https://www.nuget.org/packages/RimDev.AspNetCore.FeatureFlags)       | ![RimDev.AspNetCore.FeatureFlags NuGet Version](https://img.shields.io/nuget/v/RimDev.AspNetCore.FeatureFlags.svg)       |
| [RimDev.AspNetCore.FeatureFlags.UI](https://www.nuget.org/packages/RimDev.AspNetCore.FeatureFlags.UI) | ![RimDev.AspNetCore.FeatureFlags.UI NuGet Version](https://img.shields.io/nuget/v/RimDev.AspNetCore.FeatureFlags.UI.svg) |

## Installation

Install the [RimDev.AspNetCore.FeatureFlags][NuGet link] and (optional) [RimDev.AspNetCore.FeatureFlags.UI][NuGet UI link] NuGet packages.

```
> dotnet add package RimDev.AspNetCore.FeatureFlags
> dotnet add package RimDev.AspNetCore.FeatureFlags.UI
```

or

```
PM> Install-Package RimDev.AspNetCore.FeatureFlags
PM> Install-Package RimDev.AspNetCore.FeatureFlags.UI
```

## Usage

You'll need to wire up `Startup.cs` as follows:

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RimDev.AspNetCore.FeatureFlags;

namespace MyApplication
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
            var featureFlagsConnectionString
                = configuration.GetConnectionString("featureFlags");
            var featureFlagsInitializationConnectionString
                = configuration.GetConnectionString("featureFlagsInitialization");

            services.AddRimDevFeatureFlags(
                configuration,
                new[] { typeof(Startup).Assembly },
                connectionString: featureFlagsConnectionString,
                initializationConnectionString: featureFlagsInitializationConnectionString
                );

            // IFeatureManagerSnapshot should always be scoped / per-request lifetime
            services.AddScoped<IFeatureManagerSnapshot>(serviceProvider =>
            {
                var featureFlagSessionManager = serviceProvider.GetRequiredService<FeatureFlagsSessionManager>();
                var featureFlagsSettings = serviceProvider.GetRequiredService<FeatureFlagsSettings>();
                return new LussatiteLazyCacheFeatureManager(
                    featureFlagsSettings.FeatureFlagTypes.Select(x => x.Name).ToList(),
                    new []
                    {
                        // in other use cases, you might list multiple ISessionManager objects to have layers
                        featureFlagSessionManager
                    });
            });

            services.AddRimDevFeatureFlagsUi();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseFeatureFlags(options);

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                // IMPORTANT: Controlling access of the UI / API of this library is the responsibility of the user.
                // Apply authentication / authorization around the `UseFeatureFlagsUI` method as needed,
                // as this method wires up the various endpoints.
                endpoints.MapFeatureFlagsUI(options);
            });
        }
    }
}
```

Next, create feature flags like this in the assemblies passed to `AddRimDevFeatureFlags()`:

```csharp
using RimDev.AspNetCore.FeatureFlags;

namespace MyApplication
{
    [Description("My feature description.")] // Optional displays on the UI
    public class MyFeature : Feature
    {
        // Feature classes could include other static information if desired by your application.
    }
}
```

**Now you can dependency inject any of your feature flags using the standard ASP.NET Core IoC!**

```csharp
public class MyController : Controller
{
    private readonly MyFeature myFeature;

    public MyController(MyFeature myFeature)
    {
        this.myFeature = myFeature;
    }

    // Use myFeature instance here, using myFeature.Value for the on/off toggle value.
}
```

## UI

The UI wired up by `UseFeatureFlagsUI` is available by default at `/_features`. The UI and API endpoints can be modified in `FeatureFlagUiSettings` if you'd like, too.

## License

MIT License
