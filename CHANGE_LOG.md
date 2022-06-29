# RimDev.FeatureFlags Change Log

## v3.0 May-July 2022

Major rewrite to move to using the `IFeatureManagerSnapshot` and `ISessionManager` interfaces from [Microsoft.FeatureManagement](https://www.nuget.org/packages/Microsoft.FeatureManagement/).  This will allow layering of `ISessionManager` implementations to give flexibility in how the feature value is calculated.

Under RimDev.FeatureFlags v2 you were limited to only looking at a single database table for values.  With RimDev.FeatureFlags v3, you can allow per-user feature flag values to override per-application values; or some other desired layering.  You can mix and match any set of value providers which implement the `ISessionManager` interface when constructing the `IFeatureManagerSnapshot` object in a Dependency Injection container.

You can also choose to use any reasonable implementation of `IFeatureManagerSnapshot` instead of [Microsoft.FeatureManagement](https://www.nuget.org/packages/Microsoft.FeatureManagement/) if you don't need the advanced features that the Microsoft implementation provides.  The out of the box experience uses [Lussatite.FeatureManagement](https://www.nuget.org/packages/Lussatite.FeatureManagement) which is a light implementation which allows layering of session managers.

The main package now targets .NET Standard 2.0, with an additional package (RimDev.AspNetCore.FeatureFlags.UI) added to provide the pre-built .NET Core 3.1+ / .NET 5+ web UI and API.

### Additions

- `FeatureFlagUiSettings`: Is the new settings class for the UI project classes.  Some of these properties used to live in `FeatureFlagOptions`.

### Changes

- All UI-related classes / methods have been moved to the UI package.
- The default ServiceLifetime for a Feature is now `Scoped` instead of `Transient`.  There is no longer a way to set the service lifetime.
- Building a `Feature` object now looks at the registered `IFeatureManagementSnapshot` to obtain the value.
- `FeatureSetRequest` is now `FeatureRequest` in the UI project.
- The description for a `Feature` now comes from the `[Description(string)]` attribute on the class, not from an overridden property.
- The "Value" property is now named "Enabled".
- Use of LazyCache `IAppCache` where appropriate.

### Removed / Obsoleted

- `Feature.ServiceLifetime` property.  All feature objects are constructed as `Scoped` lifetime.
- Some properties in `FeatureFlagOptions` related to the user-interface / API.  They have been moved to `FeatureFlagUiSettings`.
- Classes: `CachedSqlFeatureProvider`, `FeatureFlags`, `FeatureFlagsBuilder`, and `InMemoryFeatureProvider` have all been removed.
- Interfaces: `IFeatureProvider` has been removed.

## v2.2 May 2022

- Rework build process.
- Package upgrades to address vulnerable dependencies.
