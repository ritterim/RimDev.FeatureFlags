using System.Collections.Generic;

namespace FeatureFlags
{
    public class FeatureStoreData
    {
        public bool Enabled { get; set; }
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
    }
}
