using System;
using System.CodeDom;

namespace IronAHK.Scripting
{
    class CodeComplexAssignStatement : CodeAssignStatement
    {
        public CodeComplexAssignStatement(CodeComplexVariableReferenceExpression left, CodeExpression right)
            : base(left, right) { }

        public new CodeComplexVariableReferenceExpression Left
        {
            get { return base.Left as CodeComplexVariableReferenceExpression; }
            set { base.Left = value; }
        }

        public static explicit operator CodeMethodInvokeExpression(CodeComplexAssignStatement assignment)
        {
            var set = new CodeMethodInvokeExpression();
            set.Method = new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(Rusty.Core)), "SetEnv");

            set.Parameters.Add(assignment.Left.QualifiedName);
            set.Parameters.Add(assignment.Right);

            return set;
        }
    }
}
