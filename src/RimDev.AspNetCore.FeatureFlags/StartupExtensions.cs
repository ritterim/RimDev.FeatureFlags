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
            RimDevFeatureFlagsSettings settings = null
            )
        {
            settings ??= new RimDevFeatureFlagsSettings();

            foreach (var featureType in settings.FeatureFlagAssemblies.GetFeatureTypesInAssemblies())
            {
                service.AddScoped(featureType, serviceProvider =>
                {
                    var featureManager = serviceProvider.GetRequiredService<IFeatureManagerSnapshot>();
                    var value = featureManager.IsEnabledAsync(featureType.Name)
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult();;
                    var feature = (Feature)Activator.CreateInstance(featureType)
                        ?? throw new Exception($"Unable to create instance of {featureType.Name}.");
                    feature.Value = value;
                    return feature;
                });
            }

            return service;
        }

        public static IApplicationBuilder InitializeFeatureFlagsTable(
            this IApplicationBuilder builder
            )
        {
            var sessionManager = builder.ApplicationServices.GetRequiredService<RimDevFeatureFlagsSessionManager>();
            sessionManager.CreateDatabaseTable()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();;

            return builder;
        }

        private static IEnumerable<Type> GetFeatureTypesInAssemblies(
            this IEnumerable<Assembly> featureFlagAssemblies)
        {
            return featureFlagAssemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(Feature)));
        }
    }
}