using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InCube.Core.Collections;
using NUnit.Framework;

namespace InCube.Core.Test.Collections
{
    public class EnumerablesTest
    {
        private static IList<T> Shuffle<T>(IList<T> list, Random rng)
        {
            for (var n = list.Count - 1; n > 0; --n)
            {
                var k = rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }

        private const int ElementCount = 1 << 8;

        private static readonly IReadOnlyList<int> ShuffledList = 
            Shuffle(Enumerable.Range(0, ElementCount).ToList(), new Random(0)).AsReadOnly();

        [Test]
        public void TestMaxBy()
        {
            var maxIdx = ShuffledList.ZipWithIndex().MaxBy(x => x.value).index;
            Assert.AreEqual(ElementCount - 1, ShuffledList[maxIdx]);
        }

        [Test]
        public void TestMinBy()
        {
            var maxIdx = ShuffledList.ZipWithIndex().MinBy(x => x.value).index;
            Assert.AreEqual(0, ShuffledList[maxIdx]);
        }
    }
}
