using RimDev.AspNetCore.FeatureFlags;

namespace FeatureFlags.AspNetCore
{
    public class TestStringFeature : StringFeature
    {
        public override string Description { get; } = "A test string feature.";
    }
}
