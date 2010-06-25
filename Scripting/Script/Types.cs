using System;

namespace IronAHK.Scripting
{
    partial class Script
    {
        static Type MatchTypes(ref object left, ref object right)
        {
            if (left is string || right is string)
            {
                left = ForceString(left);
                right = ForceString(right);
                return typeof(string);
            }
            else if (left is long || right is long)
            {
                left = ForceLong(left);
                right = ForceLong(right);
                return typeof(long);
            }
            else if (left is int || right is int)
            {
                left = ForceInt(left);
                right = ForceInt(right);
                return typeof(int);
            }
            else if (left is bool || right is bool)
            {
                left = ForceBool(left);
                right = ForceBool(right);
                return typeof(bool);
            }
            else
            {
                return null;
            }
        }

        static object ForceType(Type requested, object value)
        {
            if (requested == typeof(object) || requested.IsAssignableFrom(value.GetType()))
                return value;

            if (requested == typeof(decimal))
                return ForceDecimal(value);

            if (requested == typeof(double))
                return ForceDouble(value);

            if (requested == typeof(int))
                return ForceInt(value);

            if (requested == typeof(long))
                return ForceLong(value);

            if (requested == typeof(string))
                return ForceString(value);

            return value;
        }
    }
}
