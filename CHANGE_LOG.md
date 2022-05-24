# RimDev.FeatureFlags Change Log

## v3.0 May/June 2022

Major rewrite to move to using the `IFeatureManagerSnapshot` and `ISessionManager` interfaces from [Microsoft.FeatureManagement](https://www.nuget.org/packages/Microsoft.FeatureManagement/).  This will allow layering of `ISessionManager` implementations to give flexibility in how the feature value is calculated.

Under v2 you were limited to only looking at a sole database table for values.  With v3, you can allow per-user feature flag values to override per-application values; or some other desired layering.  You can mix and match any set of value providers that implement the `ISessionManager` interface when constructing the `IFeatureManagerSnapshot` object in a Dependency Injection container.

You can also choose to use any reasonable implementation of `IFeatureManagerSnapshot` instead of [Microsoft.FeatureManagement](https://www.nuget.org/packages/Microsoft.FeatureManagement/) if you don't need the advanced features that the Microsoft implementation provides.

The main package now only targets .NET Standard 2.0, with an additional package (RimDev.AspNetCore.FeatureFlags.UI) added to provide the pre-built web UI.

### Additions

- `bool Feature.IsEnabled` property replaces the `Feature.Value` property.

### Changes

- All UI-related classes / methods have been moved to the UI package.
- The default ServiceLifetime for a Feature is now `Scoped` instead of `Transient`.  There is no longer a way to set the service lifetime.  This may be revisited in the future with a custom attribute on the class that inherits from the `Feature` abstract class.
- Building a `Feature` object now looks at the registered `IFeatureManagementSnapshot` to obtain the value.

### Removed / Obsoleted

- `Feature.ServiceLifetime` property.  All feature objects are constructed as `Scoped` lifetime.
- `Feature.Value` property.  Use `Feature.IsEnabled`.

### Initial Plans

Rewrite to base much of the logic on top of Microsoft's FeatureManagement package.  More specifically to take a dependency on any reasonable implementation of IFeatureManagerSnapshot.

- IFeatureProvider replaced with Microsoft's ISessionManager
- Existing SQL feature provider reworked to be a read-write ISessionManager
- Other code takes a dependency on IFeatureManagerSnapshot, to allow layering of feature value providers
- UI changed to allow "not set" as a 3rd option instead of only true/false
- Possibly keep the ability to inject a strongly typed FeatureFlag

## v2.2 May 2022

- Rework build process.
- Package upgrades to address vulnerable dependencies.
