using System.Collections.Generic;
using System.Reflection;

namespace RimDev.AspNetCore.FeatureFlags
{
    public class FeatureFlagOptions
    {
        public IEnumerable<Assembly> FeatureFlagAssemblies { get; set; }
            = new[] { Assembly.GetEntryAssembly() };

        public IFeatureProvider Provider { get; internal set; }
    }
}
