namespace RimDev.AspNetCore.FeatureFlags
{
    public class FeatureFlagOptions
    {
        public string UiPath { get; set; } = "/_flags";

        public IFeatureProvider Provider { get; set; }
    }
}
