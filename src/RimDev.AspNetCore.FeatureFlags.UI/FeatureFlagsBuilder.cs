using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace RimDev.AspNetCore.FeatureFlags.UI
{
    internal static class FeatureFlagsBuilder
    {
        internal static async Task ApiGetPath(HttpContext context, FeatureFlagOptions options)
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
        }

        internal static async Task ApiGetAllPath(HttpContext context, FeatureFlagOptions options)
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
        }

        internal static async Task ApiSetPath(HttpContext context, FeatureFlagOptions options)
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
        }
    }
}
