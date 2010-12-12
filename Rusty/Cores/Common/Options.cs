using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;

namespace IronAHK.Rusty
{
    partial class Core
    {
        static bool? OnOff(string mode)
        {
            switch (mode.ToLowerInvariant())
            {
                case Keyword_On:
                case "1":
                    return true;

                case Keyword_Off:
                case "0":
                    return false;

                default:
                    return null;
            }
        }

        static bool IsAnyBlank(params string[] args)
        {
            foreach (var str in args)
                if (string.IsNullOrEmpty(str))
                    return true;

            return false;
        }

        static Dictionary<char, string> KeyValues(string Options, bool Lowercase, char[] Exceptions)
        {
            var table = new Dictionary<char, string>();
            var buf = new StringBuilder();
            int i = 0;
            bool exp = false;
            char key = default(char);
            string val;

            if (Lowercase)
                for (i = 0; i < Exceptions.Length; i++)
                    Exceptions[i] = char.ToLowerInvariant(Exceptions[i]);
            i = 0;

            while (i < Options.Length)
            {
                char sym = Options[i];
                i++;
                if (char.IsWhiteSpace(sym) || i == Options.Length)
                {
                    if (buf.Length == 0)
                        continue;

                    if (exp)
                    {
                        exp = false;
                        val = buf.ToString();
                        buf.Length = 0;
                        if (table.ContainsKey(key))
                            table[key] = val;
                        else
                            table.Add(key, val);
                        continue;
                    }

                    key = buf[0];
                    if (Lowercase)
                        key = char.ToLowerInvariant(key);

                    foreach (var ex in Exceptions)
                        if (key == ex)
                        {
                            exp = true;
                            buf.Length = 0;
                            continue;
                        }

                    val = buf.Length > 1 ? buf.Remove(0, 1).ToString() : string.Empty;

                    if (table.ContainsKey(key))
                        table[key] = val;
                    else
                        table.Add(key, val);

                    buf.Length = 0;
                }
                else
                    buf.Append(sym);
            }

            if (exp && key != default(char))
            {
                if (table.ContainsKey(key))
                    table[key] = null;
                else
                    table.Add(key, null);
            }

            return table;
        }

        static List<string> ParseFlags(ref string arg)
        {
            var list = new List<string>();
            const char flag = '*';
            var i = -1;

            foreach (var sym in arg)
            {
                i++;

                if (Array.IndexOf(Keyword_Spaces, sym) != -1)
                    continue;

                if (sym != flag)
                    break;

                var z = i;

                for (; i < arg.Length; i++)
                    if (char.IsWhiteSpace(arg, i) || arg[i] == flag)
                        break;

                if (z == i)
                    continue;

                var item = arg.Substring(z, i - z);
                list.Add(item);
            }

            arg = arg.Substring(i);
            return list;
        }

        static string[] ParseOptions(string options)
        {
            return options.Split(Keyword_Spaces, StringSplitOptions.RemoveEmptyEntries);
        }

        static bool IsOption(string options, string search)
        {
            if (string.IsNullOrEmpty(options) || string.IsNullOrEmpty(search))
                return false;

            return options.Trim().Equals(search, StringComparison.OrdinalIgnoreCase);
        }

        static bool OptionContains(string options, params string[] keys)
        {
            foreach (string key in keys)
                if (!OptionContains(options, key))
                    return false;

            return true;
        }

        static bool OptionContains(string options, string key, bool casesensitive = false)
        {
            // TODO: test OptionContains method
            var comp = casesensitive ? StringComparison.CurrentCulture : StringComparison.OrdinalIgnoreCase;
            var i = 0;

            while (i < options.Length)
            {
                var z = options.IndexOf(key, i, comp);

                var p = z == 0 || !char.IsLetter(options, z - 1);
                z += key.Length;

                if (!p)
                    continue;

                p = z == options.Length || !char.IsLetter(options, z + 1);

                if (!p)
                    continue;

                return true;
            }

            return false;
        }

        static Dictionary<string, string> ParseOptionsRegex(ref string options, Dictionary<string, Regex> items, bool remove = true) {
            var results = new Dictionary<string, string>();

            foreach(var item in items) {
                if(item.Value.IsMatch(options)) {
                    var match = item.Value.Match(options).Groups[1].Captures[0];
                    results.Add(item.Key, match.Value);

                    if(remove)
                        options = options.Substring(0, match.Index) + options.Substring(match.Index + match.Length);
                } else {
                    results.Add(item.Key, "");
                }
            }
            return results;
        }

        /// <summary>
        /// Parse a string and get Coordinates
        /// </summary>
        /// <param name="input">String in Format X123 Y123</param>
        /// <param name="p">out Point Struct if possible</param>
        /// <returns>true if parsing succesful</returns>
        private static bool TryParseCoordinate(string input, out Point p) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Merges two Dictionarys in generic way
        /// </summary>
        /// <typeparam name="T">any</typeparam>
        /// <typeparam name="T2">any</typeparam>
        /// <param name="dict1">Dictionary 1</param>
        /// <param name="dict2">Dictionary 2</param>
        /// <returns>Merged Dictionary</returns>
        static Dictionary<T, T2> MergeDictionarys<T, T2>(Dictionary<T, T2> dict1, Dictionary<T, T2> dict2) {

            var merged = new Dictionary<T, T2>();

            foreach(var key in dict1.Keys) {
                merged.Add(key, dict1[key]);
            }

            foreach(var key in dict2.Keys) {
                if(!merged.ContainsKey(key))
                    merged.Add(key, dict2[key]);
            }
            return merged;
        }
    }
}
