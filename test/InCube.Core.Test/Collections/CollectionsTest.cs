using System.Collections.Generic;
using System.Linq;
using InCube.Core.Collections;
using InCube.Core.Functional;
using NUnit.Framework;

namespace InCube.Core.Test.Collections
{
    public class CollectionsTest
    {
        [Test]
        public void TestDigitizeList()
        {
            var bins = new List<int> {1, 3, 3, 5};
            Assert.AreEqual(0, bins.Digitize(0));
            Assert.AreEqual(1, bins.Digitize(1));
            Assert.AreEqual(1, bins.Digitize(2));
            Assert.AreEqual(3, bins.Digitize(3));
            Assert.AreEqual(3, bins.Digitize(4));
            Assert.AreEqual(4, bins.Digitize(5));
            Assert.AreEqual(4, bins.Digitize(6));
        }

        [Test]
        public void TestDigitizeArray()
        {
            var bins = new int[] { 1, 3, 3, 5 };
            Assert.AreEqual(0, bins.Digitize(0));
            Assert.AreEqual(1, bins.Digitize(1));
            Assert.AreEqual(1, bins.Digitize(2));
            Assert.AreEqual(3, bins.Digitize(3));
            Assert.AreEqual(3, bins.Digitize(4));
            Assert.AreEqual(4, bins.Digitize(5));
            Assert.AreEqual(4, bins.Digitize(6));
        }

        [Test]
        public void RemoveSeq()
        {
            const int count = 100;
            var seq = Enumerable.Range(0, count).ToArray();
            Assert.AreEqual(count, seq.RemoveSeq(x => false));
            var nextFree = seq.RemoveSeq(x => x % 2 == 0);
            Assert.AreEqual(count / 2, nextFree);
            for (var i = 0; i < nextFree; ++i)
            {
                Assert.True(seq[i] % 2 == 1);
            }
            Assert.AreEqual(0, seq.RemoveSeq(x => true, stopIdx: nextFree));
        }

        [Test]
        public void RemovePar()
        {
            const int count = 1024;
            var seq = Enumerable.Range(0, count).ToArray();
            Assert.AreEqual(count, seq.RemovePar(x => false));
            var nextFree = seq.RemovePar(x => x % 2 == 0);
            Assert.AreEqual(count / 2, nextFree);
            for (var i = 0; i < nextFree; ++i)
            {
                Assert.True(seq[i] % 2 == 1);
            }
            Assert.AreEqual(0, seq.RemovePar(x => true, stopIdx: nextFree));
        }

        [Test]
        public void TestSlice()
        {
            var expected = new []{2, 3, 4};
            var start = expected.First();
            var stop = start + expected.Length;
            var elems = Enumerables.Iterate(0, x => x + 1);
            Assert.AreEqual(expected, elems.Slice(start, stop).ToArray());
            var array = Enumerable.Range(0, 10).ToArray();
            Assert.AreEqual(expected, array.Slice(start, stop));
        }
    }
}
