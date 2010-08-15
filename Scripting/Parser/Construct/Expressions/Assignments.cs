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

            if (x < 0 || !IsVarReference(parts[x]))
                return false;

            if (!(y < parts.Count && parts[y] is string && IsVariable((string)parts[y])))
                return false;

            int z = x - 1;

            if (z < 0)
                return true;

            if (IsVarAssignment(parts[z]))
                return true;

            if (!(parts[z] is Script.Operator) || (Script.Operator)parts[z] == Script.Operator.IdentityEquality)
                return false;

            return true;
        }

        void MergeAssignmentAt(List<object> parts, int i)
        {
            int x = i - 1, y = i + 1;
            bool right = y < parts.Count;

            if (parts[i] as CodeBinaryOperatorType? != CodeBinaryOperatorType.Assign)
                return;

            if (i > 0 && IsJsonObject(parts[x]))
            {
                MergeObjectAssignmentAt(parts, i);
                return;
            }
            else if (i > 0 && IsArrayExtension(parts[x]))
            {
                var extend = (CodeMethodInvokeExpression)parts[x];
                extend.Parameters.Add(right ? VarMixedExpr(parts[y]) : new CodePrimitiveExpression(null));
                if (right)
                    parts.RemoveAt(y);
                parts.RemoveAt(i);
                return;
            }

            var assign = new CodeBinaryOperatorExpression { Operator = CodeBinaryOperatorType.Assign };
            parts[i] = assign;

            if (assign.Left != null)
                return;

            if (parts[x] is CodeBinaryOperatorExpression)
            {
                var binary = (CodeBinaryOperatorExpression)parts[x];
                assign.Left = (CodeArrayIndexerExpression)binary.Left;
            }
            else if (IsVarReference(parts[x]))
                assign.Left = (CodeArrayIndexerExpression)parts[x];
            else if (parts[x] is CodePropertyReferenceExpression)
                assign.Left = (CodePropertyReferenceExpression)parts[x];
            else
                assign.Left = VarId((CodeExpression)parts[x]);
            assign.Right = right ? VarMixedExpr(parts[y]) : new CodePrimitiveExpression(null);

            parts[x] = assign;

            if (right)
                parts.RemoveAt(y);
            parts.RemoveAt(i);
        }

        void MergeObjectAssignmentAt(List<object> parts, int i)
        {
            int x = i - 1, y = i + 1;
            var invoke = (CodeMethodInvokeExpression)parts[x];
            CodeExpression target = null;
            var step = new List<CodeExpression>();

            while (invoke.Parameters.Count == 2 && invoke.Method.MethodName == InternalMethods.Index.MethodName)
            {
                step.Add(invoke.Parameters[1]);

                if (invoke.Parameters[0] is CodeMethodInvokeExpression)
                    invoke = (CodeMethodInvokeExpression)invoke.Parameters[0];
                else
                {
                    target = invoke.Parameters[0];
                    break;
                }
            }

            var set = (CodeMethodInvokeExpression)InternalMethods.SetObject;
            set.Parameters.Add(step[0]);
            step.RemoveAt(0);
            set.Parameters.Add(target);
            set.Parameters.Add(new CodeArrayCreateExpression(typeof(object), step.ToArray()));

            if (y < parts.Count)
            {
                set.Parameters.Add(VarMixedExpr(parts[y]));
                parts.RemoveAt(y);
            }
            else
                set.Parameters.Add(new CodePrimitiveExpression(null));

            parts.RemoveAt(i);
            parts[x] = set;
        }
    }
}
