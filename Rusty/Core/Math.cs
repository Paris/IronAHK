using System;
using System.Diagnostics;

namespace IronAHK.Rusty
{
    partial class Core
    {
        /// <summary>
        /// Returns the absolute value of a number.
        /// </summary>
        /// <param name="n">Any number.</param>
        /// <returns>The magnitude of <paramref name="n"/>.</returns>
        public static decimal Abs(decimal n)
        {
            return Math.Abs(n);
        }

        /// <summary>
        /// Returns the angle whose cosine is the specified number.
        /// </summary>
        /// <param name="n">A number representing a cosine, where -1 ≤ <paramref name="n"/> ≤ 1.</param>
        /// <returns>An angle, θ, measured in radians, such that 0 ≤ θ ≤ π.</returns>
        public static double ACos(double n)
        {
            return Math.Acos(n);
        }

        /// <summary>
        /// Returns the angle whose sine is the specified number.
        /// </summary>
        /// <param name="n">A number representing a sine, where -1 ≤ <paramref name="n"/> ≤ 1.</param>
        /// <returns>An angle, θ, measured in radians, such that -π/2 ≤ θ ≤ π/2.</returns>
        public static double ASin(double n)
        {
            return Math.Asin(n);
        }

        /// <summary>
        /// Returns the angle whose tangent is the specified number.
        /// </summary>
        /// <param name="n">A number representing a tangent.</param>
        /// <returns>An angle, θ, measured in radians, such that -π/2 ≤ θ ≤ π/2.</returns>
        public static double ATan(double n)
        {
            return Math.Atan(n);
        }

        /// <summary>
        /// Returns the smallest integer greater than or equal to the specified decimal number.
        /// </summary>
        /// <param name="n">A number.</param>
        /// <returns>The smallest integer greater than or equal to <paramref name="n"/>.</returns>
        public static decimal Ceil(decimal n)
        {
            return Math.Ceiling(n);
        }

        /// <summary>
        /// Returns the cosine of the specified angle.
        /// </summary>
        /// <param name="n">An angle, measured in radians.</param>
        /// <returns>The cosine of <paramref name="n"/>.</returns>
        public static double Cos(double n)
        {
            return Math.Cos(n);
        }

        /// <summary>
        /// Returns the hyperbolic cosine of the specified angle.
        /// </summary>
        /// <param name="n">An angle, measured in radians.</param>
        /// <returns>The hyperbolic cosine of <paramref name="n"/>.</returns>
        public static double Cosh(double n)
        {
            return Math.Cosh(n);
        }

        /// <summary>
        /// Add a value to a variable using numeric or date-time arithmetic.
        /// </summary>
        /// <param name="var">A variable.</param>
        /// <param name="value">A number.</param>
        /// <param name="units">
        /// To use date arithmetic specify one of the following words:
        /// <c>seconds</c> (<c>s</c>), <c>minutes</c> (<c>m</c>), <c>hours</c> (<c>h</c>), <c>days</c> (<c>d</c>), <c>months</c> or <c>years</c> (<c>y</c>).
        /// If this parameter is blank the functions performs a numeric addition.
        /// </param>
        public static void EnvAdd(ref double var, double value, string units = null)
        {
            if (string.IsNullOrEmpty(units))
            {
                var += value;
                return;
            }

            var time = ToDateTime(((int)var).ToString());

            switch (units.ToLowerInvariant())
            {
                case Keyword_Seconds:
                case "s":
                    time = time.AddSeconds(value);
                    break;

                case Keyword_Minutes:
                case "m":
                    time = time.AddMinutes(value);
                    break;

                case Keyword_Hours:
                case "h":
                    time = time.AddHours(value);
                    break;

                case Keyword_Days:
                case "d":
                    time = time.AddDays(value);
                    break;

                case Keyword_Months:
                case "mn":
                    time = time.AddMonths((int)value);
                    break;

                case Keyword_Years:
                case "y":
                    time = time.AddYears((int)value);
                    break;
            }

            var = FromTime(time);
        }

        /// <summary>
        /// See <see cref="EnvAdd"/>.
        /// </summary>
        /// <param name="var">A variable.</param>
        /// <param name="value">A value.</param>
        /// <param name="units">A numeric unit.</param>
        [Obsolete]
        public static void EnvSub(ref double var, double value, string units = null)
        {
            EnvAdd(ref var, -value, units);
        }

        /// <summary>
        /// Returns <c>e</c> raised to the specified power.
        /// </summary>
        /// <param name="n">A number specifying a power.</param>
        /// <returns>The number <c>e</c> raised to the power <paramref name="n"/>.</returns>
        public static double Exp(double n)
        {
            return Math.Exp(n);
        }

        /// <summary>
        /// Returns the largest integer less than or equal to the specified decimal number.
        /// </summary>
        /// <param name="n">A number.</param>
        /// <returns>The largest integer less than or equal to <paramref name="n"/>.</returns>
        public static decimal Floor(decimal n)
        {
            return Math.Floor(n);
        }

        /// <summary>
        /// Returns the natural (base e) logarithm of a specified number.
        /// </summary>
        /// <param name="n">A number whose logarithm is to be found.</param>
        /// <returns>The natural logarithm of <paramref name="n"/>.</returns>
        public static double Ln(double n)
        {
            return Math.Log(n);
        }

