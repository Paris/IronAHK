using System;

namespace IronAHK.Scripting
{
    partial class Script
    {
        #region Binary

        public enum Operator
        {
            Add,
            Subtract,
            Multiply,
            Divide,
            Modulus,
            Assign,
            IdentityInequality,
            IdentityEquality,
            ValueEquality,
            BitwiseOr,
            BitwiseAnd,
            BooleanOr,
            BooleanAnd,
            LessThan,
            LessThanOrEqual,
            GreaterThan,
            GreaterThanOrEqual,

            Increment,
            Decrement,

            Minus,
            LogicalNot,
            BitwiseNot,
            Address,
            Dereference,

            Power,
            FloorDivide,
            BitShiftRight,
            BitShiftLeft,
            BitwiseXor,
            ValueInequality,
            Concat,

            LogicalNotEx,

            TernaryA,
            TernaryB,
        };

        public static object Operate(Operator op, object left, object right)
        {
            switch (op)
            {
                case Operator.Add:
                    return ForceDecimal(left) + ForceDecimal(right);

                case Operator.BitShiftLeft:
                    return ForceLong(left) << ForceInt(right);

                case Operator.BitShiftRight:
                    return ForceLong(left) >> ForceInt(right);

                case Operator.BitwiseAnd:
                    return ForceLong(left) & ForceLong(right);

                case Operator.BitwiseOr:
                    return ForceLong(left) | ForceLong(right);

                case Operator.BooleanAnd:
                    return ForceBool(left) && ForceBool(right);

                case Operator.BooleanOr:
                    return ForceBool(left) || ForceBool(right);

                case Operator.Concat:
                    return string.Concat(ForceString(left), ForceString(right));

                case Operator.Divide:
                    return ForceDecimal(left) / ForceDecimal(right);

                case Operator.FloorDivide:
                    return Math.Floor(ForceDecimal(left) / ForceDecimal(right));

                case Operator.GreaterThan:
                    return ForceDecimal(left) > ForceDecimal(right);

                case Operator.GreaterThanOrEqual:
                    return ForceDecimal(left) >= ForceDecimal(right);

                case Operator.IdentityEquality:
                    return left == null ? right == null : left.Equals(right);

                case Operator.IdentityInequality:
                    return left == null ? right != null : !left.Equals(right);

                case Operator.LessThan:
                    return ForceDecimal(left) < ForceDecimal(right);

                case Operator.LessThanOrEqual:
                    return ForceDecimal(left) <= ForceDecimal(right);

                case Operator.Modulus:
                    return ForceDecimal(left) % ForceDecimal(right);

                case Operator.Multiply:
                    return ForceDecimal(left) * ForceDecimal(right);

                case Operator.Power:
                    return Math.Pow((double)ForceDecimal(left), (double)ForceDecimal(right));

                case Operator.Subtract:
                    return ForceDecimal(left) - ForceDecimal(right);

                case Operator.ValueEquality:
                    MatchTypes(ref left, ref right);
                    return left == null ? right == null : left.Equals(right);

                case Operator.ValueInequality:
                    MatchTypes(ref left, ref right);
                    return left == null ? right != null : !left.Equals(right);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region Ternary

        public delegate object ExpressionDelegate();

        public static object OperateTernary(bool result, ExpressionDelegate x, ExpressionDelegate y)
        {
            return result ? x() : y();
        }

        #endregion

        #region Unary

        public static object OperateUnary(Operator op, object right)
        {
            switch (op)
            {
                case Operator.Minus:
                    {
                        decimal value = ForceDecimal(right);
                        return value == default(decimal) ? right : -value;
                    }

                case Operator.LogicalNot:
                case Operator.LogicalNotEx:
                    return !IfTest(right);

                case Operator.BitwiseNot:
                    return IsNumeric(right) ? right : ~ForceInt(right);

                case Operator.Dereference:
                    // TODO: dereference operator
                    return null;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static bool IfTest(object result)
        {
            if (result is bool)
                return (bool)result;
            else if (result is decimal || result is long || result is int)
                return ((decimal)result) != 0;
            else if(result is float)
                return ((float)result) != 0;
            else if (result is string)
                return !string.IsNullOrEmpty((string)result);
            else
                return result != null;
        }

        #endregion

        #region Helpers

        static bool IsNumeric(object value)
        {
            return value is int || value is float || value is decimal;
        }

        #endregion
    }
}
