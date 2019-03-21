using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace RimDev.AspNetCore.FeatureFlags.Tests
{
    public class IApplicationBuilderExtensions
    {
        private readonly WebApplicationFactory<TestStartup> factory;

        public IApplicationBuilderExtensions()
        {
            this.factory = new TestWebApplicationFactory();
        }

        [Fact]
        public async Task Get_ReturnsExpectedFeature()
        {
            var client = factory.CreateClient();
            await client.GetAsync("/"); // Invoke Initialize of IFeatureProvider

            var testFeature = new TestFeature
            {
                Value = true
            };

            await TestStartup.Options.Provider.Set(testFeature);

            var response = await client.GetAsync(
                $"{TestStartup.Options.ApiGetPath}?feature={testFeature.GetType().Name}");

            response.EnsureSuccessStatusCode();

            var feature = await response.Content.ReadAsJson<TestFeature>();

            Assert.True(feature.Value);
        }

        [Fact]
        public async Task GetAll_ReturnsExpectedFeatures()
        {
            var client = factory.CreateClient();
            await client.GetAsync("/"); // Invoke Initialize of IFeatureProvider

            var testFeature = new TestFeature
            {
                Value = true
            };

            var testFeature2 = new TestFeature2
            {
                Value = true
            };

            await TestStartup.Options.Provider.Set(testFeature);
            await TestStartup.Options.Provider.Set(testFeature2);

            var response = await client.GetAsync(TestStartup.Options.ApiGetAllPath);

            response.EnsureSuccessStatusCode();

            // Technically not a valid type for TestFeature2:
            var features = await response.Content.ReadAsJson<IEnumerable<TestFeature>>();

            Assert.Equal(2, features.Count());

            Assert.All(features, feature => Assert.True(feature.Value));
        }

        [Fact]
        public async Task Set_SetsExpectedFeature()
        {
            var client = factory.CreateClient();
            await client.GetAsync("/"); // Invoke Initialize of IFeatureProvider

            var provider = TestStartup.Options.Provider;

            var feature = new TestFeature
            {
                Value = true
            };

            var response = await client.PostAsync(
                TestStartup.Options.ApiSetPath,
                new StringContent(JsonConvert.SerializeObject(
                    new FeatureSetRequest
                    {
                        Feature = feature.GetType().Name,
                        Value = true
                    })));

            response.EnsureSuccessStatusCode();

            var providerFeature = await provider.Get(typeof(TestFeature).Name);

            Assert.True(providerFeature.Value);
        }

        [Fact]
        public async Task Ui_ReturnsExpectedHtml()
        {
            var client = factory.CreateClient();

            var response = await client.GetAsync(TestStartup.Options.UiPath);

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            Assert.StartsWith("<!DOCTYPE html>", responseString);
        }
    }
}
