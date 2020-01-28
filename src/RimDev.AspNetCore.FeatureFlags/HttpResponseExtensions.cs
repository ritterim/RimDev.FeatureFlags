using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace RimDev.AspNetCore.FeatureFlags
{
    internal static class HttpResponseExtensions
    {
        internal static async Task WriteManifestResource(this HttpResponse response, Type type, string contentType, string name)
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
