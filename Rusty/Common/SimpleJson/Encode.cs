using System;
using System.Collections.Generic;
using System.Text;

namespace IronAHK.Rusty.Common
{
    partial class SimpleJson
    {
        /// <summary>
        /// Format a dictionary of string key and object value pairs as a JSON string.
        /// </summary>
        /// <param name="Elements">The table of key and values. Objects other than a string, boolean or numeric type have their <code>ToString()</code> method called for a compatible value.</param>
        /// <returns>A JSON representation.</returns>
        public static string Encode(Dictionary<string, object> Elements)
        {
            return EncodeObject(Elements);
        }

        static string EncodeObject(object node)
        {
            if (node == null)
                return Null;

            var json = new StringBuilder();

            if (node is Dictionary<string, object>)
            {
                var pairs = (Dictionary<string, object>)node;
                json.Append(ObjectOpen);
                int n = pairs.Keys.Count;
                foreach (var key in pairs.Keys)
                {
                    json.Append(Space);
                    json.Append(StringBoundary);
                    json.Append(key);
                    json.Append(StringBoundary);
                    json.Append(Space);
                    json.Append(MemberAssign);
                    json.Append(Space);
                    json.Append(EncodeObject(pairs[key]));
                    n--;
                    json.Append(n == 0 ? Space : MemberSeperator);
                }
                json.Append(ObjectClose);
            }
            else if (node is object[])
            {
                var list = (object[])node;
                json.Append(ArrayOpen);
                int n = list.Length;
                foreach (var sub in list)
                {
                    json.Append(Space);
                    json.Append(EncodeObject(sub));
                    n--;
                    json.Append(n == 0 ? Space : MemberSeperator);
                }
                json.Append(ArrayClose);
            }
            else if (node is bool)
                json.Append((bool)node ? True : False);
            else if (node is byte || node is sbyte || node is short || node is ushort || node is int || node is uint || node is long || node is ulong || node is float || node is double || node is decimal)
                json.Append(node.ToString());
            else
            {
                json.Append(StringBoundary);
                if (node is string)
                    json.Append((string)node);
                else
                    json.Append(node.ToString());
                json.Append(StringBoundary);
            }

            return json.ToString();
        }
    }
}
