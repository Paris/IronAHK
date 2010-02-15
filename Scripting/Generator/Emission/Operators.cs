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

        string ScriptOperator(Script.Operator op)
        {
            switch (op)
            {
                case Script.Operator.Add: return "+";
                case Script.Operator.Address: return "&";
                case Script.Operator.Assign: return ":=";
                case Script.Operator.BitShiftLeft: return "<<";
                case Script.Operator.BitShiftRight: return ">>";
                case Script.Operator.BitwiseAnd: return "&";
                case Script.Operator.BitwiseNot: return "~";
                case Script.Operator.BitwiseOr: return "|";
                case Script.Operator.BitwiseXor: return "^";
                case Script.Operator.BooleanAnd: return "and";
                case Script.Operator.BooleanOr: return "or";
                case Script.Operator.Concat: return ".";
                case Script.Operator.Decrement: return "--";
                case Script.Operator.Dereference: return "*";
                case Script.Operator.Divide: return "/";
                case Script.Operator.FloorDivide: return "//";
                case Script.Operator.GreaterThan: return ">";
                case Script.Operator.GreaterThanOrEqual: return ">=";
                case Script.Operator.IdentityEquality: return "==";
                case Script.Operator.IdentityInequality: return "!==";
                case Script.Operator.Increment: return "++";
                case Script.Operator.LessThan: return "<";
                case Script.Operator.LessThanOrEqual: return "<=";
                case Script.Operator.LogicalNot: return "!";
                case Script.Operator.LogicalNotEx: return "not";
                case Script.Operator.Minus: return "-";
                case Script.Operator.Multiply: return "*";
                case Script.Operator.Power: return "**";
                case Script.Operator.Subtract: return "-";
                case Script.Operator.TernaryA: return "?";
                case Script.Operator.TernaryB: return ":";
                case Script.Operator.ValueEquality: return "=";
                case Script.Operator.ValueInequality: return "!=";

                default: throw new ArgumentOutOfRangeException("op");
            }
        }
    }
}
