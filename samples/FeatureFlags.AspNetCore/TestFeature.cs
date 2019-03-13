using RimDev.AspNetCore.FeatureFlags;

namespace FeatureFlags.AspNetCore
{
    public class TestFeature : Feature
    {
        public override string Description { get; } = "A test feature.";
    }
}
