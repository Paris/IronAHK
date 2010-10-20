using System;

namespace IronAHK.Rusty
{
    partial class Core
    {
        // TODO: organise Math.cs

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
        /// Returns the arccosine of a number in radians.
        /// </summary>
        /// <param name="Number">-1 &lt; n &lt; 1</param>
        /// <returns></returns>
        public static decimal ACos(decimal Number)
        {
            return (decimal)Math.Acos((double)Number);
        }

        /// <summary>
        /// Returns the ASCII code (a number between 1 and 255) for the first character in a string.
        /// </summary>
        /// <param name="String">A string.</param>
        /// <returns>The ASCII code. If String is empty, 0 is returned.</returns>
        public static decimal Asc(string String)
        {
            return string.IsNullOrEmpty(String) ? 0 : String[0];
        }

        /// <summary>
        /// Returns the arcsine of a number in radians.
        /// </summary>
        /// <param name="Number">-1 &lt; n &lt; 1</param>
        /// <returns></returns>
        public static decimal ASin(decimal Number)
        {
            return (decimal)Math.Asin((double)Number);
        }

        /// <summary>
        /// Returns the arctangent of a number in radians.
        /// </summary>
        /// <param name="Number">-1 &lt; n &lt; 1</param>
        /// <returns></returns>
        public static decimal ATan(decimal Number)
        {
            return (decimal)Math.Atan((double)Number);
        }

        /// <summary>
        /// Returns a number rounded up to the nearest integer.
        /// </summary>
        /// <param name="Number">Any number.</param>
        /// <returns></returns>
        public static decimal Ceil(decimal Number)
        {
            return Math.Ceiling(Number);
        }

        /// <summary>
        /// Returns the single character corresponding to the Unicode value indicated by a number.
        /// </summary>
        /// <param name="Number">A positive integer.</param>
        /// <returns></returns>
        public static string Chr(decimal Number)
        {
            return ((char)Number).ToString();
        }

        /// <summary>
        /// Returns the cosent of a number in radians.
        /// </summary>
        /// <param name="Number">-1 &lt; n &lt; 1</param>
        /// <returns></returns>
        public static decimal Cos(decimal Number)
        {
            return (decimal)Math.Cos((double)Number);
        }

