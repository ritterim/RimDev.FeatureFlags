using System;
using System.ComponentModel;
using System.Reflection;

namespace RimDev.AspNetCore.FeatureFlags
{
    public static class TypeExtensions
    {
        public static string GetDescription(this Type type)
        {
            var firstDescription = type.GetCustomAttribute<DescriptionAttribute>();
            return firstDescription?.Description;
        }
    }
}
