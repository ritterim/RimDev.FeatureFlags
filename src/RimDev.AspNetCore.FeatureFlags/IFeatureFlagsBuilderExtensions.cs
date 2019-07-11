using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FeatureFlags
{
    public static class IFeatureFlagsBuilderExtensions
    {
        public static IFeatureFlagsBuilder AddStore<TStore>(this IFeatureFlagsBuilder builder, string name)
            where TStore : class, IStore
        {
            builder.ServiceCollection.AddTransient<IStore, TStore>();
            builder.ServiceCollection.AddSingleton<IStorePointer>(new StorePointer(name, typeof(TStore)));

            return builder;
        }

        internal static IFeatureFlagsBuilder AddConditions(this IFeatureFlagsBuilder builder, IEnumerable<Assembly> assemblies = null)
        {
            var conditions = (assemblies ?? new[] { Assembly.GetEntryAssembly() }).SelectMany(x => x.GetTypes().Where(y => !y.IsAbstract && typeof(ICondition).IsAssignableFrom(y))).ToList();

            foreach (var condition in conditions)
            {
                builder.ServiceCollection.AddTransient(typeof(ICondition), condition);
            }

            return builder;
        }

        internal static IFeatureFlagsBuilder AddFeatures(this IFeatureFlagsBuilder builder, IEnumerable<Assembly> assemblies = null)
        {
            var features = (assemblies ?? new[] { Assembly.GetEntryAssembly() }).SelectMany(x => x.GetTypes().Where(y => !y.IsAbstract && typeof(Feature).IsAssignableFrom(y))).ToList();
            var addFeatureMethod = typeof(IServiceCollectionExtensions).GetMethod(nameof(IServiceCollectionExtensions.AddFeature));

            foreach (var feature in features)
            {
                addFeatureMethod.MakeGenericMethod(feature).Invoke(builder.ServiceCollection, new[] { builder.ServiceCollection });
            }

            return builder;
        }
    }
}
