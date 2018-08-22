using System;
using System.Collections.Generic;
using System.Linq;
using InCube.Core.Collections;
using NUnit.Framework;
using InCube.Core.Functional;
using static InCube.Core.Functional.Options;

namespace InCube.Core.Test.Functional
{
    public class OptionsTest
    {
        [Test]
        public void TestNone()
        {
            var none = None;
            Assert.False(none.HasValue);
            Assert.Throws<InvalidOperationException>(() =>
            {
                var _ = none.Value;
            });
            Assert.AreEqual(null, none.GetValueOrDefault());
            //Assert.AreEqual(none.As<int>() is IOption<T>);
        }

        [Test]
        public void TestSome()
        {
            var some = Some(1);
            Assert.True(some.HasValue);
            Assert.AreEqual(some.Value, 1);
            Assert.AreEqual(1, some.GetValueOrDefault(0));
            Assert.AreEqual(1, some.GetValueOrDefault());
            Assert.AreEqual(Some(2), some.AsEnumerable().Select(x => x + 1).FirstOption());
        }

        [Test]
        public void TestEquals()
        {
            var none = None;
            // ReSharper disable once EqualExpressionComparison
#pragma warning disable CS1718 // Comparison made to same variable
            Assert.True(none == none);
#pragma warning restore CS1718 // Comparison made to same variable
            var intNone = Empty<int>();
            Assert.True(none == intNone);
            Assert.True(intNone == none);
            IOption<Nothing> boxedNone = none;
            Assert.True(none == boxedNone.ToOption());
            Assert.True(boxedNone.ToOption() == none);
            var nullNone = (IOption<Nothing>) null;
            Assert.True(none == nullNone);
            Assert.True(nullNone == none);
            Assert.True(null == nullNone);
            Assert.True(nullNone == null);
            Assert.True(none == null);
            Assert.True(null == none);
            //Assert.True(none.As<long>() == None.As<int>());

            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.True(Equals(none, intNone));
            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.True(Equals(intNone, none));

            var some = Some(1);
            // ReSharper disable once EqualExpressionComparison
#pragma warning disable CS1718 // Comparison made to same variable
            Assert.True(some == some);
#pragma warning restore CS1718 // Comparison made to same variable
            Assert.True(some == Some(1));
            Assert.True(some != Some(2));
            var nullInt = (IOption<int>) null;
            Assert.True(some != nullInt);
            Assert.True(nullInt != some);

            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.False(Equals(some, Some(1L)));

            Assert.True(none != some);
            Assert.True(some != none);

            var someArray = new[] {1}.ToOption();
            Assert.True(someArray != new[] { 1 });
            Assert.True(someArray != none);
            Assert.True(none != someArray);
            Assert.True(someArray != null);
            Assert.True(null != someArray);
        }

        [Test]
        public void TestImplicitConversion()
        {
            Option<int> Convert(Option<int> opt = default) => opt;

            Assert.True(Convert() == None);
            Assert.True(Convert(1) == Some(1));
        }

        [Test]
        public void TestMatch()
        {
            string MatchToString<T>(Option<T> opt) => opt.Match(none: () => "None", some: i => $"Some({i})");

            Assert.AreEqual("Some(1)", MatchToString(Some(1)));
            Assert.AreEqual("None", MatchToString(None));
        }

        [Test]
        public void TestCovariance()
        {
            const int count = 10;
            IOption<ICollection<int>> l1 = Some(Enumerable.Range(0, count).ToList());
            IOption<ICollection<int>> l2 = Some(Enumerable.Range(0, count).ToArray());
            Assert.AreEqual(l1.Value.Count, l2.Value.Count);
        }
    }
}