
namespace IronAHK.Scripting
{
    partial class Script
    {
        static int Called = 0;

        public static float ForceFloat(object input)
        {
            if (input is float)
                return (float)input;
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

        public static string ForceString(object input)
        {
            return input is string ? (string)input : input.ToString();
        }
    }
}
