using System;
using System.CodeDom;
using System.Collections.Generic;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        List<CodeMethodInvokeExpression> invokes = new List<CodeMethodInvokeExpression>();

        #region DOM

        CodeMemberMethod LocalMethod(string name)
        {
            var method = new CodeMemberMethod() { Name = name, ReturnType = new CodeTypeReference(typeof(object)) };
            var param = new CodeParameterDeclarationExpression(typeof(object[]), args);
            param.UserData.Add("rawtype", typeof(object[]));
            method.Parameters.Add(param);
            return method;
        }

        CodeMethodInvokeExpression LocalLabelInvoke(string name)
        {
            var invoke = (CodeMethodInvokeExpression)InternalMethods.LabelCall;
            invoke.Parameters.Add(new CodePrimitiveExpression(name));
            return invoke;
        }

        CodeMethodInvokeExpression LocalMethodInvoke(string name)
        {
            var invoke = new CodeMethodInvokeExpression();
            invoke.Method.MethodName = name;
            invoke.Method.TargetObject = null;
            return invoke;
        }

        #endregion

        #region Resolve

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

        #endregion
    }
}
