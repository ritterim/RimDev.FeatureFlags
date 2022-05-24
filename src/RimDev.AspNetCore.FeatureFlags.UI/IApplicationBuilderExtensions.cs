using Microsoft.AspNetCore.Builder;

namespace RimDev.AspNetCore.FeatureFlags.UI
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseFeatureFlagsUI(
            this IApplicationBuilder builder,
            FeatureFlagOptions options = default(FeatureFlagOptions))
        {
            builder.Map(options.ApiGetPath, appBuilder =>
            {
                appBuilder.Run(context => FeatureFlagsBuilder.ApiGetPath(context, options));
            });

            builder.Map(options.ApiGetAllPath, appBuilder =>
            {
                appBuilder.Run(context => FeatureFlagsBuilder.ApiGetAllPath(context, options));
            });

            builder.Map(options.ApiSetPath, appBuilder =>
            {
                appBuilder.Run(context => FeatureFlagsBuilder.ApiSetPath(context, options));
            });

            builder.Map(options.UiPath, x =>
            {
                x.Map($"/main.js", y => y.Run(context => context.Response.WriteManifestResource(typeof(IApplicationBuilderExtensions), "application/javascript", "main.js")));
                x.Run(context => context.Response.WriteManifestResource(typeof(IApplicationBuilderExtensions), "text/html", "index.html"));
            });

            return builder;
        }
    }
}
