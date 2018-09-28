using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InCube.Core.Collections;
using InCube.Core.Numerics;
using NUnit.Framework;

namespace InCube.Core.Test.Collections
{
    public class EnumerablesTest
    {
        private const int ElementCount = 1 << 8;

        private static readonly IReadOnlyList<int> ShuffledList = 
            Enumerable.Range(0, ElementCount).ToList().Shuffle(new Random(0)).AsReadOnly();

        [Test]
        public void TestMaxBy()
        {
            var maxIdx = ShuffledList.ZipWithIndex().MaxBy(x => x.value).index;
            Assert.AreEqual(ElementCount - 1, ShuffledList[maxIdx]);
        }

        [Test]
        public void TestMinBy()
        {
            var minId = ShuffledList.ZipWithIndex().MinBy(x => x.value).index;
            Assert.AreEqual(0, ShuffledList[minId]);
        }

        [Test]
        public void TestIsSorted()
        {
            var range = Enumerable.Range(0, 100).ToArray();
            Assert.True(range.IsSorted(strict: false));
            Assert.True(range.IsSorted(strict: true));
            var reverse = range.Reverse().ToArray();
            Assert.False(reverse.IsSorted(strict: false));
            Assert.False(reverse.IsSorted(strict: true));
            var constant = Enumerables.Repeat(1).Take(100).ToArray();
            Assert.True(constant.IsSorted(strict: false));
            Assert.True(constant.Reverse().IsSorted(strict: false));
            Assert.False(constant.IsSorted(strict: true));
        }

        [Test]
        public void TestSlice2DArray()
        {
            var a = new [,]
            {
                {1, 2, 3},
                {4, 5, 6},
                {7, 8, 9}
            };

            Assert.That(a, Is.EquivalentTo(a.Slice()));
            const int rowOffset = 1;
            const int colOffset = 2;
            var lower = a.Slice(rowStartInclusive: rowOffset, colStartInclusive: colOffset);
            Assert.AreEqual(a.GetLength(0) - rowOffset, lower.GetLength(0));
            Assert.AreEqual(a.GetLength(1) - colOffset, lower.GetLength(1));
            Assert.That(lower, Is.EquivalentTo(new [,] { {6}, {9}}));

            var upper = a.Slice(rowStopExclusive: a.GetLength(0) - rowOffset, 
                colStopExclusive: a.GetLength(1) - colOffset);
            Assert.AreEqual(a.GetLength(0) - rowOffset, lower.GetLength(0));
            Assert.AreEqual(a.GetLength(1) - colOffset, lower.GetLength(1));
            Assert.That(upper, Is.EquivalentTo(new[,] { { 1 }, { 4 } }));

            var mid = a.Slice(1, 2, 1, 2);
            Assert.That(mid, Is.EquivalentTo(new[,] { { 5 } }));

            Assert.AreEqual(0, a.Slice(rowStopExclusive: 0).GetLength(0));
            Assert.AreEqual(0, a.Slice(colStopExclusive: 0).GetLength(1));
            Assert.Throws<ArgumentException>(() => a.Slice(rowStartInclusive: a.GetLength(0) + 1));
            Assert.Throws<ArgumentException>(() => a.Slice(colStartInclusive: a.GetLength(1) + 1));
            Assert.Throws<ArgumentException>(() => a.Slice(rowStartInclusive: 1, rowStopExclusive: 0));
            Assert.Throws<ArgumentException>(() => a.Slice(colStartInclusive: 1, colStopExclusive: 0));
            Assert.Throws<ArgumentException>(() => a.Slice(rowStartInclusive: - 1));
            Assert.Throws<ArgumentException>(() => a.Slice(colStartInclusive: -1));
        }
    }
}
