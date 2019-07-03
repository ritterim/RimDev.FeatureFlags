using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FeatureFlags;
using FeatureFlags.Attributes;
using Xunit;

namespace RimDev.AspNetCore.FeatureFlags.Tests
{
    public class FeatureAttributeTests
    {
        [Fact]
        public void FeatureAttribute_throws_if_class_not_a_feature()
        {
            var type = typeof(Not);
            Assert.Throws<ArgumentException>(() => FeatureAttribute.GetMetadata(type));
        }
        public class Not
        {

        }

        [Fact]
        public void FeatureAttribute_can_get_metadata_without_attribute()
        {
            var type = typeof(WithoutAttribute);
            var metadata = FeatureAttribute.GetMetadata(type);

            Assert.NotNull(metadata);
            Assert.Equal(type.Name, metadata.Name);
        }

        public class WithoutAttribute : Feature
        {

        }

        [Fact]
        public void FeatureAttribute_can_get_metadata_with_attribute()
        {
            var type = typeof(WithAttribute);
            var metadata = FeatureAttribute.GetMetadata(type);

            Assert.NotNull(metadata);
            Assert.Equal("Test Feature", metadata.Name);
            Assert.Equal("a test feature", metadata.Description);
        }

        [Feature(Name = "Test Feature", Description = "a test feature")]
        public class WithAttribute : Feature
        {

        }

        [Fact]
        public void FeatureAttribute_can_get_metadata_with_store_attribute()
        {
            var type = typeof(WithStore);
            var metadata = FeatureAttribute.GetMetadata(type);

            Assert.NotEmpty(metadata.Stores);
            var key = metadata.Stores.Select(x => x.Key).First();

            Assert.Equal("test", key);
        }

        [Feature(Stores = new [] { "test" })]
        public class WithStore : Feature
        {
        }

        [Fact]
        public void FeatureAttribute_can_get_metadata_with_condition_attribute()
        {
            var type = typeof(WithCondition);
            var metadata = FeatureAttribute.GetMetadata(type);

            Assert.NotEmpty(metadata.Conditions);
        }

        [Condition(typeof(TestCondition))]
        public class WithCondition : Feature
        {
            public class TestCondition : ICondition
            {
                public Task Apply(Feature feature)
                {
                    throw new NotImplementedException();
                }
            }
        }

        [Fact]
        public void FeatureAttribute_can_get_metadata_with_multiple_condition_attributes()
        {
            var type = typeof(WithMultipleConditions);
            var metadata = FeatureAttribute.GetMetadata(type);

            Assert.Equal(2, metadata.Conditions.Count);
        }

        [Condition(typeof(OneCondition))]
        [Condition(typeof(TwoCondition))]
        public class WithMultipleConditions : Feature
        {
            public class OneCondition : ICondition
            {
                public Task Apply(Feature feature)
                {
                    throw new NotImplementedException();
                }
            }

            public class TwoCondition : ICondition
            {
                public Task Apply(Feature feature)
                {
                    throw new NotImplementedException();
                }
            }
        }

        [Fact]
        public void FeatureAttribute_can_get_metadata_with_condition_and_parameters_attribute()
        {
            var type = typeof(WithConditionAndParameters);
            var metadata = FeatureAttribute.GetMetadata(type);

            Assert.Single(metadata.Conditions);

            var condition = metadata.Conditions.First();

            Assert.Single(condition.Parameters.Where(p => p.Required));
            Assert.Single(condition.Parameters.Where(p => !p.Required));
        }

        [Condition(
            typeof(TestCondition),
            Optional = new [] { "optional" },
            Required = new [] { "required" }
        )]
        public class WithConditionAndParameters : Feature
        {
            public class TestCondition : ICondition
            {
                public Task Apply(Feature feature)
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}
