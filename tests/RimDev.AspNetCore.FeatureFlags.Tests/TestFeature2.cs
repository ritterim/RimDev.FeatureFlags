namespace RimDev.AspNetCore.FeatureFlags.Tests
{
    public class TestFeature2 : FeatureValue<bool>
    {
        public override string Description { get; } = "Test feature 2.";
    }
}
