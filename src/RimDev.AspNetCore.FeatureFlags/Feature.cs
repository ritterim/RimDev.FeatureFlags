using System.Collections.Generic;

namespace FeatureFlags
{
    public abstract class Feature
    {
        public bool Enabled { get; set; }
        public List<Parameter> Parameters { get; set; }
            = new List<Parameter>();
    }
}
