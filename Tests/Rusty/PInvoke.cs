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
            byte[] var;
            Core.VarSetCapacity(out var, length, fill);

            Assert.AreEqual(var.Length, length, "byte[] length");
            Assert.AreEqual(var[0], fill, "byte[] at index 0");
            Assert.AreEqual(var[length - 1], fill, "byte[] at index n-1");
        }
    }
}
