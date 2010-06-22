using System;
using System.Diagnostics;
using System.Reflection;

namespace IronAHK.Scripting
{
    partial class Script
    {
        public static object Invoke(object del, params object[] parameters)
        {
            if (!(del is Delegate))
                return null;

            try
            {
                return ((Delegate)del).DynamicInvoke(new object[] { parameters });
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static object FunctionCall(string name, params object[] parameters)
        {
            var stack = new StackTrace(false).GetFrames();
            MethodInfo method = null;

            for (int i = 0; i < 3; i++)
            {
                var type = stack[i].GetMethod().DeclaringType;
                method = FindMethod(name, type.GetMethods(), parameters);
                if (method != null)
                    break;
            }

            return method == null || !method.IsStatic ? null : method.Invoke(null, new object[] { parameters });
        }

        static MethodInfo FindMethod(string name, MethodInfo[] list, object[] parameters)
        {
            for (int i = 0; i < list.Length; i++)
                if (list[i].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return list[i];

            return null;
        }
    }
}
