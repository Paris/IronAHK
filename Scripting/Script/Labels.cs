
namespace IronAHK.Scripting
{
    partial class Script
    {
        public static string LabelMethodName(string raw)
        {
            foreach (var sym in raw)
            {
                if (!char.IsLetterOrDigit(sym))
                    return string.Concat("label_", raw.GetHashCode().ToString("X"));
            }
            return raw;
        }

        public static object LabelCall(string name)
        {
            return FunctionCall(LabelMethodName(name), new object[] { });
        }
    }
}
