using System;
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

        [Obsolete("All Feature objects are now registered as Scoped.")]
        [JsonProperty("serviceLifetime")]
        public virtual ServiceLifetime ServiceLifetime { get; }
            = ServiceLifetime.Transient;

        [Obsolete("Use the Enabled property.")]
        [JsonProperty("value")]
        public bool Value { get; set; }

        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
    }
}
