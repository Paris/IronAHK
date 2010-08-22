using System;
using System.CodeDom;
using System.Collections.Generic;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        CodeExpression StringConcat(params CodeExpression[] parts)
        {
            var list = new List<CodeExpression>(parts.Length);

            foreach (var part in parts)
            {
                if (part is CodePrimitiveExpression)
                {
                    var value = ((CodePrimitiveExpression)part).Value;
                    if (value is string && string.IsNullOrEmpty((string)value))
                        continue;
                }

                list.Add(part);
            }

            if (list.Count == 1)
                return list[0];

            Type str = typeof(string);
            var method = (CodeMethodReferenceExpression)InternalMethods.Concat;
            var all = new CodeArrayCreateExpression(str, list.ToArray());
            return new CodeMethodInvokeExpression(method, all);
        }
    }
}
