using NUnit.Framework;

namespace InCube.Core.Test
{
    public class StringsTest
    {
        [Test]
        public void Test_NotNullOrWhiteSpace_NotReplaced()
        {
            const string? original = "a";
            var result = original.OrElseIfNullOrWhiteSpace("b");
            Assert.AreEqual(original, result);
        }

        [Test]
        public void Test_IsNull_IsReplaced()
        {
            string? original = null;
            const string? alternative = "b";
            var result = original.OrElseIfNullOrWhiteSpace(alternative);
            Assert.AreEqual(alternative, result);
        }

        [Test]
        public void Test_IsWhiteSpace_IsReplaced()
        {
            const string? original = "";
            const string? alternative = "b";
            var result = original.OrElseIfNullOrWhiteSpace(alternative);
            Assert.AreEqual(alternative, result);
        }

        [Test]
        public void Test_Idempotence()
        {
            const string? original = "";
            const string? alternative = "b";
            var result = original.OrElseIfNullOrWhiteSpace(alternative).OrElseIfNullOrWhiteSpace("c");
            Assert.AreEqual(alternative, result);
        }
    }
}