namespace RimDev.AspNetCore.FeatureFlags.Tests
{
    public class TestFeature : FeatureValue<bool>
    {
        public override string Description { get; } = "Test feature.";
    }
}
