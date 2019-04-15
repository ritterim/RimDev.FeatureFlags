using RimDev.AspNetCore.FeatureFlags;

namespace FeatureFlags.AspNetCore
{
    public class TestFeature : FeatureValue<bool>
    {
        public override string Description { get; } = "A test feature.";
    }
}
