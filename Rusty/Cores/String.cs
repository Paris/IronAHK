using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Core
    {
        /// <summary>
        /// Returns the Unicode value (an integer between 1 and 65535) for the specified character in a string.
        /// </summary>
        /// <param name="str">A string.</param>
        /// <param name="n">The zero-based character position in the string.
        /// If this is blank it is assumed to be <c>0</c>.</param>
        /// <returns>The Unicode value.
        /// If <paramref name="str"/> is empty or <paramref name="n"/> is specified out of bounds, <c>0</c> is returned.</returns>
        public static int Asc(string str, int n = 0)
        {
            return string.IsNullOrEmpty(str) || n < 0 || n > str.Length ? 0 : (int)str[n];
        }

        /// <summary>
        /// Encodes binary data to a base 64 character string.
        /// </summary>
        /// <param name="value">The data to encode.</param>
        /// <returns>A base 64 string representation of the given binary data.</returns>
        public static string Base64Encode(object value)
        {
            return Convert.ToBase64String(ToByteArray(value));
        }

        /// <summary>
        /// Decodes a base 64 character string to binary data.
        /// </summary>
        /// <param name="s">The base 64 string to decode.</param>
        /// <returns>A binary byte array of the given sequence.</returns>
        public static byte[] Base64Decode(string s)
        {
            return Convert.FromBase64String(s);
        }

        /// <summary>
        /// Returns the single character corresponding to a Unicode value.
        /// </summary>
        /// <param name="n">A Unicode value.</param>
        /// <returns>A Unicode character whose value is <paramref name="n"/>.</returns>
        public static string Chr(int n)
        {
            return ((char)n).ToString();
        }

        /// <summary>
        /// Transforms a YYYYMMDDHH24MISS timestamp into the specified date/time format.
        /// </summary>
        /// <param name="output">The variable to store the result.</param>
        /// <param name="stamp">Leave this parameter blank to use the current local date and time.
        /// Otherwise, specify all or the leading part of a timestamp in the YYYYMMDDHH24MISS format.</param>
        /// <param name="format">
        /// <para>If omitted, it defaults to the time followed by the long date,
        /// both of which will be formatted according to the current user's locale.</para>
        /// <para>Otherwise, specify one or more of the date-time formats,
        /// along with any literal spaces and punctuation in between.</para>
        /// </param>
        public static void FormatTime(out string output, string stamp, string format)
        {
            DateTime time;
            if (stamp.Length == 0)
                time = DateTime.Now;
            else
            {
                try
                {
                    time = ToDateTime(stamp);
                }
                catch (ArgumentOutOfRangeException)
                {
                    output = null;
                    return;
                }
            }

            switch (format.ToLowerInvariant())
            {
                case Keyword_Time:
                    format = "t";
                    break;

                case Keyword_ShortDate:
                    format = "d";
                    break;

                case Keyword_LongDate:
                    format = "D";
                    break;

                case Keyword_YearMonth:
                    format = "Y";
                    break;

                case Keyword_YDay:
                    output = time.DayOfYear.ToString();
                    return;

                case Keyword_YDay0:
                    output = time.DayOfYear.ToString().PadLeft(3, '0');
                    return;

                case Keyword_WDay:
                    output = time.DayOfWeek.ToString();
                    return;

                case Keyword_YWeek:
                    {
                        int week = System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                        output = time.ToString("Y") + week;
                        return;
                    }

                default:
                    if (format.Length == 0)
                        format = "f";
                    break;
            }

            try
            {
                output = time.ToString(format);
            }
            catch (FormatException)
            {
                output = null;
            }
        }

        /// <summary>
        /// Encodes binary data to a hexadecimal string.
        /// </summary>
        /// <param name="value">The data to encode.</param>
        /// <returns>A hexadecimal string representation of the given binary data.</returns>
        public static string HexEncode(object value)
        {
            return ToString(ToByteArray(value));
        }

        /// <summary>
        /// Decodes a hexadecimal string to binary data.
        /// </summary>
        /// <param name="hex">The hexadecimal string to decode.</param>
        /// <returns>A binary byte array of the given sequence.</returns>
        public static byte[] HexDecode(string hex)
        {
            var binary = new byte[hex.Length / 2];

            for (int i = 0; i < hex.Length; i += 2)
            {
                var n = new string(new[] { hex[i], hex[i + 1] });
                binary[i / 2] = byte.Parse(n, NumberStyles.AllowHexSpecifier);
            }

            return binary;
        }

        /// <summary>
        /// Returns the position of the first or last occurrence of the specified substring within a string.
        /// </summary>
        /// <param name="input">The string to check.</param>
        /// <param name="needle">The substring to search for.</param>
        /// <param name="caseSensitive"><c>true</c> to use a case sensitive comparison, <c>false</c> otherwise.</param>
        /// <param name="index">The one-based starting character position.
        /// Specify zero or leave blank to search in reverse, i.e. right to left.</param>
        /// <returns>The one-based index of the position of <paramref name="needle"/> in <paramref name="input"/>.
        /// A value of zero indicates no match.</returns>
        public static int InStr(string input, string needle, bool caseSensitive = false, int index = 1)
        {
            var compare = caseSensitive ? StringComparison.Ordinal : A_StringComparison;
            const int offset = 1;

            if (index == 0)
                return offset + input.LastIndexOf(needle, compare);

            if (index < 0 || index > input.Length)
                return 0;

            return offset + input.IndexOf(needle, index - 1, compare);
        }

        /// <summary>
        /// Determines whether a string contains a pattern (regular expression).
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="needle">The pattern to search for, which is a regular expression.</param>
        /// <param name="output">The variable to store the result.</param>
        /// <param name="index">The one-based starting character position.
        /// If this is less than one it is considered an offset from the end of the string.</param>
        /// <returns>The one-based index of the position of the first match.
        /// A value of zero indicates no match.</returns>
        public static int RegExMatch(string input, string needle, out string[] output, int index)
        {
            Regex exp;
            bool reverse = index < 1;

            try { exp = ParseRegEx(needle, reverse); }
            catch (ArgumentException)
            {
                output = new string[] { };
                ErrorLevel = 2;
                return 0;
            }

            if (index < 0)
            {
                int l = input.Length - 1;
                input = input.Substring(0, Math.Min(l, l + index));
                index = 0;
            }

            index = Math.Max(0, index - 1);
            
            Match res = exp.Match(input, index);

            var matches = new string[res.Groups.Count];
            for (int i = 0; i < res.Groups.Count; i++)
                matches[i] = res.Groups[i].Value;

            output = matches;

            return matches.Length == 0 ? 0 : res.Groups[0].Index + 1;
        }

        /// <summary>
        /// Replaces occurrences of a pattern (regular expression) inside a string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="needle">The pattern to search for, which is a regular expression.</param>
        /// <param name="replace">The string to replace <paramref name="needle"/>.</param>
        /// <param name="count">The variable to store the number of replacements that occurred.</param>
        /// <param name="limit">The maximum number of replacements to perform.
        /// If this is below one all matches will be replaced.</param>
        /// <param name="index">The one-based starting character position.
        /// If this is less than one it is considered an offset from the end of the string.</param>
        /// <returns>The new string.</returns>
        public static string RegExReplace(string input, string needle, string replace, out int count, int limit = -1, int index = 1)
        {
            Regex exp;

            try
            {
                exp = ParseRegEx(needle, index < 1);
            }
            catch (ArgumentException)
            {
                count = 0;
                ErrorLevel = 2;
                return null;
            }

            if (limit < 1)
                limit = int.MaxValue;

            if (index < 1)
                index = input.Length + index - 1;

            index = Math.Min(Math.Max(0, index), input.Length);

            var n = 0;

            MatchEvaluator match = delegate(Match hit)
            {
                n++;
                return replace;
            };

            count = n;
            var result = exp.Replace(input, match, limit, index);
            return result;
        }

        /// <summary>
        /// Arranges a variable's contents in alphabetical, numerical, or random order optionally removing duplicates.
        /// </summary>
        /// <param name="input">The variable whose contents to use as the input.</param>
        /// <param name="options">See the remarks.</param>
        /// <remarks>
        /// <list type="table">
        /// <listheader>
        /// <term>Name</term>
        /// <description>Description</description>
        /// </listheader>
        /// <item>
        /// <term>C</term>
        /// <description>Case sensitive.</description>
        /// </item>
        /// <item>
        /// <term>CL</term>
        /// <description>Case sensitive based on current user's locale.</description>
        /// </item>
        /// <item>
        /// <term>D<c>x</c></term>
        /// <description>Specifies <c>x</c> as the delimiter character which is <c>`n</c> by default.</description>
        /// </item>
        /// <item>
        /// <term>F <c>name</c></term>
        /// <description>Use the return value of the specified function for comparing two items.</description>
        /// </item>
        /// <item>
        /// <term>N</term>
        /// <description>Numeric sorting.</description>
        /// </item>
        /// <item>
        /// <term>P<c>n</c></term>
        /// <description>Sorts items based on character position <c>n</c>.</description>
        /// </item>
        /// <item>
        /// <term>R</term>
        /// <description>Sort in reverse order.</description>
        /// </item>
        /// <item>
        /// <term>Random</term>
        /// <description>Sort in random order.</description>
        /// </item>
        /// <item>
        /// <term>U</term>
        /// <description>Remove any duplicate items.</description>
        /// </item>
        /// <item>
        /// <term>Z</term>
        /// <description>Considers a trailing delimiter as a boundary which otherwise would be ignored.</description>
        /// </item>
        /// <item>
        /// <term>\</term>
        /// <description>File path sorting.</description>
        /// </item>
        /// </list>
        /// </remarks>
        public static void Sort(ref string input, params string[] options)
        {
            var opts = KeyValues(string.Join(",", options), true, new[] { 'f' });
            MethodInfo function = null;

            if (opts.ContainsKey('f'))
            {
                function = FindLocalRoutine(opts['f']);
                if (function == null)
                    return;
            }

            char split = '\n';

            if (opts.ContainsKey('d'))
            {
                string s = opts['d'];
                if (s.Length == 1)
                    split = s[0];
                opts.Remove('d');
            }

            string[] list = input.Split(new[] { split }, StringSplitOptions.RemoveEmptyEntries);

            if (split == '\n')
            {
                for (int i = 0; i < list.Length; i++)
                {
                    int x = list[i].Length - 1;
                    if (list[i][x] == '\r')
                        list[i] = list[i].Substring(0, x);
                }
            }

            if (opts.ContainsKey('z') && input[input.Length - 1] == split)
            {
                Array.Resize(ref list, list.Length + 1);
                list[list.Length - 1] = string.Empty;
                opts.Remove('z');
            }

            bool withcase = false;
            if (opts.ContainsKey('c'))
            {
                string mode = opts['c'];
                if (mode == "l" || mode == "L")
                    withcase = false;
                else
                    withcase = true;
                opts.Remove('c');
            }

            bool numeric = false;
            if (opts.ContainsKey('n'))
            {
                numeric = true;
                opts.Remove('n');
            }

            int sortAt = 1;
            if (opts.ContainsKey('p'))
            {
                if (!int.TryParse(opts['p'], out sortAt))
                    sortAt = 1;
                opts.Remove('p');
            }

            bool reverse = false, random = false;
            if (opts.ContainsKey(Keyword_Random[0]))
            {
                if (opts[Keyword_Random[0]].Equals(Keyword_Random.Substring(1), StringComparison.OrdinalIgnoreCase)) // Random
                    random = true;
                else
                    reverse = true;
                opts.Remove(Keyword_Random[0]);
            }

            bool unique = false;
            if (opts.ContainsKey('u'))
            {
                unique = true;
                opts.Remove('u');
            }

            bool slash = false;
            if (opts.ContainsKey('\\'))
            {
                slash = true;
                opts.Remove('\\');
            }

            var comp = new CaseInsensitiveComparer();
            var rand = new Random();

            Array.Sort(list, delegate(string x, string y)
            {
                if (function != null)
                {
                    object value = null;

                    try { value = function.Invoke(null, new object[] { new object[] { x, y } }); }
                    catch (Exception) { }

                    int result;

                    if (value is string && int.TryParse((string)value, out result))
                        return result;

                    return 0;
                }
                else if (x == y)
                    return 0;
                else if (random)
                    return rand.Next(-1, 2);
                else if (numeric)
                {
                    int a, b;
                    return int.TryParse(x, out a) && int.TryParse(y, out b) ?
                        a.CompareTo(b) : x.CompareTo(y);
                }
                else
                {
                    if (slash)
                    {
                        int z = x.LastIndexOf('\\');
                        if (z != -1)
                            x = x.Substring(z + 1);
                        z = y.LastIndexOf('\\');
                        if (z != -1)
                            y = y.Substring(z + 1);
                        if (x == y)
                            return 0;
                    }
                    return withcase ? x.CompareTo(y) : comp.Compare(x, y);
                }
            });

            if (unique)
            {
                int error = 0;
                var ulist = new List<string>(list.Length);
                foreach (var item in list)
                    if (!ulist.Contains(item))
                        ulist.Add(item);
                    else
                        error++;
                ErrorLevel = error;
                list = ulist.ToArray();
            }

            if (reverse)
                Array.Reverse(list);

            input = string.Join(split.ToString(), list);
        }

        /// <summary>
        /// Returns the length of a string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The total length of the string, including any invisbile characters such as null.</returns>
        public static int StrLen(string input)
        {
            return input.Length;
        }

        /// <summary>
        /// Converts a string to lowercase.
        /// </summary>
        /// <param name="output">The variable to store the result.</param>
        /// <param name="input">The variable whose contents to use as the input.</param>
        /// <param name="title"><c>true</c> to use title casing, <c>false</c> otherwise.</param>
        public static void StringLower(out string output, ref string input, bool title)
        {
            output = title ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input) : input.ToLowerInvariant();
        }

        /// <summary>
        /// Replaces the specified substring with a new string.
        /// </summary>
        /// <param name="output">The variable to store the result.</param>
        /// <param name="input">The variable whose contents to use as the input.</param>
        /// <param name="search">The substring to search for.</param>
        /// <param name="replace">The string to replace <paramref name="search"/>.</param>
        /// <param name="all"><c>true</c> to replace all instances, <c>false</c> otherwise.</param>
        /// <remarks>If <paramref name="all"/> is true and <see cref="A_StringCaseSense"/> is on a faster replacement algorithm is used.</remarks>
        public static void StringReplace(out string output, ref string input, string search, string replace, bool all)
        {
            if (IsAnyBlank(input, search, replace))
            {
                output = string.Empty;
                return;
            }

            var compare = _StringComparison ?? StringComparison.OrdinalIgnoreCase;

            if (all && compare == StringComparison.Ordinal)
                output = input.Replace(search, replace);
            else
            {
                var buf = new StringBuilder(input.Length);
                int z = 0, n = 0, l = search.Length;

                while (z < input.Length && (z = input.IndexOf(search, z, compare)) != -1)
                {
                    if (n < z)
                        buf.Append(input, n, z - n);
                    buf.Append(replace);
                    z += l;
                    n = z;
                }

                if (n < input.Length)
                    buf.Append(input, n, input.Length - n);

                output = buf.ToString();
            }
        }

        /// <summary>
        /// Separates a string into an array of substrings using the specified delimiters.
        /// </summary>
        /// <param name="output">The variable to store the result.</param>
        /// <param name="input">The variable whose contents to use as the input.</param>
        /// <param name="delimiters">One or more characters (case sensitive), each of which is used to determine
        /// where the boundaries between substrings occur in <paramref name="input"/>.
        /// If this is blank each character of <paramref name="input"/> will be treated as a substring.</param>
        /// <param name="trim">An optional list of characters (case sensitive) to exclude from the beginning and end of each array element.</param>
        public static void StringSplit(out string[] output, ref string input, string delimiters, string trim)
        {
            if (delimiters.Length == 0)
            {
                var list = new List<string>(input.Length);
                foreach (var letter in input)
                    if (trim.IndexOf(letter) == -1)
                        list.Add(letter.ToString());
                output = list.ToArray();
                return;
            }

            output = input.Split(delimiters.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (trim.Length != 0)
            {
                char[] omit = trim.ToCharArray();
                for (int i = 0; i < output.Length; i++)
                    output[i] = output[i].Trim(omit);
            }
        }

        /// <summary>
        /// Converts a string to uppercase.
        /// </summary>
        /// <param name="output">The variable to store the result.</param>
        /// <param name="input">The variable whose contents to use as the input.</param>
        /// <param name="title"><c>true</c> to use title casing, <c>false</c> otherwise.</param>
        public static void StringUpper(out string output, ref string input, bool title)
        {
            output = title ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input) : input.ToUpperInvariant();
        }

        /// <summary>
        /// Retrieves one or more characters from the specified position in a string.
        /// </summary>
        /// <param name="input">The string to use.</param>
        /// <param name="index">The one-based starting character position.
        /// If this is less than one it is considered an offset from the end of the string.</param>
        /// <param name="length">The maximum number of characters to retrieve.
        /// Leave this parameter blank to return the entire leading part of the string.
        /// Specify a negative value to omit that many characters from the end of the string.</param>
        /// <returns>The new substring.</returns>
        public static string SubStr(string input, int index, int length = int.MaxValue)
        {
            if (string.IsNullOrEmpty(input) || length == 0)
                return string.Empty;

            if (index < 1)
                index += input.Length;

            index--;

            if (index < 0 || index >= input.Length)
                return string.Empty;

            int d = input.Length - index;

            if (length < 0)
                length += d;

            length = Math.Min(length, d);
            return input.Substring(index, length);
        }

        /// <summary>
        /// Performs miscellaneous math functions, bitwise operations, and tasks such as ASCII to Unicode conversion.
        /// This function is obsolete, please use the related newer syntax.
        /// <seealso cref="Asc"/>
        /// <seealso cref="Chr"/>
        /// <seealso cref="Mod"/>
        /// <seealso cref="Exp"/>
        /// <seealso cref="Sqrt"/>
        /// <seealso cref="Log"/>
        /// <seealso cref="Ln"/>
        /// <seealso cref="Round"/>
        /// <seealso cref="Ceil"/>
        /// <seealso cref="Floor"/>
        /// <seealso cref="Abs"/>
        /// <seealso cref="Sin"/>
        /// <seealso cref="Cos"/>
        /// <seealso cref="Tan"/>
        /// <seealso cref="ASin"/>
        /// <seealso cref="ACos"/>
        /// <seealso cref="ATan"/>
        /// <seealso cref="Floor"/>
        /// <seealso cref="Floor"/>
        /// </summary>
        [Obsolete]
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
