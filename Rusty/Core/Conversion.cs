using System;

namespace IronAHK.Rusty
{
    public partial class Core
    {
        static int Called = 0;

        public static float ForceFloat(object In)
        {
            if(In is int)
                return (float) (int) In;

            if(In is float)
                return (float) In;

            if(In is string)
            {
                float Ret;
                if(float.TryParse((string) In, out Ret))
                    return Ret;
            }

            return 0;
        }

        public static string ForceString(object In)
        {
            return In.ToString();
        }
    }
}
