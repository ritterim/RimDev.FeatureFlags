using Microsoft.AspNetCore.Builder;

namespace RimDev.AspNetCore.FeatureFlags.UI
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseFeatureFlagsUI(
            this IApplicationBuilder builder,
            FeatureFlagUiSettings settings = default(FeatureFlagUiSettings))
        {
            settings ??= new FeatureFlagUiSettings();

            builder.Map(settings.ApiGetPath, appBuilder =>
            {
                appBuilder.Run(context => FeatureFlagsUiBuilder.ApiGetPath(context, settings));
            });

            builder.Map(settings.ApiGetAllPath, appBuilder =>
            {
                appBuilder.Run(context => FeatureFlagsUiBuilder.ApiGetAllPath(context, settings));
            });

            builder.Map(settings.ApiSetPath, appBuilder =>
            {
                appBuilder.Run(context => FeatureFlagsUiBuilder.ApiSetPath(context, settings));
            });

            builder.Map(settings.UiPath, x =>
            {
                x.Map($"/main.js", y => y.Run(context => context.Response.WriteManifestResource(typeof(IApplicationBuilderExtensions), "application/javascript", "main.js")));
                x.Run(context => context.Response.WriteManifestResource(typeof(IApplicationBuilderExtensions), "text/html", "index.html"));
            });

            return builder;
        }
    }
}
