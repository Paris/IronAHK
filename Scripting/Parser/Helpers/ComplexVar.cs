using System;
using System.CodeDom;
using System.Collections.Generic;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        CodeExpression ComplexVarRef(object var)
        {
#pragma warning disable 0162
            if (UseComplexVar)
                return (CodeComplexVariableReferenceExpression)var;
            else
                return (CodeMethodInvokeExpression)(CodeComplexVariableReferenceExpression)var;
#pragma warning restore 0162
        }
    }
}
