using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RimDev.AspNetCore.FeatureFlags.Tests.Testing.ApplicationFactory;
using RimDev.AspNetCore.FeatureFlags.UI;
using Xunit;

namespace RimDev.AspNetCore.FeatureFlags.Tests
{
    [Collection(nameof(TestWebApplicationCollection))]
    public class FeatureFlagsUIBuilderTests
    {
        private readonly TestWebApplicationFactory fixture;

        public FeatureFlagsUIBuilderTests(TestWebApplicationFactory fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async Task Get_ReturnsExpectedFeature()
        {
            var client = fixture.CreateClient();
            var uiSettings = fixture.Services.GetRequiredService<FeatureFlagUISettings>();

            var request = new FeatureRequest
            {
                Name = nameof(TestFeature),
                Enabled = true
            };

            await SetValueViaApiAsync(request);

            var response = await client.GetAsync(
                $"{uiSettings.ApiGetPath}?feature={request.Name}");

            response.EnsureSuccessStatusCode();

            var feature = await response.Content.ReadAsJson<FeatureResponse>();

            Assert.True(feature.Enabled);
            Assert.Equal(nameof(TestFeature), feature.Name);
            Assert.Equal("Test feature description.", feature.Description);
        }

        [Fact]
        public async Task GetAll_ReturnsExpectedFeatures()
        {
            var client = fixture.CreateClient();

            var testFeature = new FeatureRequest
            {
                Name = nameof(TestFeature),
                Enabled = true
            };

            var testFeature2 = new FeatureRequest
            {
                Name = nameof(TestFeature2),
                Enabled = true
            };

            await SetValueViaApiAsync(testFeature);
            await SetValueViaApiAsync(testFeature2);

            var uiSettings = fixture.Services.GetRequiredService<FeatureFlagUISettings>();
            var response = await client.GetAsync(uiSettings.ApiGetAllPath);

            response.EnsureSuccessStatusCode();

            var features = (await response.Content.ReadAsJson<IEnumerable<FeatureResponse>>()).ToList();

            Assert.Equal(2, features.Count);

            Assert.All(features, feature => Assert.True(feature.Enabled));
        }

        private async Task SetValueViaApiAsync(FeatureRequest featureRequest)
        {
            var client = fixture.CreateClient();
            var uiSettings = fixture.Services.GetRequiredService<FeatureFlagUISettings>();
            var response = await client.PostAsync(
                uiSettings.ApiSetPath,
                new StringContent(JsonConvert.SerializeObject(featureRequest))
                );
            response.EnsureSuccessStatusCode();
        }

        private async Task<FeatureResponse> GetFeatureFromApiAsync(string featureName)
        {
            var client = fixture.CreateClient();
            var uiSettings = fixture.Services.GetRequiredService<FeatureFlagUISettings>();
            var httpResponse = await client.GetAsync(
                $"{uiSettings.ApiGetPath}?feature={featureName}"
                );
            httpResponse.EnsureSuccessStatusCode();
            return await httpResponse.Content.ReadAsJson<FeatureResponse>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(false)]
        [InlineData(true)]
        public async Task Set_SetsExpectedFeature(bool? expected)
        {
            var request = new FeatureRequest
            {
                Name = nameof(TestFeature2),
                Enabled = expected
            };

            await SetValueViaApiAsync(request);

            var result = await GetFeatureFromApiAsync(nameof(TestFeature2));

            Assert.Equal(expected, result.Enabled);
            Assert.Equal(nameof(TestFeature2), result.Name);
            Assert.Equal("Test feature 2 description.", result.Description);
        }

        [Fact]
        public async Task UIPath_ReturnsExpectedHtml()
        {
            var client = fixture.CreateClient();
            var uiSettings = fixture.Services.GetRequiredService<FeatureFlagUISettings>();
            var response = await client.GetAsync(uiSettings.UIPath);

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            Assert.StartsWith("<!DOCTYPE html>", responseString);
        }
    }
}
