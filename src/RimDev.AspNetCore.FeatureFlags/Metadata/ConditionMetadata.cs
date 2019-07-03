using System;
using System.Collections.Generic;

namespace FeatureFlags.Metadata
{
    public class ConditionMetadata
    {
        public Type Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public IReadOnlyList<ParameterMetadata> Parameters { get; set; }
            = new List<ParameterMetadata>();
    }
}
