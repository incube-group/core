using System;
using System.Linq;
using InCube.Core.Collections;
using NUnit.Framework;
using InCube.Core.Functional;
using static InCube.Core.Functional.Option;

namespace InCube.Core.Test.Functional
{
    public class OptionTest
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
            Assert.True(none == null);
            Assert.True(null == none);

            var nullableOne = 1.ToNullable();
            Assert.True(nullableOne.Equals(1));
            Assert.True(nullableOne == 1);
            Assert.True(1 == nullableOne);

            var optionOne = Some(1);
            Assert.True(optionOne.Equals(1));
            Assert.True(optionOne == 1);
            Assert.True(1 == optionOne);
            Assert.True(optionOne.Equals((object)optionOne));


            Assert.True(intNone.Equals(none));

            // ReSharper disable once EqualExpressionComparison
#pragma warning disable CS1718 // Comparison made to same variable
            Assert.True(optionOne == optionOne);
#pragma warning restore CS1718 // Comparison made to same variable
            Assert.True(optionOne == Some(1));
            Assert.True(optionOne != Some(2));

            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.False(Equals(optionOne, Some(1L)));

            Assert.True(none != optionOne);
            Assert.True(optionOne != none);

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

            Option<Option<object>> optOptObj = Empty<object>();
            Assert.True(optOptObj.HasValue);
            Option<Option<int>> optOptInt = Empty<int>();
            Assert.True(optOptInt.HasValue);
            Option<int?> optNullableInt = default(int?);
            Assert.True(optNullableInt.HasValue);
        }

        [Test]
        public void TestWhere()
        {
            var opt = Some(1);
            Assert.True(opt.Where(x => x == 1).HasValue);
            Assert.False(opt.Where(x => x == 2).HasValue);
            Assert.False(Empty<int>().Where(x => x == 2).HasValue);
        }

        [Test]
        public void TestMatch()
        {
            string MatchToString<T>(Option<T> opt)
            {
                return opt.Match(none: () => "None", some: i => $"Some({i})");
            }

            Assert.AreEqual("Some(1)", MatchToString(Some(1)));
            Assert.AreEqual("None", MatchToString(None));
        }

        [Test]
        public void TestCovariance()
        {
            const int count = 10;
            var l1 = Some(Enumerable.Range(0, count).ToList());
            var l2 = Some(Enumerable.Range(0, count).ToArray());
            Assert.AreEqual(l1.Value.Count, l2.Value.Length);
        }

        [Test]
        public void TestFlatten()
        {
            double? one = 1;
            Assert.AreEqual(one, Some(one).Flatten());
            double? none = default;
            // ReSharper disable once ExpressionIsAlwaysNull
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.AreEqual(none, Some(none).Flatten());

        }
    }
}