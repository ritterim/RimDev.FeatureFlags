using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace RimDev.AspNetCore.FeatureFlags
{
    public class FeatureFlagOptions
    {
        public string ApiGetAllPath { get; set; } = "/_features_get_all";

        public string ApiGetPath { get; set; } = "/_features_get";

        public string ApiSetPath { get; set; } = "/_features_set";

        public string UiPath { get; set; } = "/_features";

        public IEnumerable<Assembly> FeatureFlagAssemblies { get; set; }
            = new[] { Assembly.GetEntryAssembly() };

        public ServiceLifetime FeatureLifetime { get; set; } = ServiceLifetime.Transient;

        public IFeatureProvider Provider { get; internal set; }
    }
}
