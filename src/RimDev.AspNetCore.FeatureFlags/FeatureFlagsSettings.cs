using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lussatite.FeatureManagement.SessionManagers;

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

        /// <summary>How long a cache entry will be valid until it is forced to
        /// refresh from the database.  Defaults to 60 seconds.</summary>
        public TimeSpan CacheTime { get; set; } = TimeSpan.FromSeconds(60.0);

        /// <summary>The <see cref="SqlSessionManagerSettings"/> object which lets the
        /// <see cref="FeatureFlagsSessionManager"/> communicate with a database backend.
        /// </summary>
        public SqlSessionManagerSettings SqlSessionManagerSettings { get; set; }
    }
}
