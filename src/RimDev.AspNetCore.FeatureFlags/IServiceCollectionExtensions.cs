using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Reflection;

namespace FeatureFlags
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddFeature<T>(this IServiceCollection serviceCollection)
            where T : Feature
        {
            serviceCollection.Add(new ServiceDescriptor(typeof(T), serviceProvider =>
            {
                var factory = serviceProvider.GetRequiredService<FeatureFlagsFactory>();

                var feature = factory
                    .Get<T>(serviceProvider)
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();

                return feature;
            }, ServiceLifetime.Transient));

            return serviceCollection;
        }

        public static IFeatureFlagsBuilder AddFeatureFlags(
            this IServiceCollection serviceCollection,
            IEnumerable<Assembly> featureAssemblies = null,
            IEnumerable<Assembly> conditionAssemblies = null)
        {
            serviceCollection.AddTransient<FeatureFlagsFactory>();

            var builder = new FeatureFlagsBuilder(serviceCollection);

            builder
                .AddFeatures(featureAssemblies)
                .AddConditions(conditionAssemblies);

            return builder;
        }
    }
}
