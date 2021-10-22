using InCube.Core.Functional;
using NUnit.Framework;

namespace InCube.Core.Test.Functional
{
    public class OptionalsTest
    {
        [Test]
        public void TestOptional_SetToNullExplicitly()
        {
            var optional = Optionals.None<object>();
            var target = new object();
            optional.SetOptionally(ref target);
            Assert.IsNull(target);
        }

        [Test]
        public void TestOptional_SetToNotNullExplicitly()
        {
            var optional = new object().Optionally();
            object? target = null;
            optional.SetOptionally(ref target);
            Assert.NotNull(target);
        }
    }
}