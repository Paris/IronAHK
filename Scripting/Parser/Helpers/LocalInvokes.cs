using System;
using System.CodeDom;
using System.Collections.Generic;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        List<CodeMethodInvokeExpression> invokes = new List<CodeMethodInvokeExpression>();

        void ResolveLocalInvokes()
        {
            foreach (var invoke in invokes)
            {
                string name = LocalMethodName(invoke.Method.MethodName);

                if (name == null)
                    continue;

                invoke.Method.MethodName = name;

                var obj = new CodeArrayCreateExpression();
                obj.Size = invoke.Parameters.Count;
                obj.CreateType = new CodeTypeReference(typeof(object));
                obj.Initializers.AddRange(invoke.Parameters);
                invoke.Parameters.Clear();
                invoke.Parameters.Add(obj);
            }

            invokes.Clear();
        }

        string LocalMethodName(string name)
        {
            foreach (var method in methods)
                if (method.Key.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return method.Key;
            return null;
        }
    }
}
