namespace RimDev.AspNetCore.FeatureFlags.Tests.Testing.Configuration
{
    public class RimDevTestsConfiguration
    {
        public RimDevTestsSqlConfiguration Sql { get; set; }
            = new RimDevTestsSqlConfiguration();
    }
}
