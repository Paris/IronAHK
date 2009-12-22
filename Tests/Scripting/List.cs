using NUnit.Framework;

namespace IronAHK.Tests
{
    partial class Scripting
    {
        [Test]
        public void SimplePass()
        {
            Assert.IsTrue(TestScript("pass"));
        }

        [Test]
        public void If()
        {
            Assert.IsTrue(TestScript("if"));
        }

        [Test]
        public void Functions()
        {
            Assert.IsTrue(TestScript("functions"));
        }

        [Test]
        public void Comments()
        {
            Assert.IsTrue(TestScript("comments"));
        }

        [Test]
        public void Directive()
        {
            Assert.IsTrue(TestScript("directive"));
        }

        [Test]
        public void Command()
        {
            Assert.IsTrue(TestScript("command"));
        }

        [Test]
        public void Loop()
        {
            Assert.IsTrue(TestScript("loop"));
        }

        [Test]
        public void Assign()
        {
            Assert.IsTrue(TestScript("assign"));
        }

        [Test]
        public void Goto()
        {
            Assert.IsTrue(TestScript("goto"));
        }

        [Test]
        public void Hotkey()
        {
            Assert.IsTrue(TestScript("hotkey"));
        }
    }
}
