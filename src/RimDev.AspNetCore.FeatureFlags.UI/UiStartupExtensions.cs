using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace RimDev.AspNetCore.FeatureFlags.UI
{
    public static class UiStartupExtensions
    {
        public static IServiceCollection AddRimDevFeatureFlagsUi(
            this IServiceCollection service
            )
        {
            service.AddSingleton<FeatureFlagUiSettings>();
            return service;
        }

        /// <summary>
        /// <para>Uses <see cref="FeatureFlagsUiBuilder"/> to construct the default UI and API endpoints for feature
        /// flags toggles.</para>
        /// <para>Note that if you layer session managers, the value retrieved for a particular feature
        /// may not match what is shown in this UI.  This UI/API only displays/updates the values for the
        /// <see cref="FeatureFlagsSessionManager"/> session manager.</para>
        /// <para>IMPORTANT: Controlling access of the UI / API of this library is the responsibility of the user.
        /// Apply authentication / authorization around the `UseFeatureFlagsUI` method as needed, as this method
        /// simply wires up the various endpoints.</para>
        /// </summary>
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

        /// <summary>
        /// <para>Uses <see cref="IEndpointRouteBuilder"/> to map the UI/API endpoints.</para>
        /// <para>Note that if you layer session managers, the value retrieved for a particular feature
        /// may not match what is shown in this UI.  This UI/API only displays/updates the values for the
        /// <see cref="FeatureFlagsSessionManager"/> session manager.</para>
        /// <para>IMPORTANT: Controlling access of the UI / API of this library is the responsibility of the user.
        /// Apply authentication / authorization around the `UseFeatureFlagsUI` method as needed, as this method
        /// simply wires up the various endpoints.</para>
        /// </summary>
        public static IEndpointConventionBuilder MapFeatureFlagsUI(
            this IEndpointRouteBuilder builder,
            FeatureFlagsSettings settings,
            FeatureFlagUiSettings uiSettings = default(FeatureFlagUiSettings)
            )
        {
            if (settings is null) throw new ArgumentNullException(nameof(settings));

            var featureFlagsUiBuilder = new FeatureFlagsUiBuilder();

            return builder.Map(
                uiSettings.UiPath + "/{**path}",
                async context =>
                {
                    var path = context.Request.Path;

                    if (path == uiSettings.ApiGetPath)
                    {
                        await featureFlagsUiBuilder.ApiGetPath(context, settings);
                        return;
                    }

                    if (path == uiSettings.ApiGetAllPath)
                    {
                        await featureFlagsUiBuilder.ApiGetAllPath(context, settings);
                        return;
                    }

                    if (path == uiSettings.ApiSetPath)
                    {
                        await featureFlagsUiBuilder.ApiSetPath(context, settings);
                        return;
                    }

                    if (path == $"{uiSettings.UiPath}/main.js")
                    {
                        await context.Response.WriteManifestResource(typeof(UiStartupExtensions), "application/javascript", "main.js");
                        return;
                    }

                    if (path == uiSettings.UiPath)
                    {
                        await context.Response.WriteManifestResource(typeof(UiStartupExtensions), "text/html", "index.html");
                        return;
                    }
                });
        }
    }
}
