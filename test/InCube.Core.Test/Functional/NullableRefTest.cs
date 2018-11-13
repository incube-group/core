using System;
using System.Collections.Generic;
using System.Linq;
using InCube.Core.Collections;
using NUnit.Framework;
using InCube.Core.Functional;
using Newtonsoft.Json;
using static InCube.Core.Functional.NullableRef;

namespace InCube.Core.Test.Functional
{
    internal class Boxed<T> : IEquatable<Boxed<T>> where T : struct
    {
        [JsonConstructor]
        internal Boxed(T value)
        {
            this.Value = value;
        }

        public T Value { get; }

        public static implicit operator T(Boxed<T> boxed) => boxed.Value;

        public static implicit operator Boxed<T>(T value) => new Boxed<T>(value);

        public bool Equals(Boxed<T> that)
        {
            if (ReferenceEquals(null, that))
            {
                return false;
            }

            if (ReferenceEquals(this, that))
            {
                return true;
            }

            return EqualityComparer<T>.Default.Equals(this.Value, that.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((Boxed<T>) obj);
        }

        public override int GetHashCode() => this.Value.GetHashCode();

        public override string ToString() => $"{this.Value}";
    }

    internal static class Boxed
    {
        public static Boxed<T> Of<T>(T value) where T : struct => value;
    } 

    public class NullableRefTest
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
            var one = Boxed.Of(1);
            var some = Some(one);
            Assert.True(some.HasValue);
            Assert.AreEqual(some.Value, one);
            Assert.AreEqual(one, some.GetValueOrDefault(0));
            Assert.AreEqual(one, some.GetValueOrDefault());
            Assert.AreEqual(Some(Boxed.Of(2)), some.Select(x => Boxed.Of(x + 1)));
        }

        [Test]
        public void TestEquals()
        {
            var none = None;
            // ReSharper disable once EqualExpressionComparison
#pragma warning disable CS1718 // Comparison made to same variable
            Assert.True(none == none);
#pragma warning restore CS1718 // Comparison made to same variable
            var intNone = Empty<Boxed<int>>();
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

            var someArray = new[] {1}.ToNullable();
            Assert.True(someArray != new[] { 1 });
            Assert.True(someArray != none);
            Assert.True(none != someArray);
            Assert.True(someArray != null);
            Assert.True(null != someArray);
        }

        [Test]
        public void TestImplicitConversion()
        {
            //Option<int> Convert(Option<int> opt = default) => opt;

            //Assert.True(Convert() == None);
            //Assert.True(Convert(1) == Some(1));

            //Option<Option<object>> optOptObj = Empty<object>();
            //Assert.True(optOptObj.HasValue);
            //Option<Option<int>> optOptInt = Empty<int>();
            //Assert.True(optOptObj.HasValue);
            //Option<int?> optNullableInt = default(int?);
            //Assert.True(optNullableInt.HasValue);
        }

        [Test]
        public void TestWhere()
        {
            var opt = Some(Boxed.Of(1));
            Assert.True(opt.Where(x => x == 1).HasValue);
            Assert.False(opt.Where(x => x == 2).HasValue);
            Assert.False(Empty<Boxed<int>>().Where(x => x == 2).HasValue);
        }

        [Test]
        public void TestMatch()
        {
            string MatchToString<T>(NullableRef<T> opt) where T : class
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
            var one = NullableRef.Some(Boxed.Of(1));
            Assert.AreEqual(one, Option.Some(one).Flatten());
            var none = NullableRef.Empty<Boxed<int>>();
            // ReSharper disable once ExpressionIsAlwaysNull
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.AreEqual(none, Option.Some(none).Flatten());
        }

        [Test]
        public void TestSelect()
        {
            var someOne = Some(Boxed.Of(1));
            Assert.AreEqual(typeof(int), someOne.Select(x => x.Value).GetType());
            Assert.AreEqual(typeof(NullableRef<Boxed<int>>), someOne.Select(x => x).GetType());
            Assert.AreEqual(typeof(int), someOne.SelectMany(x => x.Value.ToNullable()).GetType());
            Assert.AreEqual(typeof(NullableRef<Boxed<int>>), someOne.SelectMany(x => x.ToNullable()).GetType());
        }
    }
}