using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace RimDev.AspNetCore.FeatureFlags.UI
{
    public static class UIStartupExtensions
    {
        public static IServiceCollection AddRimDevFeatureFlagsUI(
            this IServiceCollection service
            )
        {
            service.AddSingleton<FeatureFlagUISettings>();
            return service;
        }

        /// <summary>
        /// <para>Uses <see cref="FeatureFlagsUIBuilder"/> to construct the default UI and API endpoints for feature
        /// flags toggles.</para>
        /// <para>Note that if you layer session managers, the value retrieved for a particular feature
        /// may not match what is shown in this UI.  This UI/API only displays/updates the values for the
        /// <see cref="FeatureFlagsSessionManager"/> session manager.</para>
        /// <para>IMPORTANT: Controlling access of the UI / API of this library is the responsibility of the user.
        /// Apply authentication / authorization around the `UseFeatureFlagsUI` method as needed, as this method
        /// simply wires up the various endpoints.</para>
        /// </summary>
        public static IApplicationBuilder UseRimDevFeatureFlagsUI(
            this IApplicationBuilder app
            )
        {
            var settings = app.ApplicationServices.GetRequiredService<FeatureFlagsSettings>();
            var uiSettings = app.ApplicationServices.GetRequiredService<FeatureFlagUISettings>();

            var featureFlagsUIBuilder = new FeatureFlagsUIBuilder();

            app.Map(uiSettings.ApiGetPath, appBuilder =>
            {
                appBuilder.Run(context => featureFlagsUIBuilder.ApiGetPath(context, settings));
            });

            app.Map(uiSettings.ApiGetAllPath, appBuilder =>
            {
                appBuilder.Run(context => featureFlagsUIBuilder.ApiGetAllPath(context, settings));
            });

            app.Map(uiSettings.ApiSetPath, appBuilder =>
            {
                appBuilder.Run(context => featureFlagsUIBuilder.ApiSetPath(context, settings));
            });

            app.Map(uiSettings.UIPath, x =>
            {
                x.Map($"/main.js", y => y.Run(context => context.Response.WriteManifestResource(typeof(UIStartupExtensions), "application/javascript", "main.js")));
                x.Run(context => context.Response.WriteManifestResource(typeof(UIStartupExtensions), "text/html", "index.html"));
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
            FeatureFlagUISettings uiSettings = default(FeatureFlagUISettings)
            )
        {
            if (settings is null) throw new ArgumentNullException(nameof(settings));

            var featureFlagsUIBuilder = new FeatureFlagsUIBuilder();

            return builder.Map(
                uiSettings.UIPath + "/{**path}",
                async context =>
                {
                    var path = context.Request.Path;

                    if (path == uiSettings.ApiGetPath)
                    {
                        await featureFlagsUIBuilder.ApiGetPath(context, settings);
                        return;
                    }

                    if (path == uiSettings.ApiGetAllPath)
                    {
                        await featureFlagsUIBuilder.ApiGetAllPath(context, settings);
                        return;
                    }

                    if (path == uiSettings.ApiSetPath)
                    {
                        await featureFlagsUIBuilder.ApiSetPath(context, settings);
                        return;
                    }

                    if (path == $"{uiSettings.UIPath}/main.js")
                    {
                        await context.Response.WriteManifestResource(typeof(UIStartupExtensions), "application/javascript", "main.js");
                        return;
                    }

                    if (path == uiSettings.UIPath)
                    {
                        await context.Response.WriteManifestResource(typeof(UIStartupExtensions), "text/html", "index.html");
                        return;
                    }
                });
        }
    }
}
