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
                if (!(part is CodePrimitiveExpression || part is CodeComplexVariableReferenceExpression || part is CodeMethodInvokeExpression))
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
                var concat = (CodeMethodInvokeExpression)Parser.InternalMethods.Concat;

                CodeExpression[] sub = new CodeExpression[parts.Length];

                for (int i = 0; i < parts.Length; i++)
                {
                    var part = parts[i];
                    if (part is CodePrimitiveExpression)
                        sub[i] = (CodePrimitiveExpression)part;
                    else if (part is CodeComplexVariableReferenceExpression)
                        sub[i] = (CodeMethodInvokeExpression)(CodeComplexVariableReferenceExpression)part;
                    else if (part is CodeMethodInvokeExpression)
                        sub[i] = (CodeMethodInvokeExpression)part;
                }

                concat.Parameters.Add(new CodeArrayCreateExpression(new CodeTypeReference(typeof(string[])), sub));
                return concat;
            }
        }

        public static explicit operator CodeMethodInvokeExpression(CodeComplexVariableReferenceExpression variable)
        {
            var get = (CodeMethodInvokeExpression)Parser.InternalMethods.GetEnv;
            get.Parameters.Add(variable.QualifiedName);
            return get;
        }
    }
}
