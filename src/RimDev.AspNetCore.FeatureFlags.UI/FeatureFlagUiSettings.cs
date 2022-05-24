using System.Collections.Generic;
using System.Reflection;

namespace RimDev.AspNetCore.FeatureFlags.UI
{
    public class FeatureFlagUiSettings
    {
        public string ApiGetAllPath => UiPath + "/get_all";

        public string ApiGetPath => UiPath + "/get";

        public string ApiSetPath => UiPath + "/set";

        public string UiPath => "/_features";

        public IEnumerable<Assembly> FeatureFlagAssemblies { get; set; }
            = new[] { Assembly.GetEntryAssembly() };
    }
}
