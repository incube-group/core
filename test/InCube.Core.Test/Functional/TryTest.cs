using System;
using NUnit.Framework;
using static InCube.Core.Functional.Try;

namespace InCube.Core.Test.Functional
{
    public class TryTest
    {
        [Test]
        public void TestSuccess()
        {
            const int value = 1;
            var t = Execute(() => value);
            Assert.True(t.HasValue);
            Assert.AreEqual(value, t.Value);
            Assert.Throws<InvalidOperationException>(() =>
            {
                var x = t.Exception;
            });
            Assert.AreEqual(value, t.GetValueOrDefault(value - 1));
            Assert.AreEqual(value + 1, t.Match(failure: ex => value - 1, success: v => v + 1));
            Assert.False(t.Failed().HasValue);
        }

        [Test]
        public void TestFailure()
        {
            const int value = 1;
            var t = Execute(() =>
            {
                Preconditions.CheckArgument(value <= 0, "value is positive: {}", value);
                return value;
            });
            Assert.False(t.HasValue);
            Assert.Throws<ArgumentException>(() =>
            {
                var x = t.Value;
            });
            Assert.AreEqual("value is positive: " + value, t.Exception.Message);
            Assert.AreEqual(value - 1, t.GetValueOrDefault(value - 1));
            Assert.AreEqual(value - 1, t.Match(failure: ex => value - 1, success: v => v + 1));
            Assert.True(t.Failed().HasValue);
        }
    }
}
