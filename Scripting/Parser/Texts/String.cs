using System;
using System.IO;
using System.Text;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        string MultilineString(string code)
        {
            var reader = new StringReader(code);
            string line;

            line = reader.ReadLine().Trim(Spaces);

            if (line.Length < 1 || line[0] != ParenOpen)
                throw new ArgumentException();

            #region Scan options

            string join = "\n";
            bool ltrim = false, rtrim = true, stripComments = true, percentResolve = true, literalEscape = false;

            if (line.Length > 2)
            {
                if (line.Contains("%"))
                {
                    percentResolve = false;
                    line = line.Replace("%", string.Empty);
                }
                if (line.Contains("`"))
                {
                    literalEscape = true;
                    line = line.Replace("`", string.Empty);
                }
                if (line.Contains(","))
                    line = line.Replace(",", string.Empty);

                string[] options = line.Substring(1).Trim().Split(Spaces, StringSplitOptions.RemoveEmptyEntries);
                foreach (var option in options)
                {
                    switch (option.ToUpperInvariant())
                    {
                        case "LTRIM":
                            ltrim = true;
                            break;

                        case "RTRIM":
                            break;

                        case "RTRIM0":
                            rtrim = false;
                            break;

                        case "COMMENTS":
                        case "COMMENT":
                        case "COM":
                        case "C":
                            stripComments = false;
                            break;

                        case "JOIN":
                            join = string.Empty;
                            break;

                        default:
                            const string joinOpt = "join";
                            if (option.Length > joinOpt.Length && option.Substring(0, joinOpt.Length).Equals(joinOpt, StringComparison.OrdinalIgnoreCase))
                                join = option.Substring(joinOpt.Length).Replace("`s", " ");
                            else
                                throw new ParseException(ExMultiStr);
                            break;
                    }
                }
            }

            #endregion

            #region String

            var str = new StringBuilder(code.Length);
            string resolve = Resolve.ToString();
            string escape = Escape.ToString();
            string cast = Multicast.ToString();
            string resolveEscaped = string.Concat(escape, resolve);
            var escapeEscaped = new string(Escape, 2);
            string castEscaped = string.Concat(escape, cast);

            while ((line = reader.ReadLine()) != null)
            {
                string check = line.Trim();
                if (check.Length > 0 && check[0] == ParenClose)
                    break;

                if (ltrim && rtrim)
                    line = line.Trim(Spaces);
                else if (ltrim)
                    line = line.TrimStart(Spaces);
                else if (rtrim)
                    line = line.TrimEnd(Spaces);

                if (stripComments)
                    line = StripComment(line);

                if (!percentResolve)
                    line = line.Replace(resolve, resolveEscaped);

                if (literalEscape)
                    line = line.Replace(escape, escapeEscaped);

                line = line.Replace(cast, castEscaped);

                str.Append(line);
                str.Append(join);
            }

            if (str.Length == 0)
                return string.Empty;

            str.Remove(str.Length - join.Length, join.Length);

            #endregion

            return str.ToString();
        }

        string EscapedString(string code, bool resolve)
        {
            if (code.Length == 0)
                return string.Empty;

            var buffer = new StringBuilder(code.Length + 32);
            bool escaped = false;

            foreach (var sym in code)
            {
                if (escaped)
                {
                    switch (sym)
                    {
                        case 'n': buffer.Append('\n'); break;
                        case 'r': buffer.Append('\r'); break;
                        case 'b': buffer.Append('\b'); break;
                        case 't': buffer.Append('\t'); break;
                        case 'v': buffer.Append('\v'); break;
                        case 'a': buffer.Append('\a'); break;
                        case 'f': buffer.Append('\f'); break;

                        case '0': buffer.Append('\0'); break;

                        default:
                            if (sym == Resolve)
                                buffer.Append(Escape);
                            buffer.Append(sym);
                            break;
                    }

                    escaped = false;
                }
                else if (sym == Escape)
                    escaped = true;
                else
                    buffer.Append(sym);
            }

            return buffer.ToString();
        }

        string Replace(string input, string search, string replace)
        {
            var buf = new StringBuilder(input.Length);
            int z = 0, n = 0, l = search.Length;

            while (z < input.Length && (z = input.IndexOf(search, z, StringComparison.OrdinalIgnoreCase)) != -1)
            {
                if (n < z)
                    buf.Append(input, n, z - n);
                buf.Append(replace);
                z += l;
                n = z;
            }

            if (n < input.Length)
                buf.Append(input, n, input.Length - n);

            return buf.ToString();
        }
    }
}
