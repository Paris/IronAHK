using System;
using System.Diagnostics;
using System.Reflection;

namespace IronAHK.Rusty
{
    partial class Core
    {
        static object SafeInvoke(string name, params object[] args)
        {
            var method = FindLocalRoutine(name);

            if (method == null)
                return null;

            try
            {
                return method.Invoke(null, new object[] { args });
            }
            catch { }

            return null;
        }

        static void SafeSetProperty(object item, string name, object value)
        {
            var prop = item.GetType().GetProperty(name, value.GetType());

            if (prop == null)
                return;

            prop.SetValue(item, value, null);
        }

        static MethodInfo FindLocalRoutine(string name)
        {
            return FindLocalMethod(LabelMethodName(name));
        }

        static MethodInfo FindLocalMethod(string name)
        {
            var stack = new StackTrace(false).GetFrames();

            for (int i = 0; i < stack.Length; i++)
            {
                var type = stack[i].GetMethod().DeclaringType;

                if (type == typeof(Core))
                    continue;

                // UNDONE: better way to check correct type for reflecting local methods
                if (type.FullName != "Program")
                    continue;

                var list = type.GetMethods();

                for (int z = 0; z < list.Length; z++)
                    if (list[z].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                        return list[z];
            }

            return null;
        }

        static string LabelMethodName(string raw)
        {
            foreach (var sym in raw)
            {
                if (!char.IsLetterOrDigit(sym))
                    return string.Concat("label_", raw.GetHashCode().ToString("X"));
            }
            return raw;
        }
    }
}
