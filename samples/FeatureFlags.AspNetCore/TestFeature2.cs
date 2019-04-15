using RimDev.AspNetCore.FeatureFlags;

namespace FeatureFlags.AspNetCore
{
    public class TestFeature2 : FeatureValue<bool>
    {
        public override string Description { get; } = null;
    }
}
