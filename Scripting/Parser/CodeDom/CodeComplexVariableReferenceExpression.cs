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

        public CodeMethodInvokeExpression QualifiedName
        {
            get
            {
                var concat = new CodeMethodInvokeExpression();
                concat.Method = new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(string)), "Concat", new CodeTypeReference(typeof(string[])));

                CodeExpression[] sub = new CodeExpression[parts.Length];

                for (int i = 0; i < parts.Length; i++)
                {
                    var part = parts[i];
                    if (part is CodePrimitiveExpression)
                        sub[i] = (CodePrimitiveExpression)part;
                    else if (part is CodeComplexVariableReferenceExpression)
                        sub[i] = (CodeMethodInvokeExpression)(CodeComplexVariableReferenceExpression)part;
                }

                concat.Parameters.Add(new CodeArrayCreateExpression(new CodeTypeReference(typeof(string[])), sub));
                return concat;
            }
        }

        public static explicit operator CodeMethodInvokeExpression(CodeComplexVariableReferenceExpression variable)
        {
            var get = new CodeMethodInvokeExpression();
            get.Method = new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(Rusty.Core)), "GetEnv");
            get.Parameters.Add(variable.QualifiedName);
            return get;
        }
    }
}
