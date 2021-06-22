using System;
using InCube.Core.Functional;
using NUnit.Framework;

namespace InCube.Core.Test.Functional
{
    public class NullablesTest
    {
        [Test]
        public void TestSelect()
        {
            var some = false.ToNullable();
            Assert.False(some.Select(x => x).GetValueOrDefault(true));
            var none = default(bool?);
            Assert.True(none.Select(x => x).GetValueOrDefault(true));
        }

        [Test]
        public void Test_FirstNullable_Empty_ReturnsNull()
        {
            Assert.IsNull(Array.Empty<int>().FirstNullable());
        }

        [Test]
        public void Test_FirstNullableWithPredicate_Empty_ReturnsNull()
        {
            Assert.IsNull(Array.Empty<int>().FirstNullable(i => true));
        }

        [Test]
        public void Test_FirstNullableWithPredicate_SingleElementNotSatisfyingPredicate_ReturnsNull()
        {
            Assert.IsNull(new[] {1}.FirstNullable(i => i == 2));
        }

        [Test]
        public void Test_FirstNullableWithPredicate_SingleElementSatisfyingPredicate_ReturnsElement()
        {
            Assert.AreEqual(1, new[] {1}.FirstNullable(i => i == 1));
        }

        [Test]
        public void Test_FirstNullable_SingleElement_ReturnsElement()
        {
            Assert.AreEqual(1, new[] {1}.FirstNullable());
        }
    }
}