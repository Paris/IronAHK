using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace IronAHK.Rusty
{
    partial class Core
    {
        // TODO: organise String.cs

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
        /// Returns the position of the first occurrence of the string Needle in the string Haystack. Unlike StringGetPos, position 1 is the first character; this is because 0 is synonymous with "false", making it an intuitive "not found" indicator. If the parameter CaseSensitive is omitted or false, the search is not case sensitive (the method of insensitivity depends on StringCaseSense); otherwise, the case must match exactly. If StartingPos is omitted, it defaults to 1 (the beginning of Haystack). Otherwise, specify 2 to start at Haystack's second character, 3 to start at the third, etc. If StartingPos is beyond the length of Haystack, 0 is returned. If StartingPos is 0, the search is conducted in reverse (right-to-left) so that the rightmost match is found. Regardless of the value of StartingPos, the returned position is always relative to the first character of Haystack. For example, the position of "abc" in "123abc789" is always 4. Related items: RegExMatch(), IfInString, and StringGetPos.
        /// </summary>
        /// <param name="Haystack"></param>
        /// <param name="Needle"></param>
        /// <param name="CaseSensitive"></param>
        /// <param name="StartingPos"></param>
        /// <returns></returns>
        public static int InStr(string Haystack, string Needle, string CaseSensitive, int StartingPos)
        {
            StringComparison type = CaseSensitive == null ?
                StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            return StartingPos == 0 ? Haystack.LastIndexOf(Needle, 0, type) :
                Haystack.IndexOf(Needle, StartingPos, type);
        }

        /// <summary>
        /// Determines whether a string contains a pattern (regular expression).
        /// </summary>
        /// <param name="Haystack">The string whose content is searched.</param>
        /// <param name="NeedleRegEx">The pattern to search for, which is a Perl-compatible regular expression (PCRE). The pattern's options (if any) must be included at the beginning of the string followed by an close-parenthesis. For example, the pattern "i)abc.*123" would turn on the case-insensitive option and search for "abc", followed by zero or more occurrences of any character, followed by "123". If there are no options, the ")" is optional; for example, ")abc" is equivalent to "abc".</param>
        /// <param name="UnquotedOutputVar">
        /// <para>Mode 1 (default): OutputVar is the unquoted name of a variable in which to store the part of Haystack that matched the entire pattern. If the pattern is not found (that is, if the function returns 0), this variable and all array elements below are made blank.</para>
        /// <para>If any capturing subpatterns are present inside NeedleRegEx, their matches are stored in an array whose base name is OutputVar. For example, if the variable's name is Match, the substring that matches the first subpattern would be stored in Match1, the second would be stored in Match2, and so on. The exception to this is named subpatterns: they are stored by name instead of number. For example, the substring that matches the named subpattern (?P&lt;Year&gt;\d{4}) would be stored in MatchYear. If a particular subpattern does not match anything (or if the function returns zero), the corresponding variable is made blank.</para>
        /// <para>Within a function, to create an array that is global instead of local, declare the base name of the array (e.g. Match) as a global variable prior to using it. The converse is true for assume-global functions.</para>
        /// <para>Mode 2 (position-and-length): If a capital P is present in the RegEx's options -- such as "P)abc.*123" -- the length of the entire-pattern match is stored in OutputVar (or 0 if no match). If any capturing subpatterns are present, their positions and lengths are stored in two arrays: OutputVarPos and OutputVarLen. For example, if the variable's base name is Match, the one-based position of the first subpattern's match would be stored in MatchPos1, and its length in MatchLen1 (zero is stored in both if the subpattern was not matched or the function returns 0). The exception to this is named subpatterns: they are stored by name instead of number (e.g. MatchPosYear and MatchLenYear).</para>
        /// </param>
        /// <param name="StartingPos">
        /// <para>If StartingPosition is omitted, it defaults to 1 (the beginning of Haystack). Otherwise, specify 2 to start at the second character, 3 to start at the third, and so on. If StartingPosition is beyond the length of Haystack, the search starts at the empty string that lies at the end of Haystack (which typically results in no match).</para>
        /// <para>If StartingPosition is less than 1, it is considered to be an offset from the end of Haystack. For example, 0 starts at the last character and -1 starts at the next-to-last character. If StartingPosition tries to go beyond the left end of Haystack, all of Haystack is searched.</para>
        /// <para>Regardless of the value of StartingPosition, the return value is always relative to the first character of Haystack. For example, the position of "abc" in "123abc789" is always 4.</para>
        /// </param>
        /// <returns>RegExMatch() returns the position of the leftmost occurrence of NeedleRegEx in the string Haystack. Position 1 is the first character. Zero is returned if the pattern is not found. If an error occurs (such as a syntax error inside NeedleRegEx), an empty string is returned and ErrorLevel is set to one of the values below instead of 0.</returns>
        public static int RegExMatch(string Haystack, string NeedleRegEx, out string[] UnquotedOutputVar, int StartingPos)
        {
            Regex exp;
            try { exp = ParseRegEx(NeedleRegEx); }
            catch (ArgumentException)
            {
                UnquotedOutputVar = new string[] { };
                error = 2;
                return 0;
            }

            Match res = exp.Match(Haystack, StartingPos);

            string[] matches = new string[res.Groups.Count];
            for (int i = 0; i < res.Groups.Count; i++)
                matches[i] = res.Groups[i].Value;

            UnquotedOutputVar = matches;

            return 0;
        }

        /// <summary>
        /// Replaces occurrences of a pattern (regular expression) inside a string.
        /// </summary>
        /// <param name="Haystack">The string whose content is searched and replaced.</param>
        /// <param name="NeedleRegEx">The pattern to search for, which is a Perl-compatible regular expression (PCRE). The pattern's options (if any) must be included at the beginning of the string followed by an close-parenthesis. For example, the pattern "i)abc.*123" would turn on the case-insensitive option and search for "abc", followed by zero or more occurrences of any character, followed by "123". If there are no options, the ")" is optional; for example, ")abc" is equivalent to "abc".</param>
        /// <param name="Replacement">
        /// <para>The string to be substituted for each match, which is plain text (not a regular expression). It may include backreferences like $1, which brings in the substring from Haystack that matched the first subpattern. The simplest backreferences are $0 through $9, where $0 is the substring that matched the entire pattern, $1 is the substring that matched the first subpattern, $2 is the second, and so on. For backreferences above 9 (and optionally those below 9), enclose the number in braces; e.g. ${10}, ${11}, and so on. For named subpatterns, enclose the name in braces; e.g. ${SubpatternName}. To specify a literal $, use $$ (this is the only character that needs such special treatment; backslashes are never needed to escape anything).</para>
        /// <para>To convert the case of a subpattern, follow the $ with one of the following characters: U or u (uppercase), L or l (lowercase), T or t (title case, in which the first letter of each word is capitalized but all others are made lowercase). For example, both $U1 and $U{1} transcribe an uppercase version of the first subpattern.</para>
        /// <para>Nonexistent backreferences and those that did not match anything in Haystack -- such as one of the subpatterns in (abc)|(xyz) -- are transcribed as empty strings.</para>
        /// </param>
        /// <param name="OutputVarCount">The unquoted name of a variable in which to store the number of replacements that occurred (0 if none).</param>
        /// <param name="Limit">If Limit is omitted, it defaults to -1, which replaces all occurrences of the pattern found in Haystack. Otherwise, specify the maximum number of replacements to allow. The part of Haystack to the right of the last replacement is left unchanged.</param>
        /// <param name="StartingPos">
        /// <para>If StartingPosition is omitted, it defaults to 1 (the beginning of Haystack). Otherwise, specify 2 to start at the second character, 3 to start at the third, and so on. If StartingPosition is beyond the length of Haystack, the search starts at the empty string that lies at the end of Haystack (which typically results in no replacements).</para>
        /// <para>If StartingPosition is less than 1, it is considered to be an offset from the end of Haystack. For example, 0 starts at the last character and -1 starts at the next-to-last character. If StartingPosition tries to go beyond the left end of Haystack, all of Haystack is searched.</para>
        /// <para>Regardless of the value of StartingPosition, the return value is always a complete copy of Haystack -- the only difference is that more of its left side might be unaltered compared to what would have happened with a StartingPosition of 1.</para>
        /// </param>
        /// <returns>RegExReplace() returns a version of Haystack whose contents have been replaced by the operation. If no replacements are needed, Haystack is returned unaltered. If an error occurs (such as a syntax error inside NeedleRegEx), Haystack is returned unaltered (except in versions prior to 1.0.46.06, which return "") and ErrorLevel is set to one of the values below instead of 0.</returns>
        public static string RegExReplace(string Haystack, string NeedleRegEx, string Replacement, out int OutputVarCount, int Limit, int StartingPos)
        {
            Regex exp;
            try { exp = ParseRegEx(NeedleRegEx); }
            catch (ArgumentException)
            {
                OutputVarCount = 0;
                error = 2;
                return null;
            }
            int total = exp.Matches(Haystack, StartingPos).Count;
            OutputVarCount = Math.Min(Limit, total);
            return exp.Replace(Haystack, Replacement, Limit, StartingPos);
        }

        /// <summary>
        /// Arranges a variable's contents in alphabetical, numerical, or random order (optionally removing duplicates).
        /// </summary>
        /// <param name="VarName">The name of the variable whose contents will be sorted.</param>
        /// <param name="Options">See list below.</param>
        public static void Sort(ref string VarName, params string[] Options)
        {
            var opts = KeyValues(string.Join(",", Options), true, new char[] { 'f' });
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
        /// Returns the length of String. If String is a variable to which ClipboardAll was previously assigned, its total size is returned. Corresponding command: StringLen.
        /// </summary>
        /// <param name="String"></param>
        /// <returns></returns>
        public static decimal StrLen(string String)
        {
            return (decimal)String.Length;
        }

        /// <summary>
        /// Retrieves the position of the specified substring within a string.
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the retrieved position relative to the first character of <paramref name="InputVar"/>. Position 0 is the first character.</param>
        /// <param name="InputVar">The name of the input variable, whose contents will be searched.</param>
        /// <param name="SearchText">The string to search for. Matching is not case sensitive unless <see cref="A_StringCaseSense"/> has been turned on.</param>
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
        /// Matching is not case sensitive unless <see cref="A_StringCaseSense"/> has been turned on.</param>
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

        /// <summary>
        /// Copies a substring from String starting at StartingPos and proceeding rightward to include at most Length characters (if Length is omitted, it defaults to "all characters"). For StartingPos, specify 1 to start at the first character, 2 to start at the second, and so on (if StartingPos is beyond String's length, an empty string is returned). If StartingPos is less than 1, it is considered to be an offset from the end of the string. For example, 0 extracts the last character and -1 extracts the two last characters (but if StartingPos tries to go beyond the left end of the string, the extraction starts at the first character). Length is the maximum number of characters to retrieve (fewer than the maximum are retrieved whenever the remaining part of the string too short). Specify a negative Length to omit that many characters from the end of the returned string (an empty string is returned if all or too many characters are omitted). Related items: RegExMatch(), StringMid, StringLeft/Right, StringTrimLeft/Right.
        /// </summary>
        /// <param name="String"></param>
        /// <param name="StartingPos"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static string SubStr(string String, decimal StartingPos, decimal Length)
        {
            return String.Substring((int)(StartingPos < 1 ? String.Length - StartingPos : StartingPos + 1), (int)Length);
        }
    }
}