using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace RimDev.AspNetCore.FeatureFlags.UI
{
    internal class FeatureFlagsUiBuilder
    {
        internal async Task ApiGetPath(
            HttpContext context,
            FeatureFlagsSettings settings
            )
        {
            if (context.Request.Method != HttpMethods.Get)
            {
                context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                return;
            }

            var featureFlagsSessionManager = context.RequestServices.GetRequiredService<FeatureFlagsSessionManager>();

            var featureName = context.Request.Query["feature"];
            if (string.IsNullOrEmpty(featureName))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            var featureType = settings.FeatureFlagAssemblies.GetFeatureType(featureName);

            if (featureType is null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            var value = await featureFlagsSessionManager.GetAsync(featureName);

            var response = new FeatureResponse
            {
                Name = featureName,
                Description = featureType.GetDescription(),
                Enabled = value,
            };

            var json = JsonConvert.SerializeObject(response);

            await context.Response.WriteAsync(json).ConfigureAwait(false);
        }

        internal async Task ApiGetAllPath(
            HttpContext context,
            FeatureFlagsSettings settings
            )
        {
            if (context.Request.Method != HttpMethods.Get)
            {
                context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                return;
            }

            var sessionManager = context.RequestServices.GetService<FeatureFlagsSessionManager>();

            if (sessionManager == null)
                throw new InvalidOperationException(
                    $"{nameof(FeatureFlagsSessionManager)} must be registered via {nameof(UiStartupExtensions.UseFeatureFlagsUI)}()");

            var features = new List<object>();
            foreach (var featureType in settings.FeatureFlagAssemblies.GetFeatureTypes())
            {
                var featureName = featureType.Name;
                var enabled = await sessionManager.GetAsync(featureName);
                var featureResponse = new FeatureResponse
                {
                    Name = featureName,
                    Description = featureType.GetDescription(),
                    Enabled = enabled,
                };

                features.Add(featureResponse);
            }

            var json = JsonConvert.SerializeObject(features);

            await context.Response.WriteAsync(json).ConfigureAwait(false);
        }

        internal async Task ApiSetPath(
            HttpContext context,
            FeatureFlagsSettings settings
            )
        {
            if (context.Request.Method != HttpMethods.Post)
            {
                context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                return;
            }

            var featureFlagsSessionManager = context.RequestServices.GetRequiredService<FeatureFlagsSessionManager>();

            string requestString;
            using (var streamReader = new StreamReader(context.Request.Body))
            {
                requestString = await streamReader
                    .ReadToEndAsync()
                    .ConfigureAwait(false);
            }

            var setRequest = (FeatureRequest) JsonConvert.DeserializeObject(
                requestString,
                typeof(FeatureRequest)
                );

            if (setRequest is null)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            var featureType = settings.FeatureFlagAssemblies.GetFeatureType(setRequest.Name);

            if (featureType is null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            await featureFlagsSessionManager
                .SetNullableAsync(setRequest.Name, setRequest.Enabled)
                .ConfigureAwait(false);

            context.Response.StatusCode = StatusCodes.Status204NoContent;
        }
    }
}
