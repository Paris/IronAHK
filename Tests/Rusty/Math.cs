using System;
using System.Collections.Generic;
using System.Threading;
using IronAHK.Rusty;
using NUnit.Framework;

namespace IronAHK.Tests
{
    partial class Rusty
    {
        [Test, Category("Math")]
        public void Abs()
        {
            Assert.AreEqual(1, Core.Abs(1));
            Assert.AreEqual(1, Core.Abs(-1));
            Assert.AreEqual(9.81, Core.Abs(-9.81m));
            Assert.AreEqual(0, Core.Abs(-0));
        }

        [Test, Category("Math")]
        public void ACos()
        {
            foreach (var n in new[] { -1, -0.5, 0, 0.5, 1, 0.675 })
                Assert.AreEqual(Math.Acos(n), Core.ACos(n));
        }

        [Test, Category("Math")]
        public void ASin()
        {
            foreach (var n in new[] { -1, -0.5, 0, 0.5, 1, 0.675 })
                Assert.AreEqual(Math.Asin(n), Core.ASin(n));
        }

        [Test, Category("Math")]
        public void ATan()
        {
            foreach (var n in new[] { -1, -0.5, 0, 0.5, 1, 0.675 })
                Assert.AreEqual(Math.Atan(n), Core.ATan(n));
        }

        [Test, Category("Math")]
        public void Ceil()
        {
            foreach (var n in new[] { -1m, -2.1m, 0m, 1.000001m })
                Assert.AreEqual(Math.Ceiling(n), Core.Ceil(n));
        }

        [Test, Category("Math")]
        public void Cos()
        {
            foreach (var n in new[] { -1, -0.5, 0, 0.5, 1, 0.675 })
            {
                var v = n * Math.PI;
                Assert.AreEqual(Math.Cos(v), Core.Cos(v));
            }
        }

        [Test, Category("Math")]
        public void Cosh()
        {
            foreach (var n in new[] { -1, -0.5, 0, 0.5, 1, 0.675 })
            {
                var v = n * Math.PI;
                Assert.AreEqual(Math.Cosh(v), Core.Cosh(v));
            }
        }

        [Test, Category("Math")]
        public void EnvAdd()
        {
            const double d = 100;
            double n;

            n = 0;
            Core.EnvAdd(ref n, d);
            Assert.AreEqual(0 + d, n);

            n = 0;
            Core.EnvAdd(ref n, -d);
            Assert.AreEqual(0 + -d, n);

            n = 0;
            Core.EnvAdd(ref n, d, null);
            Assert.AreEqual(0 + d, n);

            n = 0;
            Core.EnvAdd(ref n, d, string.Empty);
            Assert.AreEqual(0 + d, n);

            var time = DateTime.Now;
            var list = new Dictionary<DateTime, string[]>(6);
            list.Add(time.AddSeconds(d), new[] { "s", "S", "Seconds" });
            list.Add(time.AddMinutes(d), new[] { "m", "M", "Minutes" });
            list.Add(time.AddHours(d), new[] { "h", "h", "Hours" });
            list.Add(time.AddDays(d), new[] { "d", "d", "Days" });
            list.Add(time.AddMonths((int)d), new[] { "mn", "MN", "Months" });
            list.Add(time.AddYears((int)d), new[] { "y", "Y", "Years" });

            foreach (var offset in list.Keys)
            {
                foreach (var unit in list[offset])
                {
                    n = Core.FromTime(time);
                    Core.EnvAdd(ref n, d, unit);
                    Assert.AreEqual(Core.FromTime(offset), n, unit);
                }
            }
        }

        [Test, Category("Math")]
        public void Exp()
        {
            foreach (var n in new[] { -1, -0.5, 0, 0.5, 1, 0.675 })
                Assert.AreEqual(Math.Exp(n), Core.Exp(n));
        }

        [Test, Category("Math")]
        public void Floor()
        {
            foreach (var n in new[] { -1m, -0.5m, 0m, 0.5m, 1m, 0.675m })
                Assert.AreEqual(Math.Floor(n), Core.Floor(n));
        }

        [Test, Category("Math")]
        public void Ln()
        {
            foreach (var n in new[] { -1, -0.5, 0, 0.5, 1, 0.675 })
                Assert.AreEqual(Math.Log(n), Core.Ln(n));
        }

