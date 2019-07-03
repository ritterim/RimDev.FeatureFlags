using System;
using System.Collections.Generic;
using System.Linq;
using FeatureFlags.Metadata;

namespace FeatureFlags.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ConditionAttribute : Attribute
    {
        private readonly Type type;
        public string Name { get; set; }
        public string Description { get; set; }
        /// <summary>
        ///  Required Parameters, will throw if these conditions are not met
        /// </summary>
        public string[] Required { get; set; }
        /// <summary>
        /// Optional Parameters, will attempt to use default value if conditions are not met
        /// </summary>
        public string[] Optional { get; set; }

        public ConditionAttribute(Type type)
        {
            this.type = type;
        }

        public ConditionMetadata GetMetadata()
        {
            var required = Required ?? Array.Empty<string>();
            var optional = Optional ?? Array.Empty<string>();

            return new ConditionMetadata
            {
                Type = type,
                Name = Name ?? type.Name,
                Description = Description,
                Parameters =
                    required
                    .Select(
                        name => new ParameterMetadata
                        {
                            Name = name,
                            Required = true
                        }
                    )
                    .Union(optional
                        .Select
                        (
                            name => new ParameterMetadata
                            {
                                Name = name
                            }
                        )
                    )
                    .ToList(),
            };
        }

        public static IReadOnlyList<ConditionMetadata> GetMetadata(Type type)
        {
            var conditions = type.GetCustomAttributes(typeof(ConditionAttribute), true)
                .Cast<ConditionAttribute>()
                .Where(condition => condition != null)
                .Select(condition => condition.GetMetadata())
                .ToList()
                .AsReadOnly();

            return conditions;
        }
    }
}
