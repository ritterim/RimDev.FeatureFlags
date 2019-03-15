using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RimDev.AspNetCore.FeatureFlags
{
    public class FeatureFlagOptions
    {
        public string ApiGetAllPath { get; set; } = "/_features_get_all";

        public string ApiGetPath { get; set; } = "/_features_get";

        public string ApiSetPath { get; set; } = "/_features_set";

        public string UiPath { get; set; } = "/_features";

        public IEnumerable<Assembly> FeatureFlagAssemblies { get; set; }
            = new[] { Assembly.GetExecutingAssembly() };

        public IFeatureProvider Provider { get; set; }
    }
}
