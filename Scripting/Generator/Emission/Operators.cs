using System;
using System.CodeDom;

namespace IronAHK.Scripting
{
    partial class Emit
    {
        string Operator(CodeBinaryOperatorType op)
        {
            switch (op)
            {
                case CodeBinaryOperatorType.Add: return "+";
                case CodeBinaryOperatorType.Assign: return ":=";
                case CodeBinaryOperatorType.BitwiseAnd: return "&";
                case CodeBinaryOperatorType.BitwiseOr: return "|";
                case CodeBinaryOperatorType.BooleanAnd: return "and";
                case CodeBinaryOperatorType.BooleanOr: return "or";
                case CodeBinaryOperatorType.Divide: return "/";
                case CodeBinaryOperatorType.GreaterThan: return ">";
                case CodeBinaryOperatorType.GreaterThanOrEqual: return ">=";
                case CodeBinaryOperatorType.IdentityEquality: return "==";
                case CodeBinaryOperatorType.IdentityInequality: return "!=";
                case CodeBinaryOperatorType.LessThan: return "<";
                case CodeBinaryOperatorType.LessThanOrEqual: return "<=";
                case CodeBinaryOperatorType.Multiply: return "*";
                case CodeBinaryOperatorType.Subtract: return "-";
                case CodeBinaryOperatorType.ValueEquality: return "=";

                default: throw new ArgumentOutOfRangeException("op");
            }
        }
    }
}
