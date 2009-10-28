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

                case Operator.Subtract:
                    return ForceDecimal(left) - ForceDecimal(right);

                case Operator.Multiply:
                    return ForceDecimal(left) * ForceDecimal(right);

                case Operator.BitShiftLeft:
                    return ForceLong(left) << ForceInt(right);

                case Operator.Concat:
                    return string.Concat(ForceString(left), ForceString(right));

                // TODO: complete other operators
            }

            return null;
        }
    }
}
