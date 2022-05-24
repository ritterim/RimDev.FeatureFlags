using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace RimDev.AspNetCore.FeatureFlags.UI
{
    public static class IEndpointRouteBuilderExtensions
    {
        public static IEndpointConventionBuilder MapFeatureFlagsUI(
            this IEndpointRouteBuilder builder,
            FeatureFlagOptions options = default(FeatureFlagOptions))
        {
            return builder.Map(
                options.UiPath + "/{**path}",
                async context =>
                {
                    var path = context.Request.Path;

                    if (path == options.ApiGetPath)
                    {
                        await FeatureFlagsBuilder.ApiGetPath(context, options);
                        return;
                    }

                    if (path == options.ApiGetAllPath)
                    {
                        await FeatureFlagsBuilder.ApiGetAllPath(context, options);
                        return;
                    }

                    if (path == options.ApiSetPath)
                    {
                        await FeatureFlagsBuilder.ApiSetPath(context, options);
                        return;
                    }

                    if (path == $"{options.UiPath}/main.js")
                    {
                        await context.Response.WriteManifestResource(typeof(IApplicationBuilderExtensions), "application/javascript", "main.js");
                        return;
                    }

                    if (path == options.UiPath)
                    {
                        await context.Response.WriteManifestResource(typeof(IApplicationBuilderExtensions), "text/html", "index.html");
                        return;
                    }
                });
        }
    }
}
