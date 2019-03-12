namespace RimDev.AspNetCore.FeatureFlags
{
    public abstract class Feature<TValue>
    {
        public virtual string Description { get; }

        public TValue Value { get; set; }
    }
}
