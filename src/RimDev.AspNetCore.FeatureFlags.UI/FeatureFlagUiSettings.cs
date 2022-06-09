namespace RimDev.AspNetCore.FeatureFlags.UI
{
    public class FeatureFlagUiSettings
    {
        public string ApiGetAllPath => UiPath + "/get_all";

        public string ApiGetPath => UiPath + "/get";

        public string ApiSetPath => UiPath + "/set";

        public string UiPath => "/_features";
    }
}
