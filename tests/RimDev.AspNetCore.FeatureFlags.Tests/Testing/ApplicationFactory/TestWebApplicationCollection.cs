using Xunit;

namespace RimDev.AspNetCore.FeatureFlags.Tests.Testing.ApplicationFactory
{
    [CollectionDefinition(nameof(TestWebApplicationCollection))]
    public class TestWebApplicationCollection : ICollectionFixture<TestWebApplicationFactory>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
