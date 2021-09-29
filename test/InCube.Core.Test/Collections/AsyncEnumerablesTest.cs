using System.Collections.Generic;
using System.Threading.Tasks;
using InCube.Core.Collections;
using NUnit.Framework;

namespace InCube.Core.Test.Collections
{
    public class AsyncEnumerablesTest
    {
        [Test]
        public async Task Test_Empty()
        {
            var looped = false;
            await foreach (var l in EnumerateObjects(0).Chunk(100))
            {
                Assert.IsFalse(looped);
                Assert.IsEmpty(l);
                looped = true;
            }
        }

        [Test]
        public async Task Test_Single()
        {
            var looped = false;
            await foreach (var l in EnumerateObjects(1).Chunk(100))
            {
                Assert.IsFalse(looped);
                Assert.AreEqual(1, l.Count);
                looped = true;
            }
        }

        [Test]
        public async Task Test_TwoChunks()
        {
            var loopCount = 0;
            await foreach (var l in EnumerateObjects(200).Chunk(100))
            {
                Assert.AreEqual(100, l.Count);
                loopCount++;
            }
            Assert.AreEqual(2, loopCount);
        }

        private static async IAsyncEnumerable<object> EnumerateObjects(int howMany)
        {
            for (var i = 0; i < howMany; i++)
            {
                yield return new object();
            }
        }
    }
}