#if NETCOREAPP3_1

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace RimDev.AspNetCore.FeatureFlags
{
    public static class IEndpointRouteBuilderExtensions
    {
        public static IEndpointRouteBuilder MapFeatureFlagsUI(
            this IEndpointRouteBuilder builder,
            FeatureFlagOptions options = default(FeatureFlagOptions))
        {
            builder.Map(options.ApiGetPath, context => FeatureFlagsBuilder.ApiGetPath(context, options));
            builder.Map(options.ApiGetAllPath, context => FeatureFlagsBuilder.ApiGetAllPath(context, options));
            builder.Map(options.ApiSetPath, context => FeatureFlagsBuilder.ApiSetPath(context, options));

            builder.Map(
                $"{options.UiPath}/main.js",
                context => context.Response.WriteManifestResource(typeof(IApplicationBuilderExtensions), "application/javascript", "main.js"));

            builder.Map(
                options.UiPath,
                context => context.Response.WriteManifestResource(typeof(IApplicationBuilderExtensions), "text/html", "index.html"));

            return builder;
        }
    }
}

#endif
