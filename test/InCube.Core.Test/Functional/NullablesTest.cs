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
    }
}
