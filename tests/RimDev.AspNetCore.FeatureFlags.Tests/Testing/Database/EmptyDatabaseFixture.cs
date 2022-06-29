using System;

namespace RimDev.AspNetCore.FeatureFlags.Tests.Testing.Database
{
    /// <summary>Creates up a blank, freshly created, randomly named, test database without any migrations.
    /// Creation of database stubs is optional.</summary>
    public class EmptyDatabaseFixture : TestSqlClientDatabaseFixture
    {
        public EmptyDatabaseFixture()
        {
            Console.WriteLine($"Creating {nameof(EmptyDatabaseFixture)} instance...");
            Console.WriteLine($"Created {nameof(EmptyDatabaseFixture)}.");
        }
    }
}
