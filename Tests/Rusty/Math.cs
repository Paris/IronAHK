using System;
using IronAHK.Rusty;
using NUnit.Framework;

namespace Tests
{
    partial class Rusty
    {
        [Test, Category("Math")]
        public void Trigonometry()
        {
            foreach (decimal d in new decimal[] { 0m, 45m, 90m, 271.3m, (decimal)System.Math.PI * -10 })
            {
                string s;

                s = d.ToString();
                Assert.AreEqual((decimal)System.Math.Sin((double)d), Core.Sin(d), "sin " + s);
                Assert.AreEqual((decimal)System.Math.Cos((double)d), Core.Cos(d), "cos " + s);
                Assert.AreEqual((decimal)System.Math.Tan((double)d), Core.Tan(d), "tan " + s);

                decimal r = d * ((decimal)System.Math.PI / 180);
                s = r.ToString();
                if (r < -1 || r > 1)
                {
                    Assert.Throws<OverflowException>(delegate { Core.ASin(r); }, "asin " + s);
                    Assert.Throws<OverflowException>(delegate { Core.ACos(r); }, "acos " + s);
                }
                else
                {
                    Assert.AreEqual((decimal)System.Math.Asin((double)r), Core.ASin(r), "asin " + s);
                    Assert.AreEqual((decimal)System.Math.Acos((double)r), Core.ACos(r), "acos " + s);
                }
                Assert.AreEqual((decimal)System.Math.Atan((double)r), Core.ATan(r), "atan " + s);
            }
        }
    }
}
