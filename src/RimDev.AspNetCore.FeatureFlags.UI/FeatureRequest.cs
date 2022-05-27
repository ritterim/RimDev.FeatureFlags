using System;
using Newtonsoft.Json;

namespace RimDev.AspNetCore.FeatureFlags.UI
{
    public class FeatureRequest
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [Obsolete("Use Name property.")]
        [JsonProperty("feature")]
        public string Feature { get; set; }

        [Obsolete("Use the Enabled property.")]
        [JsonProperty("value")]
        public bool Value { get; set; }

        [JsonProperty("enabled")]
        public bool? Enabled { get; set; }
    }
}
