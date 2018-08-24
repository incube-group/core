using System;
using System.Collections.Generic;
using System.Linq;
using InCube.Core.Collections;
using InCube.Core.Numerics;
using NUnit.Framework;

namespace InCube.Core.Test.Collections
{
    public class CollectionsTest
    {
        [Test]
        public void TestUpperBoundList()
        {
            var bins = new List<int> {1, 3, 3, 5};
            Assert.AreEqual(0, bins.UpperBound(0));
            Assert.AreEqual(1, bins.UpperBound(1));
            Assert.AreEqual(1, bins.UpperBound(2));
            Assert.AreEqual(3, bins.UpperBound(3));
            Assert.AreEqual(3, bins.UpperBound(4));
            Assert.AreEqual(4, bins.UpperBound(5));
            Assert.AreEqual(4, bins.UpperBound(6));
        }

        [Test]
        public void TestUpperBoundArray()
        {
            var bins = new [] { 1, 3, 3, 5 };
            Assert.AreEqual(0, bins.UpperBound(0));
            Assert.AreEqual(1, bins.UpperBound(1));
            Assert.AreEqual(1, bins.UpperBound(2));
            Assert.AreEqual(3, bins.UpperBound(3));
            Assert.AreEqual(3, bins.UpperBound(4));
            Assert.AreEqual(4, bins.UpperBound(5));
            Assert.AreEqual(4, bins.UpperBound(6));
        }

        [Test]
        public void TestLowerBoundList()
        {
            var bins = new List<int> { 1, 3, 3, 5 };
            Assert.AreEqual(0, bins.LowerBound(0));
            Assert.AreEqual(0, bins.LowerBound(1));
            Assert.AreEqual(1, bins.LowerBound(2));
            Assert.AreEqual(1, bins.LowerBound(3));
            Assert.AreEqual(3, bins.LowerBound(4));
            Assert.AreEqual(3, bins.LowerBound(5));
            Assert.AreEqual(4, bins.LowerBound(6));
        }

        [Test]
        public void TestLowerBoundArray()
        {
            var bins = new [] { 1, 3, 3, 5 };
            Assert.AreEqual(0, bins.LowerBound(0));
            Assert.AreEqual(0, bins.LowerBound(1));
            Assert.AreEqual(1, bins.LowerBound(2));
            Assert.AreEqual(1, bins.LowerBound(3));
            Assert.AreEqual(3, bins.LowerBound(4));
            Assert.AreEqual(3, bins.LowerBound(5));
            Assert.AreEqual(4, bins.LowerBound(6));
        }

        [Test]
        public void TestVectorRank()
        {
            var elements = Enumerable.Range(0, 100).ToArray();
            var values = elements.ToArray().Shuffle(new Random(0));
            var ranks = values.VectorRank(elements);
            Assert.AreEqual(elements.Length, ranks.Length);
            foreach (var (element, rank) in elements.ZipAsTuple(ranks))
            {
                Assert.AreEqual(element + 1, rank);
            }

            ranks = values.Concat(values).VectorRank(elements);
            foreach (var (element, rank) in elements.ZipAsTuple(ranks))
            {
                Assert.AreEqual((element + 1) * 2, rank);
            }
        }

        [Test]
        public void TestHistogram()
        {
            const int count = 100;
            var elements = Enumerable.Range(0, count).ToArray().Shuffle(new Random(0));
            const int binSize = 10;
            var binCount = count / binSize;
            var edges = Enumerable.Range(0, binCount).Select(x => x * binSize).ToArray();
            var lowerHist = elements.MakeHistogram(edges, lowerBoundEquals: true);
            foreach (var counts in lowerHist.BinCounts)
            {
                Assert.AreEqual(binSize, counts);
            }

            var upperHist = elements.MakeHistogram(edges, lowerBoundEquals: false);
            foreach (var counts in upperHist.BinCounts.Take(binCount - 1))
            {
                Assert.AreEqual(binSize, counts);
            }

            Assert.AreEqual(binSize - 1, upperHist.BinCounts.Last());

            Assert.AreEqual(1, upperHist.DiscardedCount);

            foreach (var (elem, index) in elements.ZipAsTuple(upperHist.BinIndices))
            {
                Assert.True(elem == 0 && index == -1 || index >= 0);
            }
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
