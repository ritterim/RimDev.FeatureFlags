using System;

namespace RimDev.AspNetCore.FeatureFlags.Tests.Testing
{
    public static class Rng
    {
        private static readonly Random Random = new Random();

        public static int GetInt(int minValue, int maxValue) => Random.Next(minValue, maxValue);

        public static bool GetBool() => Random.Next(0, 2) == 1;

        public static bool? GetNullableBool()
        {
            var value = Random.Next(-1, 2);
            return value switch
            {
                0 => false,
                1 => true,
                _ => null
            };
        }
    }
}