        /// <summary>
        /// Sets a variable to the sum of itself plus the given value (can also add or subtract time from a date-time value). Synonymous with: var += value
        /// </summary>
        /// <param name="Var">The name of the variable upon which to operate.</param>
        /// <param name="Value">Any integer or floating point number.</param>
        /// <param name="TimeUnits">
        /// <para>If present, this parameter directs the command to add Value to Var, treating Var as a date-time stamp in the YYYYMMDDHH24MISS format and treating Value as the integer or floating point number of units to add (specify a negative number to perform subtraction). TimeUnits can be either Seconds, Minutes, Hours, or Days (or just the first letter of each of these).</para>
        /// <para>If Var is an empty variable, the current time will be used in its place. If Var contains an invalid timestamp or a year prior to 1601, or if Value is non-numeric, Var will be made blank to indicate the problem.</para>
        /// <para>The built-in variable A_Now contains the current local time in YYYYMMDDHH24MISS format.</para>
        /// <para>To calculate the amount of time between two timestamps, use EnvSub.</para>
        /// </param>
        public static void EnvAdd(ref int Var, int Value, string TimeUnits)
        {
            if (TimeUnits.Length == 0)
                Var -= Value;

            DateTime x = ToDateTime(Var.ToString());
            switch (TimeUnits.ToLowerInvariant())
            {
                case Keyword_Seconds:
                case "s":
                    x.AddSeconds(Value);
                    break;

                case Keyword_Minutes:
                case "m":
                    x.AddMinutes(Value);
                    break;

                case Keyword_Hours:
                case "h":
                    x.AddHours(Value);
                    break;

                case Keyword_Days:
                case "d":
                    x.AddDays(Value);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            Var = FromTime(x);
        }

        /// <summary>
        /// Sets a variable to itself minus the given value (can also compare date-time values). Synonymous with: var -= value
        /// </summary>
        /// <param name="Var">The name of the variable upon which to operate.</param>
        /// <param name="Value">Any integer or floating point number. Expressions are not supported when TimeUnits is present.</param>
        /// <param name="TimeUnits">
        /// <para>If present, this parameter directs the command to subtract Value from Var as though both of them are date-time stamps in the YYYYMMDDHH24MISS format. TimeUnits can be either Seconds, Minutes, Hours, or Days (or just the first letter of each of these). If Value is blank, the current time will be used in its place. Similarly, if Var is an empty variable, the current time will be used in its place.</para>
        /// <para>The result is always rounded down to the nearest integer. For example, if the actual difference between two timestamps is 1.999 days, it will be reported as 1 day. If higher precision is needed, specify Seconds for TimeUnits and divide the result by 60.0, 3600.0, or 86400.0.</para>
        /// <para>If either Var or Value is an invalid timestamp or contains a year prior to 1601, Var will be made blank to indicate the problem.</para>
        /// <para>The built-in variable A_Now contains the current local time in YYYYMMDDHH24MISS format.</para>
        /// <para>To precisely determine the elapsed time between two events, use the A_TickCount method because it provides millisecond precision.</para>
        /// <para>To add or subtract a certain number of seconds, minutes, hours, or days from a timestamp, use EnvAdd (subtraction is achieved by adding a negative number).</para>
        /// </param>
        public static void EnvSub(ref int Var, int Value, string TimeUnits)
        {
            EnvAdd(ref Var, -Value, TimeUnits);
        }

        /// <summary>
        /// Returns e (which is approximately 2.71828182845905) raised to the Nth power. N may be negative and may contain a decimal point. To raise numbers other than e to a power, use the ** operator.
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static decimal Exp(decimal Number)
        {
            return (decimal)Math.Exp((double)Number);
        }

        /// <summary>
        /// Returns Number rounded down to the nearest integer (without any .00 suffix). For example, Floor(1.2) is 1 and Floor(-1.2) is -2.
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static decimal Floor(decimal Number)
        {
            return Math.Floor(Number);
        }

        /// <summary>
        /// Returns the natural logarithm (base e) of Number. The result is formatted as floating point. If Number is negative, an empty string is returned.
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static decimal Ln(decimal Number)
        {
            return (decimal)Math.Log((double)Number, Math.E);
        }

        /// <summary>
        /// Returns the logarithm (base 10) of Number. The result is formatted as floating point. If Number is negative, an empty string is returned.
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static decimal Log(decimal Number)
        {
            return (decimal)Math.Log10((double)Number);
        }

        /// <summary>
        /// Modulo. Returns the remainder when Dividend is divided by Divisor. The sign of the result is always the same as the sign of the first parameter. For example, both mod(5, 3) and mod(5, -3) yield 2, but mod(-5, 3) and mod(-5, -3) yield -2. If either input is a floating point number, the result is also a floating point number. For example, mod(5.0, 3) is 2.0 and mod(5, 3.5) is 1.5. If the second parameter is zero, the function yields a blank result (empty string).
        /// </summary>
        /// <param name="Dividend"></param>
        /// <param name="Divisor"></param>
        /// <returns></returns>
        public static decimal Mod(decimal Dividend, decimal Divisor)
        {
            return Dividend % Divisor;
        }

        /// <summary>
        /// Generates a pseudo-random number.
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the result. The format of stored floating point numbers is determined by SetFormat.</param>
        /// <param name="Min">The smallest number that can be generated, which can be negative or floating point. If omitted, the smallest number will be 0. The lowest allowed value is -2147483648 for integers, but floating point numbers have no restrictions.</param>
        /// <param name="Max">The largest number that can be generated, which can be negative or floating point. If omitted, the largest number will be 2147483647 (which is also the largest allowed integer value -- but floating point numbers have no restrictions).</param>
        public static void Random(out double OutputVar, double Min, double Max)
        {
            var r = new Random();
            double x = Math.IEEERemainder(Min, 1), y = Math.IEEERemainder(Max, 1), z = r.Next((int)Min, (int)Max);

            if (x != 0 || y != 0)
                z += (r.NextDouble() % Math.Abs(y - x)) + x;

            OutputVar = z;
        }

        /// <summary>
        /// If N is omitted or 0, Number is rounded to the nearest integer. If N is positive number, Number is rounded to N decimal places. If N is negative, Number is rounded by N digits to the left of the decimal point. For example, Round(345, -1) is 350 and Round (345, -2) is 300. Unlike Transform Round, the result has no .000 suffix whenever N is omitted or less than 1. In v1.0.44.01+, a value of N greater than zero displays exactly N decimal places rather than obeying SetFormat. To avoid this, perform another math operation on Round()'s return value; for example: Round(3.333, 1)+0.
        /// </summary>
        /// <param name="Number"></param>
        /// <param name="Places"></param>
        /// <returns></returns>
        public static decimal Round(decimal Number, decimal Places)
        {
            return Math.Round(Number, (int)Places);
        }

        /// <summary>
        /// Returns the trigonometric sine Number. Number must be expressed in radians.
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static decimal Sin(decimal Number)
        {
            return (decimal)Math.Sin((double)Number);
        }

        /// <summary>
        /// Returns the square root of Number. The result is formatted as floating point. If Number is negative, the function yields a blank result (empty string).
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static decimal Sqrt(decimal Number)
        {
            return (decimal)Math.Sqrt((double)Number);
        }

        /// <summary>
        /// Returns the trigonometric tangent of Number. Number must be expressed in radians.
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static decimal Tan(decimal Number)
        {
            return (decimal)Math.Tan((double)Number);
        }
    }
}