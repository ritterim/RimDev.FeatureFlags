using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace RimDev.AspNetCore.FeatureFlags.UI
{
    public static class UiStartupExtensions
    {
        public static IApplicationBuilder UseRimDevFeatureFlagsUi(
            this IApplicationBuilder app
            )
        {
            var settings = app.ApplicationServices.GetRequiredService<FeatureFlagsSettings>();
            var uiSettings = app.ApplicationServices.GetRequiredService<FeatureFlagUiSettings>();

            var featureFlagsUiBuilder = new FeatureFlagsUiBuilder();

            app.Map(uiSettings.ApiGetPath, appBuilder =>
            {
                appBuilder.Run(context => featureFlagsUiBuilder.ApiGetPath(context, settings));
            });

            app.Map(uiSettings.ApiGetAllPath, appBuilder =>
            {
                appBuilder.Run(context => featureFlagsUiBuilder.ApiGetAllPath(context, settings));
            });

            app.Map(uiSettings.ApiSetPath, appBuilder =>
            {
                appBuilder.Run(context => featureFlagsUiBuilder.ApiSetPath(context, settings));
            });

            app.Map(uiSettings.UiPath, x =>
            {
                x.Map($"/main.js", y => y.Run(context => context.Response.WriteManifestResource(typeof(UiStartupExtensions), "application/javascript", "main.js")));
                x.Run(context => context.Response.WriteManifestResource(typeof(UiStartupExtensions), "text/html", "index.html"));
            });

            return app;
        }
    }
}
