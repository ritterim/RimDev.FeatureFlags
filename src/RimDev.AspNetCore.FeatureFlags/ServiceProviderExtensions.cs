using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace RimDev.AspNetCore.FeatureFlags
{
    public static class ServiceProviderExtensions
    {
        public static Feature GetFeatureFromFeatureManager(
            this IServiceProvider serviceProvider,
            Type featureType
            )
        {
            var featureManager = serviceProvider.GetRequiredService<IFeatureManagerSnapshot>();
            var value = featureManager.IsEnabledAsync(featureType.Name)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
            var feature = (Feature)Activator.CreateInstance(featureType)
                ?? throw new Exception($"Unable to create instance of {featureType.Name}.");
            feature.Enabled = value;
            return feature;
        }
    }
}
