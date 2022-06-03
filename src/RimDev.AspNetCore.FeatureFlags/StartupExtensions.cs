using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace RimDev.AspNetCore.FeatureFlags
{
    public static class StartupExtensions
    {
        /// <summary>
        /// <para>Define the DI recipes for construction of strongly-typed <see cref="Feature"/> instances.</para>
        /// <para>A <see cref="IFeatureManagerSnapshot"/> implementation must be registered in order to
        /// construct individual <see cref="Feature"/> instances.</para>
        /// </summary>
        public static IServiceCollection AddFeatureFlags(
            this IServiceCollection service,
            IEnumerable<Assembly> featureFlagAssemblies = null
            )
        {
            featureFlagAssemblies ??= new List<Assembly>();

            foreach (var featureType in featureFlagAssemblies.GetFeatureTypesInAssemblies())
            {
                service.AddScoped(featureType, serviceProvider
                    => serviceProvider.GetFeatureFromFeatureManager(featureType));
            }

            return service;
        }

        private static IEnumerable<Type> GetFeatureTypesInAssemblies(
            this IEnumerable<Assembly> featureFlagAssemblies)
        {
            return featureFlagAssemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(Feature)));
        }

        private static bool _sessionManagerInitialized;

        public static IApplicationBuilder UseFeatureFlags(
            this IApplicationBuilder builder
            )
        {
            if (!_sessionManagerInitialized)
            {
                var featureFlagSessionManager = builder
                    .ApplicationServices
                    .GetRequiredService<FeatureFlagsSessionManager>();
                featureFlagSessionManager.CreateDatabaseTable()
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
                _sessionManagerInitialized = true;
            }

            return builder;
        }
    }
}
