using System.Collections.Generic;
using System.Reflection;

namespace RimDev.AspNetCore.FeatureFlags
{
    public class FeatureFlagsSettings
    {
        /// <summary>A SQL connection string which can be used to SELECT/INSERT/UPDATE
        /// from the feature flag values table.</summary>
        public string ConnectionString { get; set; }

        /// <summary>A SQL connection string which can be used to create a missing feature
        /// flag values table.</summary>
        public string InitializationConnectionString { get; set; }

        /// <summary>The list of assemblies to scan for classes which inherit from <see cref="Feature"/>.</summary>
        public IEnumerable<Assembly> FeatureFlagAssemblies { get; set; }
    }
}
