using IronAHK.Rusty;
using NUnit.Framework;

namespace IronAHK.Tests
{
    partial class Rusty
    {
        // TODO: prinvoke tests

        [Test, Category("PInvoke")]
        public void Address()
        {
            const int length = 9000, fill = 46;
            object var = null;
            Core.VarSetCapacity(ref var, length, fill);
            var array = (byte[])var;

            Assert.AreEqual(array.Length, length, "byte[] length");
            Assert.AreEqual(array[0], fill, "byte[] at index 0");
            Assert.AreEqual(array[length - 1], fill, "byte[] at index n-1");
        }
    }
}
