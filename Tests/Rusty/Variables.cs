using IronAHK.Scripting;
using NUnit.Framework;

namespace IronAHK.Tests
{
    partial class Rusty
    {
        [Test, Category("Variables")]
        public void ReservedVariables()
        {
            Assert.IsTrue((int)Script.Vars["a_tickCount"] > 0);
        }
    }
}
