using System;
using System.Collections;

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

            Is,
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

                case Operator.Is:
                    return IfLegacy(left, "is", ForceString(right));

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
                case Operator.Subtract:
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
            else if (IsNumeric(result))
                return ForceDecimal(result) != 0;
            else if (result is string)
                return !string.IsNullOrEmpty((string)result);
            else
                return result != null;
        }

        public static bool IfLegacy(object subject, string op, string test)
        {
            // TODO: if [not] {between,in,contains,is}

            const string Between = "between";
            const string In = "in";
            const string Contains = "contains";
            const string Is = "is";
            const char Delimiter = ',';
            const string And = "and";

            const string Integer = "integer";
            const string Float = "float";
            const string Number = "number";
            const string Digit = "digit";
            const string Xdigit = "xdigit";
            const string Alpha = "alpha";
            const string Upper = "upper";
            const string Lower = "lower";
            const string Alnum = "alnum";
            const string Space = "space";
            const string Time = "time";
            const string Object = "object";
            const string Array = "array";

            string var = ForceString(subject);

            switch (op)
            {
                case Between:
                    {
                        int z = var.IndexOf(And, StringComparison.OrdinalIgnoreCase);

                        if (z == -1)
                            z = var.Length;

                        decimal low = decimal.MinValue, high = int.MaxValue;

                        if (decimal.TryParse(var.Substring(0, z), out low) && decimal.TryParse(var.Substring(z + And.Length), out high))
                        {
                            decimal d = ForceDecimal(subject);
                            return d >= low && d <= high;
                        }
                    }
                    return false;

                case In:
                    foreach (string sub in test.Split(Delimiter))
                        if (var.Equals(sub, StringComparison.OrdinalIgnoreCase))
                            return true;
                    return false;

                case Contains:
                    foreach (string sub in test.Split(Delimiter))
                        if (var.IndexOf(sub, StringComparison.OrdinalIgnoreCase) != -1)
                            return true;
                    return false;

                case Is:
                    test = test.ToLowerInvariant();
                    var type = subject.GetType();
                    switch (test)
                    {
                        case Object:
                            return typeof(IDictionary).IsAssignableFrom(type);

                        case Array:
                            return type.IsArray;
                    }
                    switch (test)
                    {
                        case Integer:
                        case Number:
                            var = var.Trim().TrimStart(new[] { '+', '-' });
                            goto case Xdigit;

                        case Xdigit:
                            if (var.Length > 3 && var[0] == '0' && (var[1] == 'x' || var[1] == 'X'))
                                var = var.Substring(2);
                            break;
                    }
                    switch (test)
                    {
                        case Float:
                            if (!var.Contains("."))
                                return false;
                            goto case Number;

                        case Number:
                            {
                                bool dot = false;

                                foreach (char sym in var)
                                {
                                    if (sym == '.')
                                    {
                                        if (dot)
                                            return false;
                                        dot = true;
                                    }
                                    else if (!char.IsDigit(sym))
                                        return false;
                                }

                                return true;
                            }

                        case Digit:
                            foreach (char sym in var)
                                if (!char.IsDigit(sym))
                                    return false;
                            return true;

                        case Integer:
                        case Xdigit:
                            {
                                foreach (char sym in var)
                                    if (!(char.IsDigit(sym) || (sym > 'a' - 1 && sym < 'f' + 1) || (sym > 'A' - 1 && sym < 'F' + 1)))
                                        return false;
                                return true;
                            }

                        case Alpha:
                            foreach (char sym in var)
                                if (!char.IsLetter(sym))
                                    return false;
                            return true;

                        case Upper:
                            foreach (char sym in var)
                                if (!char.IsUpper(sym))
                                    return false;
                            return true;

                        case Lower:
                            foreach (char sym in var)
                                if (!char.IsLower(sym))
                                    return false;
                            return true;

                        case Alnum:
                            foreach (char sym in var)
                                if (!char.IsLetterOrDigit(sym))
                                    return false;
                            return true;

                        case Space:
                            foreach (char sym in var)
                                if (!char.IsWhiteSpace(sym))
                                    return false;
                            return true;

                        case Time:
                            if (!IsNumeric(var))
                                return false;
                            return ForceLong(var) < 99991231125959;

                        default:
                            return false;
                    }
            }

            return true;
        }

        #endregion

        #region Misc

        public static int OperateZero(object expression)
        {
            return 0;
        }

        #endregion

        #region Helpers

        static bool IsNumeric(object value)
        {
            return value is int || value is float || value is double || value is decimal;
        }

        #endregion
    }
}
