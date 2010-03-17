using System;
using System.Collections;
using System.Text;

namespace IronAHK.Scripting
{
    partial class Script
    {
        #region Numeric

        public static decimal ForceDecimal(object input)
        {
            if (input is decimal)
                return (decimal)input;
            else if (input is double)
                return (decimal)(double)input;
            else if (input is float)
                return (decimal)(float)input;
            else if (input is long)
                return (decimal)(long)input;
            else if (input is int)
                return (decimal)(int)input;
            else if (input is string)
            {
                decimal result;
                if (decimal.TryParse((string)input, out result))
                    return result;
            }

            return default(decimal);
        }

        public static long ForceLong(object input)
        {
            if (input is long)
                return (long)input;
            else if (input is decimal)
                return (long)(decimal)input;
            else if (input is double)
                return (long)(double)input;
            else if (input is float)
                return (long)(float)input;
            else if (input is int)
                return (long)(int)input;
            else if (input is string)
            {
                long result;
                if (long.TryParse((string)input, out result))
                    return result;
            }

            return default(long);
        }

        public static int ForceInt(object input)
        {
            if (input is int)
                return (int)input;
            else if (input is decimal)
                return (int)(decimal)input;
            else if (input is double)
                return (int)(double)input;
            else if (input is float)
                return (int)(float)input;
            else if (input is long)
                return (int)(long)input;
            else if (input is string)
            {
                int result;
                if (int.TryParse((string)input, out result))
                    return result;
            }

            return default(int);
        }

        #endregion

        public static bool ForceBool(object input)
        {
            if (input == null)
                return false;
            else if (input is string)
                return !string.IsNullOrEmpty((string)input);
            else if (input is decimal || input is float || input is double || input is long || input is int)
                return ForceDecimal(input) != 0;

            return false;
        }

        public static string ForceString(object input)
        {
            if (input == null)
                return string.Empty;
            else if (input is string)
                return (string)input;
            else if (input is bool)
                return (bool)input ? "1" : "0";
            else if (IsNumeric(input))
                return input.ToString();

            var type = input.GetType();
            var buffer = new StringBuilder();

            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                buffer.Append(Parser.BlockOpen);

                var dictionary = (IDictionary)input;
                bool first = true;

                foreach (object key in dictionary.Keys)
                {
                    if (first)
                        first = false;
                    else
                        buffer.Append(Parser.Multicast);

                    buffer.Append(Parser.StringBound);
                    buffer.Append(ForceString(key));
                    buffer.Append(Parser.StringBound);
                    buffer.Append(Parser.AssignPre);

                    var subtype = dictionary[key].GetType();
                    bool obj = subtype.IsArray || typeof(IDictionary).IsAssignableFrom(subtype);

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

                foreach (object item in array)
                {
                    if (first)
                        first = false;
                    else
                        buffer.Append(Parser.Multicast);

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
