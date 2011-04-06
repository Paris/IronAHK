using System;
using System.Collections.Generic;
using System.Text;

namespace IronAHK.Rusty.Common
{
    partial class SimpleJson
    {
        /// <summary>
        /// Convert a JSON string to a dictionary of string key and object value pairs.
        /// </summary>
        /// <param name="Source">The JSON string to evaluate.</param>
        /// <returns>A <see cref="System.Collections.Generic.Dictionary&lt;TKey, TValue&gt;"/>.</returns>
        public static Dictionary<string, object> Decode(string Source)
        {
            var data = new Dictionary<string, object>();
            int pointer = 0;
            DecodeObject(ref data, Scan(Source, ref pointer, ObjectClose));
            return data;
        }

        static string Scan(string node, ref int i, char anchor)
        {
            int start = i + 1, skip = 1;
            bool inStr = false;

            while (++i < node.Length)
            {
                char token = node[i];
                if ((token == StringBoundary || token == StringBoundaryAlt) && node[i - 1] != Escape)
                    inStr = !inStr;

                if ((anchor == ArrayClose && token == ArrayOpen) || (anchor == ObjectClose && token == ObjectOpen))
                    skip++;
                else if ((anchor == StringBoundary || anchor == StringBoundaryAlt) && token == anchor)
                    break;
                else if (!inStr && token == anchor)
                    if (--skip == 0)
                        break;

                if (i == node.Length)
                    throw new Exception(ErrorMessage(ExUntermField, i));
            }

            return node.Substring(start, i - start);
        }

        static void Value(ref Dictionary<string, object> parent, ref string key)
        {
            object value = null;
            Value(ref parent, ref key, ref value);
        }

        static void Value(ref Dictionary<string, object> parent, ref string key, ref object value)
        {
            if (key.Length == 0)
                return;
            if (parent.ContainsKey(key))
                parent[key] = value;
            else parent.Add(key, value);
            key = string.Empty;
            value = null;
        }

        static bool IsNumber(char c)
        {
            return c == '+' || c == '-' || c == '.' || c == 'e' || c == 'E' || (c >= '0' && c <= '9');
        }

        static bool ExtractNumber(ref string node, ref int i, out object value)
        {
            var s = new StringBuilder();
            while (i < node.Length)
            {
                char c = node[i];
                if (IsNumber(c))
                    s.Append(c);
                else
                    break;
                i++;
            }
            value = null;
            double n;
            if (double.TryParse(s.ToString(), out n))
            {
                value = (((int)n)) == n ? (int)n : n;
                return true;
            }
            return false;
        }

        static bool ExtractBoolean(ref string node, ref int i, ref object value)
        {
            int r = node.Length - i + 1;

            if (r > Null.Length && string.Equals(node.Substring(i, Null.Length), Null, StringComparison.OrdinalIgnoreCase))
            {
                i += Null.Length;
                value = null;
                return true;
            }
            else if (r > True.Length && string.Equals(node.Substring(i, True.Length), True, StringComparison.OrdinalIgnoreCase))
            {
                i += True.Length;
                value = true;
                return true;
            }
            else if (r > False.Length && string.Equals(node.Substring(i, False.Length), False, StringComparison.OrdinalIgnoreCase))
            {
                i += False.Length;
                value = false;
                return true;
            }

            return false;
        }

        static object[] ParseArray(string node)
        {
            var list = new List<object>();
            object value = null;

            for (int i = 0; i < node.Length; i++)
            {
                char token = node[i];

                if (char.IsWhiteSpace(node, i))
                    continue;

                switch (token)
                {
                    case StringBoundary:
                        value = Scan(node, ref i, StringBoundary);
                        break;

                    case StringBoundaryAlt:
                        value = Scan(node, ref i, StringBoundaryAlt);
                        break;

                    case ObjectOpen:
                        var sub = new Dictionary<string, object>();
                        DecodeObject(ref sub, Scan(node, ref i, ObjectClose));
                        value = sub;
                        break;

                    case ArrayOpen:
                        value = ParseArray(Scan(node, ref i, ArrayClose));
                        break;

                    case MemberSeperator:
                        list.Add(value);
                        value = null;
                        break;

                    default:
                        if (IsNumber(token))
                            ExtractNumber(ref node, ref i, out value);
                        else if (!ExtractBoolean(ref node, ref i, ref value))
                            throw new Exception(ErrorMessage(ExNoMemberVal, i));
                        break;
                }
            }

            if (node.Length != 0)
                list.Add(value);

            return list.ToArray();
        }

        static void DecodeObject(ref Dictionary<string, object> parent, string node)
        {
            string key = string.Empty;
            bool expectVal = false, next = true;

            for (int i = 0; i < node.Length; i++)
            {
                char token = node[i];

                if (char.IsWhiteSpace(token))
                    continue;
                else if (expectVal)
                {
                    object value = null;

                    switch (token)
                    {
                        case StringBoundary:
                            value = Scan(node, ref i, StringBoundary);
                            break;

                        case StringBoundaryAlt:
                            value = Scan(node, ref i, StringBoundaryAlt);
                            break;

                        case ObjectOpen:
                            var sub = new Dictionary<string, object>();
                            DecodeObject(ref sub, Scan(node, ref i, ObjectClose));
                            value = sub;
                            break;

                        case ArrayOpen:
                            string s = Scan(node, ref i, ArrayClose);
                            value = ParseArray(s);
                            break;

                        case MemberSeperator:
                            value = null;
                            next = true;
                            break;

                        default:
                            if (IsNumber(token))
                                ExtractNumber(ref node, ref i, out value);
                            else if (!ExtractBoolean(ref node, ref i, ref value))
                                throw new Exception(ErrorMessage(ExNoMemberVal, i));
                            break;
                    }

                    Value(ref parent, ref key, ref value);
                    expectVal = false;
                }
                else if (next)
                {
                    next = false;
                    if (token == StringBoundary)
                        key = Scan(node, ref i, StringBoundary);
                    else if (token == StringBoundaryAlt)
                        key = Scan(node, ref i, StringBoundaryAlt);
                    else
                    {
                        var keyip = new StringBuilder();
                        do
                        {
                            char c = node[i];
                            if (char.IsLetterOrDigit(c) || c == '_')
                                keyip.Append(c);
                            else
                                break;
                            i++;
                        }
                        while (i < node.Length);
                        if (keyip.Length == 0)
                            throw new Exception(ErrorMessage(ExNoKeyPair, i));
                        else
                        {
                            key = keyip.ToString();
                            i--;
                        }
                    }
                }
                else if (token == MemberAssign || token == MemberAssignAlt)
                    expectVal = true;
                else if (token == MemberSeperator)
                {
                    Value(ref parent, ref key);
                    next = true;
                }
                else
                    throw new Exception(ErrorMessage(ExUnexpectedToken, i));
            }

            Value(ref parent, ref key);
        }
    }
}
