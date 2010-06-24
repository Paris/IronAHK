using IronAHK.Rusty;
using NUnit.Framework;

namespace IronAHK.Tests
{
    partial class Rusty
    {
        // TODO: string tests

        [Test, Category("String")]
        public void SubStr()
        {
            const string subject = "abcdefghi";

            Assert.AreEqual("i", Core.SubStr(subject, 0, 0));
            Assert.AreEqual("bcd", Core.SubStr(subject, 2, 3));
            Assert.AreEqual("fghi", Core.SubStr(subject, -3, 0));
            Assert.AreEqual("efg", Core.SubStr(subject, -4, -2));
        }
    }
}
