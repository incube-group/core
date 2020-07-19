using System;
using System.Linq;
using NUnit.Framework;
using InCube.Core.Functional;
using static InCube.Core.Functional.Maybe;

#pragma warning disable SA1131 // Use readable conditions
#pragma warning disable SA1312 // Variable names should begin with lower-case letter

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
                var _ = none.Value;
            });
            Assert.Throws<InvalidOperationException>(() =>
            {
                var _ = none[0];
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
                var _ = some[1];
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
            var intNone = Maybe<Boxed<int>>.None;
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
            Maybe<T> Convert<T>(Maybe<T> opt = default) where T : class => opt;

            Assert.True(Convert<Boxed<int>>() == None);
            var one = Boxed.Of(1);
            Assert.True(Convert<Boxed<int>>(one) == Some(one));
            Option<Boxed<int>> someOptInt = Some(one);
            Assert.True(someOptInt.HasValue);
            Option<Boxed<int>> noneOptInt = Maybe<Boxed<int>>.None;
            Assert.False(noneOptInt.HasValue);

            Option<Maybe<object>> optOptObj = Maybe<object>.None;
            Assert.True(optOptObj.HasValue);
            Option<Boxed<int>> optOptInt = Maybe<Boxed<int>>.None;
            Assert.False(optOptInt.HasValue);
        }

        [Test]
        public void TestWhere()
        {
            var opt = Some(Boxed.Of(1));
            Assert.True(opt.Where(x => x == 1).HasValue);
            Assert.False(opt.Where(x => x == 2).HasValue);
            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            Assert.False(Maybe<Boxed<int>>.None.Where(x => x == 2).HasValue);
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
            var none = Maybe<Boxed<int>>.None;
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
            var none = Maybe<Boxed<bool>>.None;
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

        [Test]
        public void Test_SelectMany_SomeMapsToSome()
        {
            var result = Some(new object()).SelectMany(MaybeIdentity);
            Assert.IsTrue(result.HasValue);
        }

        [Test]
        public void Test_SelectMany_NoneMapsToNone() => Assert.IsFalse(None.SelectMany(MaybeIdentity).HasValue);

        [Test]
        public void Test_SelectMany_TrivialMappingGoesToNone() => Assert.IsFalse(Some(new object()).SelectMany(MapToNone).HasValue);

        [Test]
        public void Test_SelectMany_NoneMappedToNoneStillNone() => Assert.IsFalse(None.SelectMany(MapToNone).HasValue);

        [Test]
        public void Test_Select_SomeMappedToSome() => Assert.IsTrue(Some(new object()).Select(Identity).HasValue);

        private static T Identity<T>(T obj) => obj;

        private static Maybe<T> MaybeIdentity<T>(T obj) where T : class => Some(obj);

        private static Maybe<T> MapToNone<T>(T obj) where T : class => None;
    }
}