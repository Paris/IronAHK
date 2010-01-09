using IronAHK.Rusty;
using NUnit.Framework;

namespace IronAHK.Tests
{
    partial class Rusty
    {
        [Test, Category("Variables")]
        public void Variables()
        {
            const string value = "qwerty";
            Core.SetEnv("AbCd", value);
            Assert.AreEqual(value, Core.GetEnv("abcd"), "Case insensitive variable reference");
        }

        [Test, Category("Variables")]
        public void ReservedVariables()
        {
            Assert.IsTrue((int)Core.GetEnv("a_tickCount") > 0);
        }
    }
}
