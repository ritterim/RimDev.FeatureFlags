using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RimDev.AspNetCore.FeatureFlags
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<Type> GetFeatureTypes(
            this IEnumerable<Assembly> featureFlagAssemblies
            )
        {
            return featureFlagAssemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(Feature)));
        }
    }
}
