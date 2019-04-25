using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace RimDev.AspNetCore.FeatureFlags
{
    public abstract class Feature
    {
        [JsonProperty("name")]
        public string Name => this.GetType().Name;

        [JsonProperty("description")]
        public virtual string Description { get; }

        [JsonProperty("serviceLifetime")]
        public virtual ServiceLifetime ServiceLifetime { get; }
            = ServiceLifetime.Transient; // TODO: Default to Scoped in v2?

        [JsonProperty("value")]
        public bool Value { get; set; }
    }
}
