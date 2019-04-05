using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace RimDev.AspNetCore.FeatureFlags
{
    public static class IApplicationBuilderExtensions
    {
        private static bool providerInitialized;

        public static IApplicationBuilder UseFeatureFlags(
            this IApplicationBuilder builder,
            FeatureFlagOptions options = default(FeatureFlagOptions))
        {
            builder.Use(async (context, next) =>
            {
                if (!providerInitialized)
                {
                    await options.Provider.Initialize().ConfigureAwait(false);
                    providerInitialized = true;
                }

                await next();
            });

            return builder;
        }

        public static IApplicationBuilder UseFeatureFlagsUI(
            this IApplicationBuilder builder,
            FeatureFlagOptions options = default(FeatureFlagOptions))
        {
            builder.Map(options.ApiGetPath, appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    if (context.Request.Method != HttpMethods.Get)
                    {
                        context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                        return;
                    }

                    var featureFlags = context.RequestServices.GetService<FeatureFlags>();

                    if (featureFlags == null)
                        throw new InvalidOperationException(
                            $"{nameof(FeatureFlags)} must be registered via AddFeatureFlags()");

                    var featureName = context.Request.Query["feature"];
                    if (string.IsNullOrEmpty(featureName))
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        return;
                    }

                    var featureType = options.FeatureFlagAssemblies.GetFeatureType(featureName);

                    if (featureType == null)
                    {
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        return;
                    }

                    var feature = await featureFlags.Get(featureType).ConfigureAwait(false);

                    var json = JsonConvert.SerializeObject(feature);

                    await context.Response.WriteAsync(json).ConfigureAwait(false);
                });
            });

            builder.Map(options.ApiGetAllPath, appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    if (context.Request.Method != HttpMethods.Get)
                    {
                        context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                        return;
                    }

                    var featureFlags = context.RequestServices.GetService<FeatureFlags>();

                    if (featureFlags == null)
                        throw new InvalidOperationException(
                            $"{nameof(FeatureFlags)} must be registered via AddFeatureFlags()");

                    var features = new List<object>();
                    foreach (var featureType in options.FeatureFlagAssemblies.GetFeatureTypes())
                    {
                        var feature = await featureFlags.Get(featureType).ConfigureAwait(false);

                        features.Add(feature);
                    }

                    var json = JsonConvert.SerializeObject(features);

                    await context.Response.WriteAsync(json).ConfigureAwait(false);
                });
            });

            builder.Map(options.ApiSetPath, appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    if (context.Request.Method != HttpMethods.Post)
                    {
                        context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                        return;
                    }

                    var featureFlags = context.RequestServices.GetService<FeatureFlags>();

                    if (featureFlags == null)
                        throw new InvalidOperationException(
                            $"{nameof(FeatureFlags)} must be registered via AddFeatureFlags()");

                    string requestString;
                    using (var streamReader = new StreamReader(context.Request.Body))
                    {
                        requestString = await streamReader
                            .ReadToEndAsync()
                            .ConfigureAwait(false);
                    }

                    var setRequest = (FeatureSetRequest)JsonConvert.DeserializeObject(
                        requestString, typeof(FeatureSetRequest));

                    var featureType = options.FeatureFlagAssemblies.GetFeatureType(setRequest.Feature);

                    if (featureType == null)
                    {
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        return;
                    }

                    var feature = Activator.CreateInstance(featureType);

                    (feature as Feature).Value = setRequest.Value;

                    await featureFlags.Set(feature).ConfigureAwait(false);

                    context.Response.StatusCode = StatusCodes.Status204NoContent;
                });
            });

            builder.Map(options.UiPath, x =>
            {
                x.Map($"/main.js", y => y.Run(context => context.Response.WriteManifestResource(typeof(IApplicationBuilderExtensions), "application/javascript", "main.js")));
                x.Run(context => context.Response.WriteManifestResource(typeof(IApplicationBuilderExtensions), "text/html", "index.html"));
            });

            return builder;
        }

        static async Task WriteManifestResource(this HttpResponse response, Type type, string contentType, string name)
        {
            using (var stream = type.Assembly.GetManifestResourceStream(type, name))
            {
                if (stream == null)
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    return;
                }
                response.ContentType = contentType;
                await stream.CopyToAsync(response.Body).ConfigureAwait(false);
            }
        }
    }
}
