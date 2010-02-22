using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Core
    {
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
                    x.AddSeconds((double)Value);
                    break;

                case Keyword_Minutes:
                case "m":
                    x.AddMinutes((double)Value);
                    break;

                case Keyword_Hours:
                case "h":
                    x.AddHours((double)Value);
                    break;

                case Keyword_Days:
                case "d":
                    x.AddDays((double)Value);
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
        /// Generates a pseudo-random number.
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the result. The format of stored floating point numbers is determined by SetFormat.</param>
        /// <param name="Min">The smallest number that can be generated, which can be negative or floating point. If omitted, the smallest number will be 0. The lowest allowed value is -2147483648 for integers, but floating point numbers have no restrictions.</param>
        /// <param name="Max">The largest number that can be generated, which can be negative or floating point. If omitted, the largest number will be 2147483647 (which is also the largest allowed integer value -- but floating point numbers have no restrictions).</param>
        public static void Random(out double OutputVar, double Min, double Max)
        {
            System.Random r = new Random();
            double x = Math.IEEERemainder(Min, 1), y = Math.IEEERemainder(Max, 1), z = r.Next((int)Min, (int)Max);

            if (x != 0 || y != 0)
                z += (r.NextDouble() % Math.Abs(y - x)) + x;

            OutputVar = z;
        }
    }
}