
namespace IronAHK.Scripting
{
    partial class Script
    {
        public static void Parameters(string[] names, object[] values, string[] defaults)
        {
            for (int i = 0; i < names.Length; i++)
            {
                object init;

                if (i < values.Length)
                    init = values[i];
                else if (i < defaults.Length)
                    init = defaults[i];
                else
                    init = null;

                Vars[names[i]] = init;
            }
        }
    }
}
