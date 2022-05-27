using Newtonsoft.Json;

namespace RimDev.AspNetCore.FeatureFlags.UI
{
    public class FeatureResponse
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("enabled")]
        public bool? Enabled { get; set; }
    }
}
