using Newtonsoft.Json;

namespace RimDev.AspNetCore.FeatureFlags
{
    public abstract class Feature
    {
        [JsonProperty("name")]
        public string Name => this.GetType().Name;

        [JsonProperty("description")]
        public virtual string Description { get; }

        public object Value { get; set; }
    }

    public abstract class FeatureValue<T> : Feature
    {
        [JsonProperty("value")]
        public new T Value
        {
            get => base.Value is T ? (T) base.Value : default(T);
            set => base.Value = value;
        }
    }
}
