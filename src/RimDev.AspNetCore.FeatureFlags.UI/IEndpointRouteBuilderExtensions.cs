using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace RimDev.AspNetCore.FeatureFlags.UI
{
    public static class IEndpointRouteBuilderExtensions
    {
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
