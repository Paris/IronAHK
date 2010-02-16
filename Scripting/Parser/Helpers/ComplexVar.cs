using System;
using System.CodeDom;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        CodeExpression ComplexVarRef(object var)
        {
            if (var is CodePrimitiveExpression)
                return (CodePrimitiveExpression)var;

            if (!(var is CodeComplexVariableReferenceExpression))
                throw new ArgumentException();

#pragma warning disable 0162
            if (UseComplexVar)
                return (CodeComplexVariableReferenceExpression)var;
            else
                return (CodeMethodInvokeExpression)(CodeComplexVariableReferenceExpression)var;
#pragma warning restore 0162
        }

        CodeExpression ComplexVarAssign(object var)
        {
            if (!(var is CodeComplexAssignStatement))
                throw new ArgumentException();

#pragma warning disable 0162
            if (UseComplexVar)
                return (CodeBinaryOperatorExpression)(CodeComplexAssignStatement)var;
            else
                return (CodeExpression)var;
#pragma warning restore 0162
        }

        CodeExpression WrappedComplexVar(object part)
        {
            return part is CodeComplexVariableReferenceExpression ? ComplexVarRef(part) :
                part is CodeComplexAssignStatement ? ComplexVarAssign(part) : (CodeExpression)part;
        }
    }
}
