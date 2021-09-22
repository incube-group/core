using System;
using System.Collections.Generic;
using System.Linq;
using InCube.Core.Collections;
using InCube.Core.Functional;
using InCube.Core.Numerics;
using NUnit.Framework;

namespace InCube.Core.Test.Collections
{
    public class EnumerablesTest
    {
        private const int ElementCount = 1 << 8;

        private static readonly IReadOnlyList<int> ShuffledList =
            Enumerable.Range(0, ElementCount).ToList().Shuffle(new Random(0)).AsReadOnlyList();

        [Test]
        public void TestMaxBy()
        {
            var maxIdx = ShuffledList.ZipWithIndex().MaxBy(x => x.Value).Index;
            Assert.AreEqual(ElementCount - 1, ShuffledList[maxIdx]);
        }

        [Test]
        public void TestMinBy()
        {
            var minId = ShuffledList.ZipWithIndex().MinBy(x => x.Value).Index;
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
            var constant = Enumerable.Repeat(1, 100).ToArray();
            Assert.True(constant.IsSorted(strict: false));
            Assert.True(constant.Reverse().IsSorted(strict: false));
            Assert.False(constant.IsSorted(strict: true));
        }

        [Test]
        public void TestSlice2DArray()
        {
            var a = new[,]
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 }
            };

            Assert.That(a, Is.EquivalentTo(a.Slice()));
            const int rowOffset = 1;
            const int colOffset = 2;
            var lower = a.Slice(rowOffset, colStartInclusive: colOffset);
            Assert.AreEqual(a.GetLength(0) - rowOffset, lower.GetLength(0));
            Assert.AreEqual(a.GetLength(1) - colOffset, lower.GetLength(1));
            Assert.That(lower,
                Is.EquivalentTo(new[,]
                {
                    { 6 },
                    { 9 }
                }));

            var upper = a.Slice(rowStopExclusive: a.GetLength(0) - rowOffset,
                colStopExclusive: a.GetLength(1) - colOffset);
            Assert.AreEqual(a.GetLength(0) - rowOffset, lower.GetLength(0));
            Assert.AreEqual(a.GetLength(1) - colOffset, lower.GetLength(1));
            Assert.That(upper,
                Is.EquivalentTo(new[,]
                {
                    { 1 },
                    { 4 }
                }));

            var mid = a.Slice(1, 2, 1, 2);
            Assert.That(mid, Is.EquivalentTo(new[,] { { 5 } }));

            Assert.AreEqual(0, a.Slice(rowStopExclusive: 0).GetLength(0));
            Assert.AreEqual(0, a.Slice(colStopExclusive: 0).GetLength(1));
            Assert.Throws<ArgumentException>(() => a.Slice(a.GetLength(0) + 1));
            Assert.Throws<ArgumentException>(() => a.Slice(colStartInclusive: a.GetLength(1) + 1));
            Assert.Throws<ArgumentException>(() => a.Slice(1, 0));
            Assert.Throws<ArgumentException>(() => a.Slice(colStartInclusive: 1, colStopExclusive: 0));
            Assert.Throws<ArgumentException>(() => a.Slice(-1));
            Assert.Throws<ArgumentException>(() => a.Slice(colStartInclusive: -1));
        }

        [Test]
        public void Test_ChunkEmptyInput_EmptyResult()
        {
            var result = ArraySegment<object>.Empty.Chunk(10);
            Assert.IsEmpty(result);
        }

        [Test]
        public void Test_Chunk_SingleInput_SingleResultWithSingleObject()
        {
            var result = new object().ToEnumerable().Chunk(10);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.First().Count);
        }

        [Test]
        public void Test_Chunk_Reference()
        {
            var object1 = new object();
            var object2 = new object();
            var object3 = new object();
            var object4 = new object();
            var object5 = new object();
            var input = new[] { object1, object2, object3, object4, object5 };
            var result = input.Chunk(3);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(3, result.First().Count());
            Assert.AreEqual(2, result.Skip(1).First().Count());
        }

        [Test]
        public void Test_FirstMaybe_Empty_None()
        {
            Assert.IsFalse(Array.Empty<object>().FirstMaybe().HasValue);
        }

        [Test]
        public void Test_FirstMaybeWithPredicate_Empty_None()
        {
            Assert.IsFalse(Array.Empty<object>().FirstMaybe(_ => true).HasValue);
        }

        [Test]
        public void Test_FirstMaybeWithFalsePredicate_NotEmpty_None()
        {
            Assert.IsFalse(Maybes.Some<object>(new object()).FirstMaybe(_ => false).HasValue);
        }

        [Test]
        public void Test_FirstMaybeWithPredicate_NotEmpty_Some()
        {
            Assert.IsTrue(Maybes.Some<object>(new object()).FirstMaybe(_ => true).HasValue);
        }

        [Test]
        public void Test_FirstMaybe_NotEmpty_Some()
        {
            Assert.IsTrue(Maybes.Some<object>(new object()).FirstMaybe().HasValue);
        }
    }
}