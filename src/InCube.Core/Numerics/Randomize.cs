using System;
using System.Collections.Generic;
using InCube.Core.Functional;

namespace InCube.Core.Numerics
{
    public static class Randomize
    {
        public static IList<T> Shuffle<T>(this IList<T> list, Option<Random> rngOpt = default)
        {
            var rng = rngOpt.GetValueOrDefault(() => new Random());
            for (var n = list.Count - 1; n > 0; --n)
            {
                var k = rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }

    }
}
