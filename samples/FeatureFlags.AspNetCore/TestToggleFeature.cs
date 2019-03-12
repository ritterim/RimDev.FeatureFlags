using RimDev.AspNetCore.FeatureFlags;

namespace FeatureFlags.AspNetCore
{
    public class TestToggleFeature : ToggleFeature
    {
        public override string Description { get; } = "A test toggle feature.";
    }
}
