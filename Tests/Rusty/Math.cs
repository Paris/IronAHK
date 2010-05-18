using System;
using IronAHK.Rusty;
using NUnit.Framework;

namespace IronAHK.Tests
{
    partial class Rusty
    {
        // TODO: math tests

        [Test, Category("Math")]
        public void Trigonometry()
        {
            foreach (var d in new[] { 0m, 45m, 90m, 271.3m, (decimal)Math.PI * -10 })
            {
                string s;

                s = d.ToString();
                Assert.AreEqual((decimal)Math.Sin((double)d), Core.Sin(d), "sin " + s);
                Assert.AreEqual((decimal)Math.Cos((double)d), Core.Cos(d), "cos " + s);
                Assert.AreEqual((decimal)Math.Tan((double)d), Core.Tan(d), "tan " + s);

                decimal r = d * ((decimal)Math.PI / 180);
                s = r.ToString();
                if (r < -1 || r > 1)
                {
                    try
                    {
                        Core.ASin(r);
                        throw new ArithmeticException();
                    }
                    catch (Exception e) { Assert.IsTrue(e is OverflowException, "asin " + s); }
                    try
                    {
                        Core.ACos(r);
                        throw new ArithmeticException();
                    }
                    catch (Exception e) { Assert.IsTrue(e is OverflowException, "acos " + s); }
                }
                else
                {
                    Assert.AreEqual((decimal)Math.Asin((double)r), Core.ASin(r), "asin " + s);
                    Assert.AreEqual((decimal)Math.Acos((double)r), Core.ACos(r), "acos " + s);
                }
                Assert.AreEqual((decimal)Math.Atan((double)r), Core.ATan(r), "atan " + s);
            }
        }
    }
}
