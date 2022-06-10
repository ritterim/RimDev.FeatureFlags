using System.Threading.Tasks;
using Lussatite.FeatureManagement.SessionManagers.SqlClient;
using RimDev.AspNetCore.FeatureFlags.Tests.Testing;
using RimDev.AspNetCore.FeatureFlags.Tests.Testing.Database;
using Xunit;

namespace RimDev.AspNetCore.FeatureFlags.Tests
{
    [Collection(nameof(EmptyDatabaseCollection))]
    public class FeatureFlagsSessionManagerTests
    {
        private readonly EmptyDatabaseFixture databaseFixture;

        public FeatureFlagsSessionManagerTests(
            EmptyDatabaseFixture databaseFixture
            )
        {
            this.databaseFixture = databaseFixture;
        }

        private async Task<FeatureFlagsSessionManager> CreateSut()
        {
            var sqlSessionManagerSettings = new SQLServerSessionManagerSettings
            {
                ConnectionString = databaseFixture.ConnectionString
            };

            var featureFlagsSettings = new FeatureFlagsSettings(
                featureFlagAssemblies: new[] { typeof(FeatureFlagsSessionManagerTests).Assembly }
                )
            {
                ConnectionString = databaseFixture.ConnectionString,
                InitializationConnectionString = databaseFixture.ConnectionString,
                SqlSessionManagerSettings = sqlSessionManagerSettings,
            };

            var sut = new FeatureFlagsSessionManager(
                featureFlagsSettings: featureFlagsSettings
                );

            await sqlSessionManagerSettings.CreateDatabaseTableAsync(
                featureFlagsSettings.InitializationConnectionString
                );

            return sut;
        }

        [Fact]
        public async Task Can_create_sut()
        {
            var sut = await CreateSut();
            Assert.NotNull(sut);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task SetAsync_returns_expected(bool expected)
        {
            var sut = await CreateSut();
            const string baseFeatureName = "RDANCFF_SetAsync_";
            var featureName = $"{baseFeatureName}{expected}";
            await sut.SetAsync(featureName, expected);
            var result = await sut.GetAsync(featureName);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(false)]
        [InlineData(true)]
        public async Task SetNullableAsync_returns_expected(bool? expected)
        {
            var sut = await CreateSut();
            const string baseFeatureName = "RDANCFF_SetNullableAsync_";
            var expectedName = expected.HasValue ? expected.ToString() : "Null";
            var featureName = $"{baseFeatureName}{expectedName}";
            await sut.SetNullableAsync(featureName, expected);
            var result = await sut.GetAsync(featureName);
            Assert.Equal(expected, result);
        }

        /// <summary>Exercise the SetAsync method a bunch of times to ensure there are no latent
        /// bugs with race conditions, values not being set properly, etc.</summary>
        [Fact]
        public async Task Exercise_SetAsync()
        {
            var sut = await CreateSut();
            const int iterations = 8000;
            const string baseFeatureName = "RDANCFF_SetAsync_Exercise_";
            for (var i = 0; i < iterations; i++)
            {
                var doUpdate = Rng.GetInt(0, 25) == 0; // only call SetAsync() some of the time
                var featureName = $"{baseFeatureName}{Rng.GetInt(0, 10)}";
                var enabled = Rng.GetBool();
                if (doUpdate) await sut.SetAsync(featureName, enabled);
                var result = await sut.GetAsync(featureName);
                if (doUpdate) Assert.Equal(enabled, result);
            }
        }

        /// <summary>Exercise the SetNullableAsync method a bunch of times to ensure there are no latent
        /// bugs with race conditions, values not being set properly, etc.</summary>
        [Fact]
        public async Task Exercise_SetNullableAsync()
        {
            var sut = await CreateSut();
            const int iterations = 8000;
            const string baseFeatureName = "RDANCFF_SetNullableAsync_Exercise_";
            for (var i = 0; i < iterations; i++)
            {
                var doUpdate = Rng.GetInt(0, 25) == 0; // only call SetNullableAsync() some of the time
                var featureName = $"{baseFeatureName}{Rng.GetInt(0, 10)}";
                var enabled = Rng.GetNullableBool();
                if (doUpdate) await sut.SetNullableAsync(featureName, enabled);
                var result = await sut.GetAsync(featureName);
                if (doUpdate) Assert.Equal(enabled, result);
            }
        }
    }
}
