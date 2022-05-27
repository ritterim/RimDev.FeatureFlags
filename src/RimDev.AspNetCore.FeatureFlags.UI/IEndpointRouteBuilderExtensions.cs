using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace RimDev.AspNetCore.FeatureFlags.UI
{
    public static class IEndpointRouteBuilderExtensions
    {
        public static IEndpointConventionBuilder MapFeatureFlagsUI(
            this IEndpointRouteBuilder builder,
            FeatureFlagUiSettings settings = default(FeatureFlagUiSettings))
        {
            var featureFlagsUiBuilder = new FeatureFlagsUiBuilder();

            return builder.Map(
                settings.UiPath + "/{**path}",
                async context =>
                {
                    var path = context.Request.Path;

                    if (path == settings.ApiGetPath)
                    {
                        await featureFlagsUiBuilder.ApiGetPath(context, settings);
                        return;
                    }

                    if (path == settings.ApiGetAllPath)
                    {
                        await featureFlagsUiBuilder.ApiGetAllPath(context, settings);
                        return;
                    }

                    if (path == settings.ApiSetPath)
                    {
                        await featureFlagsUiBuilder.ApiSetPath(context, settings);
                        return;
                    }

                    if (path == $"{settings.UiPath}/main.js")
                    {
                        await context.Response.WriteManifestResource(typeof(UiStartupExtensions), "application/javascript", "main.js");
                        return;
                    }

                    if (path == settings.UiPath)
                    {
                        await context.Response.WriteManifestResource(typeof(UiStartupExtensions), "text/html", "index.html");
                        return;
                    }
                });
        }
    }
}
