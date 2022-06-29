using Xunit;

namespace RimDev.AspNetCore.FeatureFlags.Tests
{
    public class FeatureTests
    {
        [Fact]
        public void TestFeature_has_correct_name()
        {
            var feature = new TestFeature();
            Assert.Equal("TestFeature", feature.Name);
        }

        [Fact]
        public void TestFeature_has_correct_description()
        {
            var feature = new TestFeature();
            Assert.Equal("Test feature description.", feature.Description);
        }

        [Fact]
        public void TestFeature2_has_correct_name()
        {
            var feature = new TestFeature2();
            Assert.Equal("TestFeature2", feature.Name);
        }

        [Fact]
        public void TestFeature2_has_correct_description()
        {
            var feature = new TestFeature2();
            Assert.Equal("Test feature 2 description.", feature.Description);
        }
    }
}
