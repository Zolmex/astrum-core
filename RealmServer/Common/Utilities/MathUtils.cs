using System;
using System.Security.Cryptography;

namespace Common.Utilities
{
    public static class MathUtils
    {
        private static readonly RandomNumberGenerator RNG = RandomNumberGenerator.Create();
        public static readonly Random Random = new Random();

        public static string GenerateSalt()
        {
            var x = new byte[0x10];
            RNG.GetNonZeroBytes(x);
            return Convert.ToBase64String(x);
        }

        public static double NextDoubleRange(int min, int max)
            => Random.Next(min, max) * Random.NextDouble();
    }
}
