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

            Assert.AreEqual(string.Empty, Core.SubStr(subject, 0, 0));
            Assert.AreEqual(string.Empty, Core.SubStr(subject, -3, 0));
            Assert.AreEqual("i", Core.SubStr(subject, 0, 5));
            Assert.AreEqual("bcd", Core.SubStr(subject, 2, 3));
            Assert.AreEqual("efg", Core.SubStr(subject, 5, -2));
            Assert.AreEqual("cde", Core.SubStr(subject, -6, 3));
            Assert.AreEqual("efg", Core.SubStr(subject, -4, -2));
            Assert.AreEqual("ghi", Core.SubStr(subject, 7));
        }

        [Test, Category("String")]
        public void InStr()
        {
            const string find = "AbC", subject = "abc ABC " + find + " abc";

            Assert.AreEqual(1, Core.InStr(subject, find));
            Assert.AreEqual(9, Core.InStr(subject, find, true));
            Assert.AreEqual(1, Core.InStr(subject, find, false, 1));
            Assert.AreEqual(5, Core.InStr(subject, find, false, 2));
            Assert.AreEqual(13, Core.InStr(subject, find, false, 0));
            Assert.AreEqual(9, Core.InStr(subject, find, true, 0));
            Assert.AreEqual(0, Core.InStr(subject, find, true, -1));
            Assert.AreEqual(0, Core.InStr(subject, find, true, subject.Length));

        }
    }
}
