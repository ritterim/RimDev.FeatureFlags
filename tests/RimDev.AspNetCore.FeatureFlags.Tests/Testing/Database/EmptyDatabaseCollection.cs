using Xunit;

namespace RimDev.AspNetCore.FeatureFlags.Tests.Testing.Database
{
    /// <summary>This xUnit collection (and the fixture <see cref="EmptyDatabaseFixture"/>
    /// should only be used for tests where you want a fresh blank test database, prior to
    /// any migrations being executed. </summary>
    [CollectionDefinition(nameof(EmptyDatabaseCollection))]
    public class EmptyDatabaseCollection : ICollectionFixture<EmptyDatabaseFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
