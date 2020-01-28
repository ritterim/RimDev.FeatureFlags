using System.Collections.Generic;
using System.Reflection;

namespace RimDev.AspNetCore.FeatureFlags
{
    public class FeatureFlagOptions
    {
        public string ApiGetAllPath => UiPath + "/get_all";

        public string ApiGetPath => UiPath + "/get";

        public string ApiSetPath => UiPath + "/set";

        public string UiPath => "/_features";

        public IEnumerable<Assembly> FeatureFlagAssemblies { get; set; }
            = new[] { Assembly.GetEntryAssembly() };

        public IFeatureProvider Provider { get; internal set; }
    }
}
