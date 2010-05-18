using System;
using System.Collections.Generic;
using System.Text;

namespace IronAHK.Rusty
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
            Type type = node.GetType();

            if (type == typeof(Dictionary<string, object>))
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
            else if (type == typeof(object[]))
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
            else if (type == typeof(bool))
                json.Append((bool)node ? True : False);
            else if (type == typeof(byte) || type == typeof(sbyte) || type == typeof(short) || type == typeof(ushort) || type == typeof(int) || type == typeof(uint) || type == typeof(long) || type == typeof(ulong) || type == typeof(float) || type == typeof(double) || type == typeof(decimal))
                json.Append(node.ToString());
            else
            {
                json.Append(StringBoundary);
                if (type == typeof(string))
                    json.Append((string)node);
                else
                    json.Append(node.ToString());
                json.Append(StringBoundary);
            }

            return json.ToString();
        }
    }
}
