# RimDev.FeatureFlags

A library for strongly typed feature flags in ASP.NET Core.

![Screenshot](https://raw.githubusercontent.com/ritterim/RimDev.FeatureFlags/master/screenshot.png)

| Package                       | Version |
| ----------------------------- | ------- |
| [RimDev.AspNetCore.FeatureFlags][NuGet link] | [![RimDev.AspNetCore.FeatureFlags NuGet Version](https://img.shields.io/nuget/v/RimDev.AspNetCore.FeatureFlags.svg)][NuGet link] |

## Installation

Install the [RimDev.AspNetCore.FeatureFlags][NuGet link] NuGet package.

```
> dotnet add package RimDev.AspNetCore.FeatureFlags
```

or

```
PM> Install-Package RimDev.AspNetCore.FeatureFlags
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
        private readonly FeatureFlagOptions options;

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            options = new FeatureFlagOptions()
                .UseCachedSqlFeatureProvider(Configuration.GetConnectionString("localDb"));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFeatureFlags(options);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseFeatureFlags(options);

            // IMPORTANT: Controlling access of the UI / API of this library is the responsibility of the user.
            // Apply authentication / authorization around the `UseFeatureFlagsUI` method as needed,
            // as this method wires up the various endpoints.
            app.UseFeatureFlagsUI(options);

            // Or, with ASP.NET Core 3.1:
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

Next, create feature flags like this in the ASP.NET Core assembly:

```csharp
using RimDev.AspNetCore.FeatureFlags;

namespace MyApplication
{
    public class MyFeature : Feature
    {
        // Optional, displays on UI:
        public override string Description { get; } = "My feature description.";
    }
}
```

**Note:** `FeatureFlagAssemblies` to scan can also be configured in `FeatureFlagOptions` if you'd like to scan assemblies other than `Assembly.GetEntryAssembly()`.

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

The UI wired up by `UseFeatureFlagsUI` is available by default at `/_features`. The UI and API endpoints can be modified in `FeatureFlagOptions` if you'd like, too.

## License

MIT License

[NuGet link]: https://www.nuget.org/packages/RimDev.AspNetCore.FeatureFlags