        [Test, Category("Math")]
        public void Log()
        {
            foreach (var n in new[] { -1, -0.5, 0, 0.5, 1, 0.675 })
                foreach (var b in new[] { -1, 0, 1, 2, 3, 4 })
                    Assert.AreEqual(Math.Log(n, b), Core.Log(n, b));

            foreach (var n in new[] { -1, -0.5, 0, 0.5, 1, 0.675 })
                Assert.AreEqual(Math.Log10(n), Core.Log(n));
        }

        [Test, Category("Math")]
        public void Mod()
        {
            foreach (var n in new[] { -1m, 0m, 1m, 2m, 3m, 4m })
                foreach (var d in new[] { -1m, -0.5m, 0m, 0.5m, 1m, 0.675m })
                    Assert.AreEqual(d == 0 ? 0 : n % d, Core.Mod(n, d));
        }

        [Test, Category("Math")]
        public void Random()
        {
            double n;

            n = -1;
            Core.Random(out n, 0, 0);
            Assert.AreEqual(0, n);

            Core.Random(out n, -1, -1);
            Assert.AreEqual(-1, n);

            Core.Random(out n, -3, -1);
            Assert.IsTrue(n == -3 || n == -2);

            Core.Random(out n, 0.1, 1);
            Assert.IsTrue(n >= 0.1 && n < 1);

            var c = new double[100];

            Core.Random(out c[0]);
            Thread.Sleep(50);
            
            for (var i = 1; i < c.Length; i++)
                Core.Random(out c[i]);

            var eq = true;

            for (var i = 1; i < c.Length; i++)
                if (!(eq = c[i] == c[i - 1]))
                    break;

            Assert.IsFalse(eq);
        }

        [Test, Category("Math")]
        public void Remainder()
        {
            foreach (var n in new[] { -1, -0.5, 0, 0.5, 1, 0.675 })
                foreach (var d in new[] { 2, 1 })
                    Assert.AreEqual(Math.IEEERemainder(n, d), Core.Remainder(n, d));

            Assert.AreEqual(0, Core.Remainder(1, 0));
        }

        [Test, Category("Math")]
        public void Round()
        {
            foreach (var n in new[] { -1m, -0.5m, 0m, 0.5m, 1m, 0.675m })
                foreach (var d in new[] { 2, 1, 0 })
                    Assert.AreEqual(Math.Round(n, d), Core.Round(n, d));

            Assert.AreEqual(5, Core.Round(5.3m, -1));
        }

        [Test, Category("Math")]
        public void Sin()
        {
            foreach (var n in new[] { -1, -0.5, 0, 0.5, 1, 0.675 })
            {
                var v = n * Math.PI;
                Assert.AreEqual(Math.Sin(v), Core.Sin(v));
            }
        }

        [Test, Category("Math")]
        public void Sinh()
        {
            foreach (var n in new[] { -1, -0.5, 0, 0.5, 1, 0.675 })
            {
                var v = n * Math.PI;
                Assert.AreEqual(Math.Sinh(v), Core.Sinh(v));
            }
        }

        [Test, Category("Math")]
        public void Sqrt()
        {
            foreach (var n in new[] { 0, 1, 4, 9, 36, 12769, 8 })
                Assert.AreEqual(Math.Sqrt(n), Core.Sqrt(n));

            Assert.AreEqual(0, Core.Sqrt(-1));
        }

        [Test, Category("Math")]
        public void Tan()
        {
            foreach (var n in new[] { -1, -0.5, 0, 0.5, 1, 0.675 })
            {
                var v = n * Math.PI;
                Assert.AreEqual(Math.Tan(v), Core.Tan(v));
            }
        }

        [Test, Category("Math")]
        public void Tanh()
        {
            foreach (var n in new[] { -1, -0.5, 0, 0.5, 1, 0.675 })
            {
                var v = n * Math.PI;
                Assert.AreEqual(Math.Tanh(v), Core.Tanh(v));
            }
        }

        [Test, Category("Math")]
        public void Truncate()
        {
            foreach (var n in new[] { -1m, -4.5m, 0m, 2.5m, 1m, 8.675m })
                Assert.AreEqual(Math.Truncate(n), Core.Truncate(n));
        }
    }
}
