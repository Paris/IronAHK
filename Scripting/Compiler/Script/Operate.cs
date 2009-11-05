using System;

namespace IronAHK.Scripting
{
    partial class Script
    {
        public enum Operator
        {
            Add = 0,
            Subtract = 1,
            Multiply = 2,
            Divide = 3,
            Modulus = 4,
            Assign = 5,
            IdentityInequality = 6,
            IdentityEquality = 7,
            ValueEquality = 8,
            BitwiseOr = 9,
            BitwiseAnd = 10,
            BooleanOr = 11,
            BooleanAnd = 12,
            LessThan = 13,
            LessThanOrEqual = 14,
            GreaterThan = 15,
            GreaterThanOrEqual = 16,

            Increment,
            Decrement,

            Power,
            FloorDivide,
            BitShiftRight,
            BitShiftLeft,
            ValueInequality,

            Concat,
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

                case Operator.Decrement:
                    return ForceDecimal(left) - 1; // UNDONE: should unary decrement be here?

                case Operator.Divide:
                    return ForceDecimal(left) / ForceDecimal(right);

                case Operator.FloorDivide:
                    return Math.Floor(ForceDecimal(left) / ForceDecimal(right));

                case Operator.GreaterThan:
                    return ForceDecimal(left) > ForceDecimal(right);

                case Operator.GreaterThanOrEqual:
                    return ForceDecimal(left) >= ForceDecimal(right);

                case Operator.IdentityEquality:
                    return left.Equals(right);

                case Operator.IdentityInequality:
                    return !left.Equals(right);

                case Operator.Increment:
                    return ForceDecimal(left) + 1; // UNDONE: should unary decrement be here?

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
                    return left.Equals(right);

                case Operator.ValueInequality:
                    MatchTypes(ref left, ref right);
                    return !left.Equals(right);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
