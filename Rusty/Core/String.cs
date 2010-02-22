using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace IronAHK.Rusty
{
    partial class Core
    {
        /// <summary>
        /// Transforms a YYYYMMDDHH24MISS timestamp into the specified date/time format.
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the result.</param>
        /// <param name="TimeStamp">Leave this parameter blank to use the current local date and time.
        /// Otherwise, specify all or the leading part of a timestamp in the YYYYMMDDHH24MISS format.</param>
        /// <param name="Format">
        /// <para>If omitted, it defaults to the time followed by the long date,
        /// both of which will be formatted according to the current user's locale.</para>
        /// <para>Otherwise, specify one or more of the date-time formats,
        /// along with any literal spaces and punctuation in between.</para>
        /// </param>
        public static void FormatTime(out string OutputVar, string TimeStamp, string Format)
        {
            DateTime time;
            if (TimeStamp.Length == 0)
                time = DateTime.Now;
            else
            {
                try
                {
                    time = ToDateTime(TimeStamp);
                }
                catch (ArgumentOutOfRangeException)
                {
                    OutputVar = null;
                    return;
                }
            }

            switch (Format.ToLowerInvariant())
            {
                case "time":
                    Format = "t";
                    break;

                case "shortdate":
                    Format = "d";
                    break;

                case "longdate":
                    Format = "D";
                    break;

                case "yearmonth":
                    Format = "Y";
                    break;

                case "yday":
                    OutputVar = time.DayOfYear.ToString();
                    return;

                case "yday0":
                    OutputVar = time.DayOfYear.ToString().PadLeft(3, '0');
                    return;

                case "wday":
                    OutputVar = time.DayOfWeek.ToString();
                    return;

                case "yweek":
                    throw new NotSupportedException(); // HACK: get ISO 8601 full year and week number

                default:
                    if (Format.Length == 0)
                        Format = "f";
                    break;
            }

            try
            {
                OutputVar = time.ToString(Format);
            }
            catch (FormatException)
            {
                OutputVar = null;
            }
        }

        /// <summary>
        /// Arranges a variable's contents in alphabetical, numerical, or random order (optionally removing duplicates).
        /// </summary>
        /// <param name="VarName">The name of the variable whose contents will be sorted.</param>
        /// <param name="Options">See list below.</param>
        public static void Sort(ref string VarName, params string[] Options)
        {
            var opts = KeyValues(string.Join(",", Options), true, new char[] { 'f' });
            
            if (opts.ContainsKey('f'))
                throw new NotSupportedException(); // UNDONE: dynamic function calling for sort

            char split = '\n';

            if (opts.ContainsKey('d'))
            {
                string s = opts['d'];
                if (s.Length == 1)
                    split = s[0];
                opts.Remove('d');
            }

            string[] list = VarName.Split(new char[] { split }, StringSplitOptions.RemoveEmptyEntries);

            if (split == '\n')
            {
                for (int i = 0; i < list.Length; i++)
                {
                    int x = list[i].Length - 1;
                    if (list[i][x] == '\r')
                        list[i] = list[i].Substring(0, x);
                }
            }

            if (opts.ContainsKey('z') && VarName[VarName.Length - 1] == split)
            {
                Array.Resize<string>(ref list, list.Length + 1);
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
            if (opts.ContainsKey('r'))
            {
                if (opts['r'].ToLowerInvariant() == "andom") // Random
                    random = true;
                else
                    reverse = true;
                opts.Remove('r');
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
            var rand = new System.Random();

            Array.Sort(list, delegate(string x, string y)
            {
                if (x == y)
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
                error = 0;
                var ulist = new List<string>(list.Length);
                foreach (string item in list)
                    if (!ulist.Contains(item))
                        ulist.Add(item);
                    else
                        error++;
                list = ulist.ToArray();
            }

            if (reverse)
                Array.Reverse(list);

            VarName = string.Join(split.ToString(), list);
        }

        /// <summary>
        /// Retrieves the position of the specified substring within a string.
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the retrieved position relative to the first character of <paramref name="InputVar"/>. Position 0 is the first character.</param>
        /// <param name="InputVar">The name of the input variable, whose contents will be searched.</param>
        /// <param name="SearchText">The string to search for. Matching is not case sensitive unless <see cref="StringCaseSense"/> has been turned on.</param>
        /// <param name="Mode">
        /// <para>This affects which occurrence will be found if <paramref name="SearchText"/> occurs more than once within <paramref name="InputVar"/>. If this parameter is omitted, <paramref name="InputVar"/> will be searched starting from the left for the first match. If this parameter is 1 or the letter R, the search will start looking at the right side of <paramref name="InputVar"/> and will continue leftward until the first match is found.</para>
        /// <para>To find a match other than the first, specify the letter L or R followed by the number of the occurrence. For example, to find the fourth occurrence from the right, specify r4. Note: If the number is less than or equal to zero, no match will be found.</para>
        /// </param>
        /// <param name="Offset">The number of characters on the leftmost or rightmost side (depending on the parameter above) to skip over. If omitted, the default is 0.</param>
        [Obsolete, Conditional("LEGACY")]
        public static void StringGetPos(ref int OutputVar, string InputVar, string SearchText, string Mode, int Offset)
        {
            int skip = 0;
            bool reverse = false;

            if (Mode.Length != 0)
            {
                switch (Mode[0])
                {
                    case 'R':
                    case 'r':
                    case '1':
                        reverse = true;
                        break;

                    case 'L':
                    case 'l':
                        break;

                    default:
                        OutputVar = default(int);
                        error = 2;
                        return;
                }

                if (Mode.Length != 1)
                    if (!int.TryParse(Mode.Substring(1).Trim(), out skip))
                        skip = 0;
            }

            int len = SearchText.Length;

            do
            {
                // HACK: check method of string get pos works
                StringComparison comp = _StringCaseSense ?? StringComparison.OrdinalIgnoreCase;
                int count = len - Offset;
                OutputVar = reverse ? InputVar.LastIndexOf(SearchText, Offset, count, comp) : InputVar.IndexOf(SearchText, Offset, count, comp);
                Offset = reverse ? len - OutputVar - 1 : OutputVar + 1;
                skip--;
            }
            while (skip > 0);

            error = OutputVar == -1 ? 1 : 0;
        }

        /// <summary>
        /// Converts a string to lowercase.
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store newly converted string.</param>
        /// <param name="InputVar">The name of the variable whose contents will be read from.</param>
        /// <param name="Title">If this parameter is the letter T, the string will be converted to title case.</param>
        public static void StringLower(out string OutputVar, string InputVar, string Title)
        {
            OutputVar = Title.Length == 1 && Title.ToLowerInvariant()[0] == 't' ?
                CultureInfo.CurrentCulture.TextInfo.ToTitleCase(InputVar) : InputVar.ToUpperInvariant();
        }

        /// <summary>
        /// Replaces the specified substring with a new string.
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the result of the replacement process.</param>
        /// <param name="InputVar">The name of the variable whose contents will be read from.</param>
        /// <param name="SearchText">The string to search for.
        /// Matching is not case sensitive unless <see cref="StringCaseSense"/> has been turned on.</param>
        /// <param name="ReplaceText">SearchText will be replaced with this text.</param>
        /// <param name="All">
        /// <para>If omitted, only the first occurrence of <paramref name="SearchText"/> will be replaced.
        /// But if this parameter is 1, A, or All, all occurrences will be replaced.</para>
        /// <para>Specify the word UseErrorLevel to store in <see cref="ErrorLevel"/> the number of occurrences replaced (0 if none).
        /// UseErrorLevel implies "All".</para>
        /// </param>
        public static void StringReplace(out string OutputVar, string InputVar, string SearchText, string ReplaceText, string All)
        {
            bool all = false, getcount = false;

            switch (All.ToLowerInvariant())
            {
                case Keyword_UseErrorLevel:
                    getcount = true;
                    all = true;
                    break;

                case Keyword_All:
                case "a":
                case "1":
                    all = true;
                    break;
            }

            int count = 0;

            if (all && _StringCaseSense == StringComparison.Ordinal)
                OutputVar = InputVar.Replace(SearchText, ReplaceText); // optimised
            else
            {
                StringBuilder buf = new StringBuilder(InputVar.Length);
                int pos = 0, offset = 0;
                while ((pos = InputVar.IndexOf(SearchText, offset)) != -1)
                {
                    buf.Append(InputVar, offset, pos - offset);
                    buf.Append(ReplaceText);
                    pos += InputVar.Length;
                    offset = pos;
                    count++;
                }
                if (offset < InputVar.Length)
                    buf.Append(InputVar, offset, InputVar.Length - offset);
                OutputVar = pos == 0 && offset == 0 ? InputVar : buf.ToString();
            }

            error = getcount ? count : count == 0 ? 1 : 0;
        }

        /// <summary>
        /// Separates a string into an array of substrings using the specified delimiters.
        /// </summary>
        /// <param name="OutputArray">The name of the array in which to store each substring extracted from <paramref name="InputVar"/>.</param>
        /// <param name="InputVar">The name of the variable whose contents will be analysed.</param>
        /// <param name="Delimiters">
        /// <para>If this parameter is blank or omitted, each character of <paramref name="InputVar"/> will be treated as a separate substring.</para>
        /// <para>Otherwise, <paramref name="Delimiters"/> contains one or more characters (case sensitive),
        /// each of which is used to determine where the boundaries between substrings occur in <paramref name="InputVar"/>.
        /// Since the delimiter characters are not considered to be part of the substrings themselves, they are never copied into <paramref name="OutputArray"/>.
        /// Also, if there is nothing between a pair of delimiters within <paramref name="InputVar"/>, the corresponding array element will be blank.</para>
        /// </param>
        /// <param name="OmitChars">
        /// <para>An optional list of characters (case sensitive) to exclude from the beginning and end of each array element.</para>
        /// <para>If <paramref name="Delimiters"/> is blank, <paramref name="OmitChars"/> indicates which characters should be excluded from the array.</para>
        /// </param>
        public static void StringSplit(out string[] OutputArray, string InputVar, string Delimiters, string OmitChars)
        {
            if (Delimiters.Length == 0)
            {
                var output = new List<string>(InputVar.Length);
                foreach (char letter in InputVar)
                    if (OmitChars.IndexOf(letter) == -1)
                        output.Add(letter.ToString());
                OutputArray = output.ToArray();
                return;
            }

            OutputArray = InputVar.Split(Delimiters.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (OmitChars.Length != 0)
            {
                char[] omit = OmitChars.ToCharArray();
                for (int i = 0; i < OutputArray.Length; i++)
                    OutputArray[i] = OutputArray[i].Trim(omit);
            }
        }

        /// <summary>
        /// Converts a string to uppercase.
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store newly converted string.</param>
        /// <param name="InputVar">The name of the variable whose contents will be read from.</param>
        /// <param name="Title">If this parameter is the letter T, the string will be converted to title case.</param>
        public static void StringUpper(out string OutputVar, string InputVar, string Title)
        {
            OutputVar = Title.Length == 1 && Title.ToLowerInvariant()[0] == 't' ?
                CultureInfo.CurrentCulture.TextInfo.ToTitleCase(InputVar) : InputVar.ToUpperInvariant();
        }
    }
}