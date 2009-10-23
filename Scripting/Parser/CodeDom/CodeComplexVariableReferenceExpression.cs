using System;
using System.CodeDom;

namespace IronAHK.Scripting
{
    class CodeComplexVariableReferenceExpression : CodeExpression
    {
        CodeExpression[] parts;

        public CodeComplexVariableReferenceExpression(params CodeExpression[] parts)
        {
            foreach (CodeExpression part in parts)
            {
                if (!(part is CodePrimitiveExpression || part is CodeComplexVariableReferenceExpression))
                    throw new ArgumentException();
            }

            this.parts = parts;
        }

        public CodeExpression[] Parts
        {
            get { return parts; }
        }

        public static explicit operator CodeMethodInvokeExpression(CodeComplexVariableReferenceExpression variable)
        {
            var concat = new CodeMethodInvokeExpression();
            concat.Method = new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(string)), "Concat", new CodeTypeReference(typeof(object[])));

            foreach (CodeExpression part in variable.Parts)
            {
                if (part is CodePrimitiveExpression)
                    concat.Parameters.Add(part);
                else if (part is CodeComplexVariableReferenceExpression)
                    concat.Parameters.Add((CodeMethodInvokeExpression)part);
            }

            return new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(Rusty.Core)), "GetEnv"), concat);
        }
    }
}
