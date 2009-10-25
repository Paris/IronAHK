
namespace IronAHK.Scripting
{
    partial class Script
    {
        public static void Parameters(string[] names, object[] values, object[] defaults)
        {
            for (int i = 0; i < names.Length; i++)
                SetEnv(names[i], i < values.Length ? values[i] : i < defaults.Length ? defaults[i] : null);
        }
    }
}
