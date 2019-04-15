using RimDev.AspNetCore.FeatureFlags;

namespace FeatureFlags.AspNetCore
{
    public class TestFeature3 : FeatureValue<bool>
    {
        public override string Description { get; } = "Another test feature.";
    }
}
