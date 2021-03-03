using System;
using InCube.Core.Collections;
using NUnit.Framework;

namespace InCube.Core.Test.Collections
{
    public class ArraysTest
    {
        [Test]
        public void Test_Set_EmptyArray()
        {
            var empty = Array.Empty<object>();
            empty.Set(new object());
            Assert.IsEmpty(empty);
        }

        [Test]
        public void Test_Set_ReferenceEqualityBetweenMembers()
        {
            var array = new object[2];
            array.Set(new object());
            Assert.AreEqual(array[0], array[1]);
        }

        [Test]
        public void Test_Constant_ZeroLength_Empty()
        {
            var result = 0.Constant(new object());
            Assert.IsEmpty(result);
        }

        [Test]
        public void Test_Constant_ReferenceEqualityBetweenMembers()
        {
            var result = 2.Constant(new object());
            Assert.AreEqual(result[0], result[1]);

        }
    }
}