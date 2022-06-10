using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RimDev.AspNetCore.FeatureFlags
{
    public class FeatureFlagsSettings
    {
        public FeatureFlagsSettings(IEnumerable<Assembly> featureFlagAssemblies)
        {
            FeatureFlagTypes = featureFlagAssemblies.GetFeatureTypes().ToList();
        }

        /// <summary>A SQL connection string which can be used to SELECT/INSERT/UPDATE
        /// from the feature flag values table.</summary>
        public string ConnectionString { get; set; }

        /// <summary>A SQL connection string which can be used to create a missing feature
        /// flag values table.</summary>
        public string InitializationConnectionString { get; set; }

        /// <summary>The list of types found by initial assembly scanning.</summary>
        public IReadOnlyCollection<Type> FeatureFlagTypes { get; }

        public Type GetFeatureType(
            string featureName
            )
        {
            return FeatureFlagTypes.SingleOrDefault(x =>
                x.Name.Equals(featureName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
