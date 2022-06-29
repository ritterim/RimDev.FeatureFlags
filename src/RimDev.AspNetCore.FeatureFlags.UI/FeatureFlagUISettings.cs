namespace RimDev.AspNetCore.FeatureFlags.UI
{
    public class FeatureFlagUISettings
    {
        public string ApiGetAllPath => UIPath + "/get_all";

        public string ApiGetPath => UIPath + "/get";

        public string ApiSetPath => UIPath + "/set";

        public string UIPath => "/_features";
    }
}
