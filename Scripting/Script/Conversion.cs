using System;
using System.Collections;
using System.Text;

namespace IronAHK.Scripting
{
    partial class Script
    {
        #region Numeric

        public static double ForceDouble(object input)
        {
            if (input is double)
                return (double)input;
            else if (input is string)
            {
                double result;
                if (double.TryParse((string)input, out result))
                    return result;
            }
            else
                return Convert.ToDouble(input);

            return 0d;
        }

        public static long ForceLong(object input)
        {
            if (input is long)
                return (long)input;
            else if (input is string)
            {
                long result;
                if (long.TryParse((string)input, out result))
                    return result;
            }
            else
                return Convert.ToInt64(input);

            return 0;
        }

        public static int ForceInt(object input)
        {
            if (input is int)
                return (int)input;
            else if (input is string)
            {
                int result;
                if (int.TryParse((string)input, out result))
                    return result;
            }
            else
                return Convert.ToInt32(input);

            return 0;
        }

        public static decimal ForceDecimal(object input)
        {
            if (input is decimal)
                return (decimal)input;
            else if (input is string)
            {
                decimal result;
                if (decimal.TryParse((string)input, out result))
                    return result;
            }
            else
                return Convert.ToDecimal(input);

            return 0m;
        }

        #endregion

        public static bool ForceBool(object input)
        {
            if (input == null)
                return false;
            else if (input is string)
                return !string.IsNullOrEmpty((string)input);
            else if (input is decimal || input is float || input is double || input is long || input is int)
                return ForceDouble(input) != 0;

            return false;
        }

        public static string ForceString(object input)
        {
            if (input == null)
                return string.Empty;
            else if (input is string)
                return (string)input;
            else if (input is char)
                return ((char)input).ToString();
            else if (input is bool)
                return (bool)input ? "1" : "0";
            else if (input is byte[])
                return Encoding.Unicode.GetString((byte[])input);
            else if (input is decimal)
                return ((decimal)input).ToString();
            else if (input is Delegate)
                return ((Delegate)input).Method.Name;
            else if (IsNumeric(input))
            {
                var t = input.GetType();
                var simple = t == typeof(int) || t == typeof(long);
                var integer = simple || (t == typeof(double) && Math.IEEERemainder((double)input, 1) == 0);
                var format = A_FormatNumeric;
                var hex = format.IndexOf('x') != -1;
                const string hexpre = "0x";

                if (integer)
                {
                    if (!hex)
                        format = "d";

                    var result = simple ? ForceLong(input).ToString(format) : ((int)(double)input).ToString("d");

                    if (hex)
                        result = hexpre + result;

                    return result;
                }

                var d = (double)input;

                if (hex)
                {
                    A_FormatNumeric = null;
                    var result = d.ToString(A_FormatNumeric);
                    A_FormatNumeric = format;
                    return hexpre + result;
                }

                return d.ToString(format);
            }

            var type = input.GetType();
            var buffer = new StringBuilder();

            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                buffer.Append(Parser.BlockOpen);

                var dictionary = (IDictionary)input;
                bool first = true;

                foreach (var key in dictionary.Keys)
                {
                    if (first)
                        first = false;
                    else
                        buffer.Append(Parser.DefaultMulticast);

                    buffer.Append(Parser.StringBound);
                    buffer.Append(ForceString(key));
                    buffer.Append(Parser.StringBound);
                    buffer.Append(Parser.AssignPre);

                    if (dictionary[key] == null)
                    {
                        buffer.Append(Parser.NullTxt);
                        continue;
                    }

                    var subtype = dictionary[key].GetType();
                    bool obj = subtype.IsArray || typeof(IDictionary).IsAssignableFrom(subtype) || dictionary[key] is Delegate;

                    if (!obj)
                        buffer.Append(Parser.StringBound);

                    buffer.Append(ForceString(dictionary[key]));

                    if (!obj)
                        buffer.Append(Parser.StringBound);
                }

                buffer.Append(Parser.BlockClose);
            }
            else if (type.IsArray)
            {
                buffer.Append(Parser.ArrayOpen);

                var array = (Array)input;
                bool first = true;

                foreach (var item in array)
                {
                    if (first)
                        first = false;
                    else
                        buffer.Append(Parser.DefaultMulticast);

                    buffer.Append(ForceString(item));
                }

                buffer.Append(Parser.ArrayClose);
            }
            else
                return string.Empty;

            return buffer.ToString();
        }
    }
}
