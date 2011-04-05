using System.CodeDom;
using System.Collections.Generic;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        void ParseObject(List<object> parts, out CodePrimitiveExpression[] keys, out CodeExpression[] values)
        {
            var names = new List<CodePrimitiveExpression>();
            var entries = new List<CodeExpression>();

            for (int i = 0; i < parts.Count; i++)
            {
                CodeExpression value = null;

                #region Name

                if (!(parts[i] is string))
                    throw new ParseException(ExUnexpected);

                var name = (string)parts[i];

                if (name.Length > 2 && name[0] == StringBound && name[name.Length - 1] == StringBound)
                    name = name.Substring(1, name.Length - 2);

                if (name.Length == 0 || !IsIdentifier(name))
                    throw new ParseException(ExInvalidVarName);

                i++;
                if (i == parts.Count)
                    goto collect;

                #endregion

                #region Assign

                if (!(parts[i] is string))
                    throw new ParseException(ExUnexpected);

                var assign = (string)parts[i];

                if (assign.Length == 1 && assign[0] == Multicast)
                    goto collect;

                if (!(assign.Length == 1 && (assign[0] == Equal || assign[0] == HotkeyBound)))
                    throw new ParseException(ExUnexpected);

                i++;
                if (i == parts.Count)
                    goto collect;

                #endregion

                #region Value

                var sub = new List<object>();
                int next = Set(parts, i);

                if (next == 0) // no enclosing set (...){...}[...] so scan until next bounary
                {
                    for (next = i; next < parts.Count; next++)
                    {
                        if (parts[next] is string && ((string)parts[next])[0] == Multicast)
                            break;
                    }
                }
                else
                    next++; // set function returns n-1 index

                for (; i < next; i++)
                    sub.Add(parts[i]);
                i--;

                value = ParseExpression(sub);

                i++;
                if (i == parts.Count)
                    goto collect;

                #endregion

                #region Delimiter

                if (!(parts[i] is string))
                    throw new ParseException(ExUnexpected);

                var delim = (string)parts[i];

                if (!(delim.Length == 1 && delim[0] == Multicast))
                    throw new ParseException(ExUnexpected);

                #endregion

                #region Collect

            collect:
                names.Add(new CodePrimitiveExpression(name));
                entries.Add(value ?? new CodePrimitiveExpression(null));

                #endregion
            }

            keys = names.ToArray();
            values = entries.ToArray();
        }

        bool IsJsonObject(object item)
        {
            return item is CodeMethodInvokeExpression && ((CodeMethodInvokeExpression)item).Method.MethodName == InternalMethods.Index.MethodName;
        }

        bool IsArrayExtension(object item)
        {
            return item is CodeMethodInvokeExpression && ((CodeMethodInvokeExpression)item).Method.MethodName == InternalMethods.ExtendArray.MethodName;
        }
    }
}
