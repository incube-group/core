using System;
using System.Linq;
using NUnit.Framework;
using InCube.Core.Functional;
using static InCube.Core.Functional.Maybe;

#pragma warning disable SA1131 // Use readable conditions

namespace InCube.Core.Test.Functional
{
    public class MaybeTest
    {
        [Test]
        public void TestNone()
        {
            var none = None;
            Assert.False(none.HasValue);
            Assert.Throws<InvalidOperationException>(() =>
            {
                var x = none.Value;
            });
            Assert.Throws<InvalidOperationException>(() =>
            {
                var x = none[0];
            });
            Assert.AreEqual(null, none.GetValueOrDefault());
        }

        [Test]
        public void TestSome()
        {
            var one = Boxed.Of(1);
            var some = Some(one);
            Assert.True(some.HasValue);
            Assert.AreEqual(some.Value, one);
            Assert.AreEqual(one, some.GetValueOrDefault(0));
            Assert.AreEqual(one, some.GetValueOrDefault());
            Assert.AreEqual(Some(Boxed.Of(2)), some.Select(x => Boxed.Of(x + 1)));
            Assert.AreEqual(one, some[0]);
            Assert.Throws<InvalidOperationException>(() =>
            {
                var x = some[1];
            });

            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => Some(default(Boxed<int>)));
        }

        [Test]
        public void TestEquals()
        {
#pragma warning disable SA1131 // Use readable conditions
            var none = None;
            // ReSharper disable once EqualExpressionComparison
#pragma warning disable CS1718 // Comparison made to same variable
            Assert.True(none == none);
#pragma warning restore CS1718 // Comparison made to same variable
            var intNone = Maybe<Boxed<int>>.Empty;
            Assert.True(none == intNone);
            Assert.True(intNone == none);
            Assert.True(none == null);
            Assert.True(null == none);

            var nullableOne = 1.ToNullable();
            Assert.True(nullableOne.Equals(1));
            Assert.True(nullableOne == 1);
            Assert.True(1 == nullableOne);

            var one = Boxed.Of(1);
            var optionOne = Some(one);
            Assert.True(optionOne.Equals(one));
            Assert.True(optionOne == one);
            Assert.True(one == optionOne);
            Assert.True(optionOne.Equals((object)optionOne));

            Assert.True(intNone.Equals(none));

            // ReSharper disable once EqualExpressionComparison
#pragma warning disable CS1718 // Comparison made to same variable
            Assert.True(optionOne == optionOne);
#pragma warning restore CS1718 // Comparison made to same variable
            Assert.True(optionOne == Some(one));
            Assert.True(optionOne != Some(Boxed.Of(2)));

            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.False(Equals(optionOne, Some(Boxed.Of(1L))));

            Assert.True(none != optionOne);
            Assert.True(optionOne != none);

            var someArray = new[] {1}.ToMaybe();
            Assert.True(someArray != new[] { 1 });
            Assert.True(someArray != none);
            Assert.True(none != someArray);
            Assert.True(someArray != null);
            Assert.True(null != someArray);
        }

        [Test]
        public void TestImplicitConversion()
        {
            Maybe<Boxed<int>> Convert(Maybe<Boxed<int>> opt = default) => opt;

            Assert.True(Convert() == None);
            var one = Boxed.Of(1);
            Assert.True(Convert(one) == Some(one));
            Option<Boxed<int>> someOptInt = Some(one);
            Assert.True(someOptInt.HasValue);

            Option<Maybe<object>> optOptObj = Maybe<object>.Empty;
            Assert.True(optOptObj.HasValue);
            Option<Boxed<int>> optOptInt = Maybe<Boxed<int>>.Empty;
            Assert.False(optOptInt.HasValue);
        }

        [Test]
        public void TestWhere()
        {
            var opt = Some(Boxed.Of(1));
            Assert.True(opt.Where(x => x == 1).HasValue);
            Assert.False(opt.Where(x => x == 2).HasValue);
            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            Assert.False(Maybe<Boxed<int>>.Empty.Where(x => x == 2).HasValue);
        }

        [Test]
        public void TestMatch()
        {
            string MatchToString<T>(Maybe<T> opt) where T : class
            {
                return opt.Match(none: () => "None", some: i => $"Some({i})");
            }

            Assert.AreEqual("Some(1)", MatchToString(Some(Boxed.Of(1))));
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
            var one = Some(Boxed.Of(1));
            Assert.AreEqual(one, Option.Some(one).Flatten());
            var none = Maybe<Boxed<int>>.Empty;
            // ReSharper disable once ExpressionIsAlwaysNull
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.AreEqual(none, Option.Some(none).Flatten());
        }

        [Test]
        public void TestSelect()
        {
            var someOne = Some(Boxed.Of(1));
            Assert.AreEqual(typeof(int), someOne.Select(x => x.Value).GetType());
            Assert.AreEqual(typeof(Maybe<Boxed<int>>), someOne.Select(x => x).GetType());
            Assert.AreEqual(typeof(int), someOne.SelectMany(x => x.Value.ToNullable()).GetType());
            Assert.AreEqual(typeof(Maybe<Boxed<int>>), someOne.SelectMany(x => x.ToMaybe()).GetType());
            var some = Some(Boxed.Of(false));
            Assert.False(some.Select(x => x.Value).GetValueOrDefault(true));
            var none = Maybe<Boxed<bool>>.Empty;
            Assert.True(none.Select(x => x.Value).GetValueOrDefault(true));
            Assert.True(some.Select(x => default(Boxed<bool>)).GetValueOrDefault(Boxed.Of(true)));
        }

        [Test]
        public void TestSize()
        {
            void AssertSameSize<T>() where T : class
            {
                Assert.AreEqual(TypeSize<T>.Size, TypeSize<Maybe<T>>.Size);
            }

            AssertSameSize<Nothing>();
            AssertSameSize<object>();
        }
    }
}