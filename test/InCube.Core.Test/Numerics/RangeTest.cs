using InCube.Core.Numerics;
using NUnit.Framework;

namespace InCube.Core.Test.Numerics
{
    public class RangeTest
    {
        [Test]
        public void TestContains()
        {
            const int min = 5;
            const int max = 10;
            var range = min.ToRange(max);
            Assert.True(range.Contains(min));
            Assert.True(range.Contains(max));
            Assert.True(range.Contains(range));
            Assert.False(range.Contains(min - 1));
            Assert.False(range.Contains(max + 1));
            Assert.False(range.Contains(range.With(min: min - 1)));
            Assert.False(range.Contains(range.With(max: max + 1)));
        }

        [Test]
        public void TestOverlapsWith()
        {
            const int min = 5;
            const int max = 10;
            var range = min.ToRange(max);
            var left = int.MinValue.ToRange(min);
            var right = max.ToRange(int.MaxValue);
            Assert.True(left.OverlapsWith(range));
            Assert.True(range.OverlapsWith(left));
            Assert.True(range.OverlapsWith(right));
            Assert.True(right.OverlapsWith(range));
            Assert.False(left.OverlapsWith(right));
            Assert.False(right.OverlapsWith(left));
            Assert.True(range.OverlapsWith(range));
        }

        [Test]
        public void TestIntersectWith()
        {
            const int min = 5;
            const int max = 10;
            var range = min.ToRange(max);
            var left = int.MinValue.ToRange(min);
            var right = max.ToRange(int.MaxValue);
            Assert.True(left.IntersectWith(range).Contains(min));
            Assert.False(left.IntersectWith(range).Contains(max));
            Assert.True(range.IntersectWith(left).Contains(min));
            Assert.False(range.IntersectWith(left).Contains(max));
            Assert.True(right.IntersectWith(range).Contains(max));
            Assert.False(right.IntersectWith(range).Contains(min));
            Assert.True(range.IntersectWith(right).Contains(max));
            Assert.False(range.IntersectWith(right).Contains(min));
            Assert.AreEqual(range, range.IntersectWith(range));
        }
    }
}
