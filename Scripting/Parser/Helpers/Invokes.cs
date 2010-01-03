using System.CodeDom;

namespace IronAHK.Scripting
{
    partial class Parser
    {
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
    }
}
