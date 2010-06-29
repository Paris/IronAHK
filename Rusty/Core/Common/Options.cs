using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
