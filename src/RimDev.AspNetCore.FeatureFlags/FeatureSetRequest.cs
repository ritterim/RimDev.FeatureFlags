using Newtonsoft.Json;

namespace RimDev.AspNetCore.FeatureFlags
{
    public class FeatureSetRequest
    {
        [JsonProperty("feature")]
        public string Feature { get; set; }

        [JsonProperty("value")]
        public bool Value { get; set; }
    }
}
