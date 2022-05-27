using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace RimDev.AspNetCore.FeatureFlags.UI
{
    public static class UiStartupExtensions
    {
        public static IApplicationBuilder UseFeatureFlagsUI(
            this IApplicationBuilder builder
            )
        {
            var settings = builder.ApplicationServices.GetRequiredService<FeatureFlagUiSettings>();
            var featureFlagsUiBuilder = new FeatureFlagsUiBuilder();

            builder.Map(settings.ApiGetPath, appBuilder =>
            {
                appBuilder.Run(context => featureFlagsUiBuilder.ApiGetPath(context, settings));
            });

            builder.Map(settings.ApiGetAllPath, appBuilder =>
            {
                appBuilder.Run(context => featureFlagsUiBuilder.ApiGetAllPath(context, settings));
            });

            builder.Map(settings.ApiSetPath, appBuilder =>
            {
                appBuilder.Run(context => featureFlagsUiBuilder.ApiSetPath(context, settings));
            });

            builder.Map(settings.UiPath, x =>
            {
                x.Map($"/main.js", y => y.Run(context => context.Response.WriteManifestResource(typeof(UiStartupExtensions), "application/javascript", "main.js")));
                x.Run(context => context.Response.WriteManifestResource(typeof(UiStartupExtensions), "text/html", "index.html"));
            });

            return builder;
        }
    }
}
