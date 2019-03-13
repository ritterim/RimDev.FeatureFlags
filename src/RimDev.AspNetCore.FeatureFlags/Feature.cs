namespace RimDev.AspNetCore.FeatureFlags
{
    public abstract class Feature
    {
        public virtual string Description { get; }

        public bool Value { get; set; }
    }
}