        /// <summary>
        /// Returns the logarithm of a specified number in a specified base.
        /// </summary>
        /// <param name="n">A number whose logarithm is to be found.</param>
        /// <param name="b">The base of the logarithm. If unspecified this is <c>10</c>.</param>
        /// <returns>The logarithm of <paramref name="n"/> to base <paramref name="b"/>.</returns>
        public static double Log(double n, double b = 10)
        {
            return b == 10 ? Math.Log10(n) : Math.Log(n, b);
        }

        /// <summary>
        /// Returns the remainder after dividing two numbers.
        /// </summary>
        /// <param name="dividend">The dividend.</param>
        /// <param name="divisor">The divisor.</param>
        /// <returns>The remainder after dividing <paramref name="dividend"/> by <paramref name="divisor"/>.</returns>
        public static decimal Mod(decimal dividend, decimal divisor)
        {
            return divisor == 0 ? 0 : dividend % divisor;
        }

        /// <summary>
        /// Returns a random number within a specified range.
        /// </summary>
        /// <param name="result">The name of the variable in which to store the result.</param>
        /// <param name="min">The inclusive lower bound of the random number returned.</param>
        /// <param name="max">The exclusive upper bound of the random number returned.</param>
        /// <remarks>If <paramref name="min"/> and <paramref name="max"/> are both integers <paramref name="result"/> will also be an integer.
        /// Otherwise <paramref name="result"/> can be a floating point number.</remarks>
        public static void Random(out double result, double min = int.MinValue, double max = int.MaxValue)
        {
            var r = RandomGenerator;
            double x = Math.IEEERemainder(min, 1), y = Math.IEEERemainder(max, 1), z = r.Next((int)min, (int)max);

            if (x != 0 || y != 0)
                z += (r.NextDouble() % Math.Abs(y - x)) + x;

            result = z;
        }

        /// <summary>
        /// Returns the remainder resulting from the division of a specified number by another specified number.
        /// </summary>
        /// <param name="x">A dividend.</param>
        /// <param name="y">A divisor.</param>
        /// <returns>A number equal to <c><paramref name="x"/> - (<paramref name="y"/> Q)</c>,
        /// where <c>Q</c> is the quotient of <c><paramref name="x"/> / <paramref name="y"/></c> rounded to
        /// the nearest integer (if <c><paramref name="x"/> / y</c> falls halfway between two integers, the even integer is returned).
        /// 
        /// If <c><paramref name="x"/> - (<paramref name="y"/> Q)</c> is zero, the value <c>0</c> is returned.
        /// 
        /// If <c><paramref name="y"/> = 0</c>, <c>0</c> is returned.</returns>
        public static double Remainder(double x, double y)
        {
            return y == 0 ? 0 : Math.IEEERemainder(x, y);
        }

        /// <summary>
        /// Rounds a number to a specified number of fractional digits.
        /// </summary>
        /// <param name="n">A decimal number to be rounded.</param>
        /// <param name="decimals">The number of decimal places in the return value.</param>
        /// <returns>The number nearest to <paramref name="n"/> that contains a number of fractional digits equal to <paramref name="decimals"/>.</returns>
        public static decimal Round(decimal n, int decimals)
        {
            return Math.Round(n, Math.Max(0, decimals));
        }

        /// <summary>
        /// Returns the sine of the specified angle.
        /// </summary>
        /// <param name="n">An angle, measured in radians.</param>
        /// <returns>The sine of <paramref name="n"/>.</returns>
        public static double Sin(double n)
        {
            return Math.Sin(n);
        }

        /// <summary>
        /// Returns the hyperbolic sine of the specified angle.
        /// </summary>
        /// <param name="n">An angle, measured in radians.</param>
        /// <returns>The hyperbolic sine of <paramref name="n"/>.</returns>
        public static double Sinh(double n)
        {
            return Math.Sinh(n);
        }

        /// <summary>
        /// Returns the square root of a specified number.
        /// </summary>
        /// <param name="n">A number.</param>
        /// <returns>The positive square root of <paramref name="n"/>.</returns>
        public static double Sqrt(double n)
        {
            return n < 0 ? 0 : Math.Sqrt(n);
        }

        /// <summary>
        /// Returns the tangent of the specified angle.
        /// </summary>
        /// <param name="n">An angle, measured in radians.</param>
        /// <returns>The tangent of <paramref name="n"/>.</returns>
        public static double Tan(double n)
        {
            return Math.Tan(n);
        }

        /// <summary>
        /// Returns the hyperbolic tangent of the specified angle.
        /// </summary>
        /// <param name="n">An angle, measured in radians.</param>
        /// <returns>The hyperbolic tangent of <paramref name="n"/>.</returns>
        public static double Tanh(double n)
        {
            return Math.Tanh(n);
        }

        /// <summary>
        /// Calculates the integral part of a specified number.
        /// </summary>
        /// <param name="n">A number to truncate.</param>
        /// <returns>The integral part of <paramref name="n"/>; that is, the number that remains after any fractional digits have been discarded.</returns>
        public static decimal Truncate(decimal n)
        {
            return Math.Truncate(n);
        }
    }
}
