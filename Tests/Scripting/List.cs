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
        public void Expressions()
        {
            Assert.IsTrue(TestScript("expressions"));
        }

        [Test]
        public void Command()
        {
            Assert.IsTrue(TestScript("command"));
        }

        [Test]
        public void Line()
        {
            Assert.IsTrue(TestScript("line"));
        }

        [Test]
        public void Loop()
        {
            Assert.IsTrue(TestScript("loop"));
        }

        [Test]
        public void Objects()
        {
            Assert.IsTrue(TestScript("objects"));
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
            Assert.IsTrue(ValidateScript("hotkey"));
        }

        [Test]
        public void VanillaExpressions()
        {
            Assert.IsTrue(ValidateScript("vanilla-Expressions"));
        }

        [Test]
        public void VanillaContinuation()
        {
            Assert.IsTrue(ValidateScript("vanilla-Line Continuation"));
        }

        [Test]
        public void VanillaMain()
        {
            Assert.IsTrue(ValidateScript("vanilla-MAIN"));
        }

        [Test]
        public void VanillaRegex()
        {
            Assert.IsTrue(ValidateScript("vanilla-RegExMatch & RegExReplace"));
        }
    }
}
