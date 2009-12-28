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
        /// Sets a variable to itself divided by the given value. Synonymous with: var /= value
        /// </summary>
        /// <param name="Var">The name of the variable upon which to operate.</param>
        /// <param name="Value">Any integer or floating point number.</param>
        [Obsolete, Conditional("LEGACY")]
        public static void EnvDiv(ref double Var, double Value)
        {
            Var /= Value;
        }
        
        /// <summary>
        /// Sets a variable to itself times the given value. Synonymous with: var *= value
        /// </summary>
        /// <param name="Var">The name of the variable upon which to operate.</param>
        /// <param name="Value">Any integer or floating point number.</param>
        [Obsolete, Conditional("LEGACY")]
        public static void EnvMult(ref double Var, double Value)
        {
            Var *= Value;
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
        /// 
        /// </summary>
        /// <param name="var"></param>
        /// <param name="value"></param>
        [Obsolete, Conditional("FLOW")]
        public static void IfEqual(ref string var, string value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="var"></param>
        /// <param name="value"></param>
        [Obsolete, Conditional("FLOW")]
        public static void IfGreater(ref string var, string value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="var"></param>
        /// <param name="value"></param>
        [Obsolete, Conditional("FLOW")]
        public static void IfGreaterOrEqual(ref string var, string value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="var"></param>
        /// <param name="value"></param>
        [Obsolete, Conditional("FLOW")]
        public static void IfLess(ref string var, string value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="var"></param>
        /// <param name="value"></param>
        [Obsolete, Conditional("FLOW")]
        public static void IfLessOrEqual(ref string var, string value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="var"></param>
        /// <param name="value"></param>
        [Obsolete, Conditional("FLOW")]
        public static void IfNotEqual(ref string var, string value)
        {
            throw new NotSupportedException();
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

        /// <summary>
        /// Sets the format of integers and floating point numbers generated by math operations.
        /// </summary>
        /// <param name="NumberType">Must be either INTEGER or FLOAT.</param>
        /// <param name="Format">
        /// <para>For NumberType INTEGER, this parameter must be H or HEX for hexadecimal, or D for decimal. Hexadecimal numbers all start with the prefix 0x (e.g. 0xFF). </para>
        /// <para>For NumberType FLOAT, this parameter is TotalWidth.DecimalPlaces (e.g. 0.6). In v1.0.46.11+, the letter "e" may be appended to produce scientific notation; e.g. 0.6e or 0.6E (using uppercase produces an uppercase E in each number instead of lowercase). Note: In AutoHotkey 1.x, scientific notation must include a decimal point; e.g. 1.0e1 is valid but not 1e1.</para>
        /// <para>TotalWidth is typically 0 to indicate that number should not have any blank or zero padding. If a higher value is used, numbers will be padded with spaces or zeroes (see below) to make them that wide.</para>
        /// <para>DecimalPlaces is the number of decimal places to display (rounding will occur). If blank or zero, neither a decimal portion nor a decimal point will be displayed, that is, the number will be stored as an integer rather than a floating point number.</para>
        /// <para>Padding: If TotalWidth is high enough to cause padding, spaces will be added on the left side; that is, each number will be right-justified. To use left-justification instead, precede TotalWidth with a minus sign. To pad with zeroes instead of spaces, precede TotalWidth with a zero (e.g. 06.2).</para>
        /// </param>
        public static void SetFormat(string NumberType, string Format)
        {
            switch (NumberType.ToLowerInvariant())
            {
                case Keyword_Integer:
                    _FormatInteger = Format[0];
                    break;

                case Keyword_Float:
                    _FormatFloat = Format;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Performs miscellaneous math functions, bitwise operations, and tasks such as ASCII/Unicode conversion.
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the result of Cmd. SetFormat determines whether integers are stored as hexadecimal or decimal.</param>
        /// <param name="Cmd">See list below.</param>
        /// <param name="Value1">See list below.</param>
        /// <param name="Value2">See list below.</param>
        [Obsolete, Conditional("LEGACY")]
        public static void Transform(ref string OutputVar, string Cmd, string Value1, string Value2)
        {
            OutputVar = string.Empty;
            switch (Cmd.Trim().ToLowerInvariant())
            {
                case Keyword_Unicode:
                    if (Value1 == null)
                        OutputVar = Clipboard.GetText();
                    else OutputVar = Value1;
                    break;
                case Keyword_Asc:
                    OutputVar = char.GetNumericValue(Value1, 0).ToString();
                    break;
                case Keyword_Chr:
                    OutputVar = char.ConvertFromUtf32(int.Parse(Value1));
                    break;
                case Keyword_Deref:
                    // TODO: dereference transform
                    break;
                case "html":
                    OutputVar = Value1
                        .Replace("\"", "&quot;")
                        .Replace("&", "&amp;")
                        .Replace("<", "&lt;")
                        .Replace(">", "&gt;")
                        .Replace("\n", "<br/>\n");
                    break;
                case Keyword_Mod:
                    OutputVar = (double.Parse(Value1) % double.Parse(Value2)).ToString();
                    break;
                case Keyword_Pow:
                    OutputVar = Math.Pow(double.Parse(Value1), double.Parse(Value2)).ToString();
                    break;
                case Keyword_Exp:
                    OutputVar = Math.Pow(double.Parse(Value1), Math.E).ToString();
                    break;
                case Keyword_Sqrt:
                    OutputVar = Math.Sqrt(double.Parse(Value1)).ToString();
                    break;
                case Keyword_Log:
                    OutputVar = Math.Log10(double.Parse(Value1)).ToString();
                    break;
                case Keyword_Ln:
                    OutputVar = Math.Log(double.Parse(Value1), Math.E).ToString();
                    break;
                case Keyword_Round:
                    int p = int.Parse(Value2);
                    OutputVar = Math.Round(double.Parse(Value1), p == 0 ? 1 : p).ToString();
                    break;
                case Keyword_Ceil:
                    OutputVar = Math.Ceiling(double.Parse(Value1)).ToString();
                    break;
                case Keyword_Floor:
                    OutputVar = Math.Floor(double.Parse(Value1)).ToString();
                    break;
                case Keyword_Abs:
                    double d = double.Parse(Value1);
                    OutputVar = (d < 0 ? d * -1 : d).ToString();
                    break;
                case Keyword_Sin:
                    OutputVar = Math.Sin(double.Parse(Value1)).ToString();
                    break;
                case Keyword_Cos:
                    OutputVar = Math.Cos(double.Parse(Value1)).ToString();
                    break;
                case Keyword_Tan:
                    OutputVar = Math.Tan(double.Parse(Value1)).ToString();
                    break;
                case Keyword_Asin:
                    OutputVar = Math.Asin(double.Parse(Value1)).ToString();
                    break;
                case Keyword_Acos:
                    OutputVar = Math.Acos(double.Parse(Value1)).ToString();
                    break;
                case Keyword_Atan:
                    OutputVar = Math.Atan(double.Parse(Value1)).ToString();
                    break;
                case Keyword_BitNot:
                    OutputVar = (~int.Parse(Value1)).ToString();
                    break;
                case Keyword_BitAnd:
                    OutputVar = (int.Parse(Value1) & int.Parse(Value2)).ToString();
                    break;
                case Keyword_BitOr:
                    OutputVar = (int.Parse(Value1) | int.Parse(Value2)).ToString();
                    break;
                case Keyword_BitXor:
                    OutputVar = (int.Parse(Value1) ^ int.Parse(Value2)).ToString();
                    break;
                case Keyword_BitShiftLeft:
                    OutputVar = (int.Parse(Value1) << int.Parse(Value2)).ToString();
                    break;
                case Keyword_BitShiftRight:
                    OutputVar = (int.Parse(Value1) >> int.Parse(Value2)).ToString();
                    break;
            }
        }
    }
}