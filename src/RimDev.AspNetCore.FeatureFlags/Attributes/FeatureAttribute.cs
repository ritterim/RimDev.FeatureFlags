using System;
using System.Linq;
using FeatureFlags.Metadata;

namespace FeatureFlags.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FeatureAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string[] Stores { get; set; }

        public static FeatureMetadata GetMetadata(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (!typeof(Feature).IsAssignableFrom(type))
            {
                throw new ArgumentException($"{type.FullName} is not a feature", nameof(type));
            }

            var feature = type
                .GetCustomAttributes(typeof(FeatureAttribute), true)
                .Cast<FeatureAttribute>()
                .FirstOrDefault();

            var metadata = new FeatureMetadata
            {
                Type = type,
                Name = feature?.Name ?? type.Name,
                Description = feature?.Description ?? string.Empty,
                Conditions = ConditionAttribute.GetMetadata(type),
                Stores = (feature?.Stores ?? Array.Empty<string>())
                    .Select(key => new StoreMetadata { Key = key })
                    .ToList()
                    .AsReadOnly()
            };

            return metadata;
        }

        public static FeatureMetadata GetMetadata<T>()
        {
            return GetMetadata(typeof(T));
        }
    }
}
