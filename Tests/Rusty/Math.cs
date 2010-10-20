using System;
using IronAHK.Rusty;
using NUnit.Framework;

namespace IronAHK.Tests
{
    partial class Rusty
    {
        // TODO: math tests

        [Test, Category("Math")]
        public void Abs()
        {
            Assert.AreEqual(1, Core.Abs(1));
            Assert.AreEqual(1, Core.Abs(-1));
            Assert.AreEqual(9.81, Core.Abs(-9.81m));
            Assert.AreEqual(0, Core.Abs(-0));
        }

        [Test, Category("Math")]
        public void ACos()
        {
            foreach (var n in new[] { -1, -0.5, 0, 0.5, 1, 0.675 })
                Assert.AreEqual(Math.Acos(n), Core.ACos(n));
        }

        [Test, Category("Math")]
        public void ASin()
        {
            foreach (var n in new[] { -1, -0.5, 0, 0.5, 1, 0.675 })
                Assert.AreEqual(Math.Asin(n), Core.ASin(n));
        }

        [Test, Category("Math")]
        public void ATan()
        {
            foreach (var n in new[] { -1, -0.5, 0, 0.5, 1, 0.675 })
                Assert.AreEqual(Math.Atan(n), Core.ATan(n));
        }
    }
}
