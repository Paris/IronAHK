using IronAHK.Rusty;
using NUnit.Framework;

namespace IronAHK.Tests
{
    partial class Rusty
    {
        // TODO: loop tests

        [Test, Category("Loop")]
        public void Loop()
        {
            const int n = 10;
            int x = 0;

            Assert.AreEqual(0, Core.A_Index);

            foreach (int i in Core.Loop(n))
            {
                Assert.AreEqual(++x, i);
                Assert.AreEqual(i, Core.A_Index);
            }

            Assert.AreEqual(x, n);
            Assert.AreEqual(0, Core.A_Index);
        }

        [Test, Category("Loop")]
        public void LoopParseCSV()
        {
            string[] items = { "first field", "SecondField", "the word \"special\" is quoted literally", string.Empty, "last field, has literal comma" };
            string list = "\"" + items[0] + "\"," + items[1] + ",\"" + items[2].Replace("\"", "\"\"") + "\"," + items[3] + ",\"" + items[4] + "\"";
            int x = 0;

            Assert.AreEqual(0, Core.A_Index);
            Assert.IsNull(Core.A_LoopField);

            foreach (string value in Core.LoopParse(list, "CSV", null))
            {
                Assert.IsNotNull(value);
                Assert.AreEqual(items[x], Core.A_LoopField);
                Assert.AreEqual(++x, Core.A_Index);
            }

            Assert.AreEqual(x, items.Length);
            Assert.AreEqual(0, Core.A_Index);
            Assert.IsNull(Core.A_LoopField);
        }
    }
}
