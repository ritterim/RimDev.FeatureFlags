using Newtonsoft.Json;

namespace RimDev.AspNetCore.FeatureFlags
{
    public abstract class Feature
    {
        [JsonProperty("name")]
        public string Name => this.GetType().Name;

        [JsonProperty("description")]
        public virtual string Description { get; }

        [JsonProperty("value")]
        public bool Value { get; set; }
    }
}
