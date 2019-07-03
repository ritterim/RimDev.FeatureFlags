using System;

namespace FeatureFlags
{
    public static class FeatureExtensions
    {
        /// <summary>
        /// Uses Convert.ChangeType to easily
        /// convert a parameters string value into a
        /// strongly typed .NET type.
        /// </summary>
        /// <remarks>May not work on all types, especially complex types</remarks>
        /// <param name="parameter"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T As<T>(this Parameter parameter)
        {
            if (parameter == null)
                return default(T);

            return (T)Convert.ChangeType(parameter.Value, typeof(T));
        }
    }
}
