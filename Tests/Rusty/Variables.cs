using IronAHK.Rusty;
using NUnit.Framework;

namespace IronAHK.Tests
{
    partial class Rusty
    {
        [Test, Category("Variables")]
        public void ReservedVariables()
        {
            Assert.IsTrue((int)Core.GetEnv("a_tickCount") > 0);
        }
    }
}
