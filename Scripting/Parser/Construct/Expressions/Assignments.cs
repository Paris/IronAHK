using System.CodeDom;
using System.Collections.Generic;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        bool IsImplicitAssignment(List<object> parts, int i)
        {
            int x = i - 1, y = i + 1;

            if (!((parts[i] is string) && ((string)parts[i]).Length == 1 && ((string)parts[i])[0] == Equal))
                return false;

            if (x < 0 || !(parts[x] is CodeComplexVariableReferenceExpression))
                return false;

            if (!(y < parts.Count && parts[y] is string && IsVariable((string)parts[y])))
                return false;

            int z = x - 1;

            if (z < 0)
                return true;

            if (parts[z] is CodeComplexAssignStatement)
                return true;

            if (!(parts[z] is Script.Operator) || (Script.Operator)parts[z] == Script.Operator.IdentityEquality)
                return false;

            return true;
        }

        void MergeAssignmentAt(List<object> parts, int i)
        {
            if (!(parts[i] is CodeComplexAssignStatement))
                return;

            int x = i - 1, y = i + 1;
            bool right = y < parts.Count;

            var assign = (CodeComplexAssignStatement)parts[i];

            if (assign.Left != null)
                return;

            if (parts[x] is CodeBinaryOperatorExpression)
            {
                var binary = (CodeBinaryOperatorExpression)parts[x];
                assign.Left = (CodeComplexVariableReferenceExpression)binary.Left;
            }
            else if (parts[x] is CodeComplexVariableReferenceExpression)
                assign.Left = (CodeComplexVariableReferenceExpression)parts[x];
            else
                assign.Left = VarId((CodeExpression)parts[x]);
            assign.Right = right ? WrappedComplexVar(parts[y]) : new CodePrimitiveExpression(null);

            parts[x] = assign;

            if (right)
                parts.RemoveAt(y);
            parts.RemoveAt(i);
        }
    }
}
