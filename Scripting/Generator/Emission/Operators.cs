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
                case CodeBinaryOperatorType.Assign: return new string(new[] { Parser.AssignPre, Parser.Equal });
                case CodeBinaryOperatorType.BooleanAnd: return Parser.AndTxt;
                case CodeBinaryOperatorType.BooleanOr: return Parser.OrTxt;

                default: throw new ArgumentOutOfRangeException("op");
            }
        }

        string ScriptOperator(Script.Operator op)
        {
            switch (op)
            {
                case Script.Operator.Add: return Parser.Add.ToString();
                case Script.Operator.Address: return Parser.Address.ToString();
                case Script.Operator.Assign: return new string(new[] { Parser.AssignPre, Parser.Equal });
                case Script.Operator.BitShiftLeft: return new string(Parser.Less, 2);
                case Script.Operator.BitShiftRight: return new string(Parser.Greater, 2);
                case Script.Operator.BitwiseAnd: return Parser.BitAND.ToString();
                case Script.Operator.BitwiseNot: return Parser.BitNOT.ToString();
                case Script.Operator.BitwiseOr: return Parser.BitOR.ToString();
                case Script.Operator.BitwiseXor: return Parser.BitXOR.ToString();
                case Script.Operator.BooleanAnd: return Parser.AndTxt;
                case Script.Operator.BooleanOr: return Parser.OrTxt;
                case Script.Operator.Concat: return Parser.Concatenate.ToString();
                case Script.Operator.Decrement: return new string(Parser.Subtract, 2);;
                case Script.Operator.Dereference: return Parser.Dereference.ToString();
                case Script.Operator.Divide: return Parser.Divide.ToString();
                case Script.Operator.FloorDivide: return new string(Parser.Divide, 2);
                case Script.Operator.GreaterThan: return Parser.Greater.ToString();
                case Script.Operator.GreaterThanOrEqual: return new string(new[] { Parser.Greater, Parser.Equal });
                case Script.Operator.IdentityEquality: return new string(Parser.Equal, 2);
                case Script.Operator.IdentityInequality: return new string(new[] { Parser.Not, Parser.Equal, Parser.Equal });
                case Script.Operator.Increment: return new string(Parser.Add, 2);
                case Script.Operator.LessThan: return Parser.Less.ToString();
                case Script.Operator.LessThanOrEqual: return new string(new[] { Parser.Less, Parser.Equal });
                case Script.Operator.LogicalNot: return Parser.Not.ToString();
                case Script.Operator.LogicalNotEx: return Parser.NotTxt;
                case Script.Operator.Minus: return Parser.Minus.ToString();
                case Script.Operator.Multiply: return Parser.Multiply.ToString();
                case Script.Operator.Power: return new string(Parser.Multiply, 2);
                case Script.Operator.Subtract: return Parser.Subtract.ToString();
                case Script.Operator.TernaryA: return Parser.TernaryA.ToString();
                case Script.Operator.TernaryB: return Parser.TernaryB.ToString(); ;
                case Script.Operator.ValueEquality: return Parser.Equal.ToString();
                case Script.Operator.ValueInequality: return new string(new[] { Parser.Not, Parser.Equal });

                default: throw new ArgumentOutOfRangeException("op");
            }
        }
    }
}
