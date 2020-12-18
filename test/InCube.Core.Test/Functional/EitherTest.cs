using System;
using InCube.Core.Functional;
using NUnit.Framework;

namespace InCube.Core.Test.Functional
{
    public class EitherTest
    {
        [Test]
        public void TestIntOrDouble()
        {
            Either<int, double> intEither = 1;
            Assert.True(intEither.IsLeft);
            Assert.False(intEither.IsRight);
            Assert.True(intEither.LeftOption.HasValue);
            Assert.False(intEither.RightOption.HasValue);
            Assert.AreEqual(1, intEither.Left);
            Assert.Throws<NotSupportedException>(() =>
            {
                var x = intEither.Right;
            });
            Assert.AreEqual(typeof(int), intEither.Type);

            Either<int, double> dblEither  = 1.0;
            Assert.False(dblEither.IsLeft);
            Assert.True(dblEither.IsRight);
            Assert.False(dblEither.LeftOption.HasValue);
            Assert.True(dblEither.RightOption.HasValue);
            Assert.AreEqual(1, dblEither.Right, 0);
            Assert.Throws<NotSupportedException>(() =>
            {
                var x = dblEither.Left;
            });
            Assert.AreEqual(typeof(double), dblEither.Type);
        }

        [Test]
        public void TestSwitch()
        {
            var either = new Either<int, double>(0.5);
            switch (either)
            {
                case {IsLeft: true} :
                    Assert.Fail();
                    break;
                case {IsRight: true} :
                    Assert.Pass();
                    break;
                default:
                    Assert.Fail();
                    break;
            }
        }
    }
}