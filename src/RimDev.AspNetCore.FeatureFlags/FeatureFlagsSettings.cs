using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using Microsoft.FeatureManagement;

namespace RimDev.AspNetCore.FeatureFlags
{
    public class FeatureFlagsSettings
    {
        /// <summary>Column name in the DbCommand result which contains the feature name.
        /// This is used during the <see cref="ISessionManager.GetAsync"/> method to
        /// verify the correct row was obtained from the database.</summary>
        public string NameColumn { get; set; }

        /// <summary>Column name in the DbCommand result which contains the feature value.
        /// This is used during the <see cref="ISessionManager.GetAsync"/> method to
        /// obtain the feature flag value.</summary>
        public string ValueColumn { get; set; }

        /// <summary>How long a cache entry will be valid until it is forced to
        /// refresh from the database.  Defaults to 60 seconds.</summary>
        public TimeSpan CacheTime { get; set; } = TimeSpan.FromSeconds(60);
    }
}
