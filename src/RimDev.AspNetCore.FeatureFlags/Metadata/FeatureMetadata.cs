using System;
using System.Collections.Generic;

namespace FeatureFlags.Metadata
{
    public class FeatureMetadata
    {
        public Type Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public IReadOnlyList<StoreMetadata> Stores { get; set; }
            = new List<StoreMetadata>();

        public IReadOnlyList<ConditionMetadata> Conditions
            = new List<ConditionMetadata>();
    }
}
