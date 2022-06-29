using System.Data.SqlClient;
using System.Threading.Tasks;
using Xunit;

namespace RimDev.AspNetCore.FeatureFlags.Tests.Testing.Database.Tests
{
    [Collection(nameof(EmptyDatabaseCollection))]
    public class EmptyDatabaseFixtureTests
    {
        private readonly EmptyDatabaseFixture _fixture;

        public EmptyDatabaseFixtureTests(EmptyDatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void MasterConnectionStringDebug_not_empty()
        {
            Assert.NotEmpty(_fixture.MasterConnectionStringDebug);
        }

        [Fact]
        public void ConnectionStringDebug_not_empty()
        {
            Assert.NotEmpty(_fixture.ConnectionStringDebug);
        }

        [Fact]
        public async Task Can_access_database_at_ConnectionString()
        {
            await using var conn = new SqlConnection(_fixture.ConnectionString);
            await conn.OpenAsync();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "select count(*) from INFORMATION_SCHEMA.TABLES;";
            var queryResult = (int) await cmd.ExecuteScalarAsync();
            Assert.True(queryResult >= 0);
        }
    }
}
