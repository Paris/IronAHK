
namespace IronAHK.Scripting
{
    partial class Script
    {
        #region Numeric

        public static float ForceFloat(object input)
        {
            // TODO: remove float type

            if (input is float)
                return (float)input;
            else if (input is decimal)
                return (float)(decimal)input;
            else if (input is long)
                return (float)(long)input;
            else if (input is int)
                return (float)(int)input;
            else if (input is string)
            {
                float result;
                if (float.TryParse((string)input, out result))
                    return result;
            }

            return default(float);
        }

        public static decimal ForceDecimal(object input)
        {
            if (input is decimal)
                return (decimal)input;
            else if (input is float)
                return (decimal)(float)input;
            else if (input is long)
                return (decimal)(long)input;
            else if (input is int)
                return (decimal)(int)input;
            else if (input is string)
            {
                decimal result;
                if (decimal.TryParse((string)input, out result))
                    return result;
            }

            return default(decimal);
        }

        public static long ForceLong(object input)
        {
            if (input is long)
                return (long)input;
            else if (input is decimal)
                return (long)(decimal)input;
            else if (input is float)
                return (long)(float)input;
            else if (input is int)
                return (long)(int)input;
            else if (input is string)
            {
                long result;
                if (long.TryParse((string)input, out result))
                    return result;
            }

            return default(long);
        }

        public static int ForceInt(object input)
        {
            if (input is int)
                return (int)input;
            else if (input is decimal)
                return (int)(decimal)input;
            else if (input is float)
                return (int)(float)input;
            else if (input is long)
                return (int)(long)input;
            else if (input is string)
            {
                int result;
                if (int.TryParse((string)input, out result))
                    return result;
            }

            return default(int);
        }

        #endregion

        public static bool ForceBool(object input)
        {
            if (input == null)
                return false;
            else if (input is string)
                return !string.IsNullOrEmpty((string)input);
            else if (input is decimal || input is float || input is long || input is int)
                return ForceDecimal(input) != 0;

            return false;
        }

        public static string ForceString(object input)
        {
            if (input == null)
                return string.Empty;
            return input is string ? (string)input : input.ToString();
        }
    }
}
