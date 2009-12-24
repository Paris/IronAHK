using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace IronAHK.Rusty
{
    partial class Core
    {
        /// <summary>
        /// Calls a function inside a DLL, such as a standard Windows API function.
        /// </summary>
        /// <param name="Function">
        /// <para>The DLL or EXE file name followed by a backslash and the name of the function. For example: "MyDLL\MyFunction" (the file extension ".dll" is the default when omitted). If an absolute path isn't specified, DllFile is assumed to be in the system's PATH or A_WorkingDir.</para>
        /// <para>DllFile may be omitted when calling a function that resides in User32.dll, Kernel32.dll, ComCtl32.dll, or Gdi32.dll. For example, "User32\IsWindowVisible" produces the same result as "IsWindowVisible". For these standard DLLs, the letter "A" suffix that appears on some API functions may also be omitted. For example, "MessageBox" is the same as "MessageBoxA".</para>
        /// <para>Performance can be dramatically improved when making repeated calls to a DLL by loading it beforehand.</para>
        /// <para>This parameter may also consist solely of an an integer, which is interpreted as the address of the function to call. Sources of such addresses include COM and RegisterCallback().</para>
        /// </param>
        /// <param name="Parameters">
        /// <para>Each of these pairs represents a single parameter to be passed to the function. The number of pairs is unlimited. For Type, see the types table below. For Arg, specify the value to be passed to the function.</para>
        /// <para>The word Cdecl is normally omitted because most functions use the standard calling convention rather than the "C" calling convention (functions such as wsprintf that accept a varying number of arguments are one exception to this). If you omit Cdecl but the call yields ErrorLevel An -- where n is the total size of the arguments you passed -- Cdecl might be required.</para>
        /// <para>If present, the word Cdecl should be listed before the return type (if any). Separate each word from the next with a space or tab. For example: "Cdecl Str"</para>
        /// <para>ReturnType: If the function returns a 32-bit signed integer (Int), BOOL, or nothing at all, ReturnType may be omitted. Otherwise, specify one of the argument types from the types table below. The asterisk suffix is also supported.</para>
        /// </param>
        /// <returns>DllCall returns the actual value returned by the function. If the function is of a type that does not return a value, the result is an undefined integer. If the function cannot be called due to an error, the return value is blank (an empty string).</returns>
        public static object DllCall(string Function, params object[] Parameters)
        {
            // see http://blogs.msdn.com/devinj/archive/2005/07/12/438323.aspx
            AppDomain domain = AppDomain.CurrentDomain;
            AssemblyName name = new AssemblyName("a");
            AssemblyBuilder assembly = domain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            ModuleBuilder module = assembly.DefineDynamicModule("w");

            var types = new List<Type>();
            var args = new List<object>();
            bool cdcel = false;
            Type t = typeof(int);

            for (int i = 0; i < Parameters.Length; i++)
            {
                if (i % 2 == 0) // type
                {
                    string p = ((string)Parameters[i]).Trim().ToLower();

                    if (i == Parameters.Length)
                    {
                        string c = "cdcel";
                        if (cdcel = p.Contains(c))
                            p = p.Replace(c, string.Empty);
                    }

                    int z = p.LastIndexOfAny(new char[] { 'p', '*' });
                    if (z != -1)
                    {
                        t = typeof(IntPtr);
                        p = p.Substring(0, z);
                    }
                    else
                        switch (p)
                        {
                            case "string":
                            case "str":
                                t = typeof(string);
                                break;
                            case "int64": t = typeof(Int64); break;
                            case "int": t = typeof(int); break;
                            case "short": t = typeof(short); break;
                            case "char": t = typeof(sbyte); break;
                            case "float": t = typeof(float); break;
                            case "double": t = typeof(double); break;
                            case "uint64": t = typeof(UInt64); break;
                            case "uint": t = typeof(uint); break;
                            case "ushort": t = typeof(ushort); break;
                            case "uchar": t = typeof(byte); break;
                        }
                }
                else
                {
                    types.Add(t);
                    if (t == typeof(IntPtr))
                    {
                        unsafe
                        {
                            byte[] p = (byte[])Parameters[i];
                            fixed (byte* pz = p)
                            {
                                args.Add(new IntPtr(pz));
                            }
                        }
                    }
                    else args.Add(Convert.ChangeType(Parameters[i], t));
                    t = typeof(int);
                }
            }

            int pos = Function.LastIndexOf(Path.PathSeparator);
            Type[] a_types = new Type[types.Count];
            types.CopyTo(a_types, 0);
            string sig = "m";
            MethodBuilder mb = module.DefinePInvokeMethod(
                sig,
                Function.Substring(0, pos),
                Function.Substring(pos + 1),
                MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.PinvokeImpl,
                CallingConventions.Standard,
                t,
                a_types,
                cdcel ? CallingConvention.Cdecl : CallingConvention.Winapi,
                CharSet.Auto);

            mb.SetImplementationFlags(MethodImplAttributes.PreserveSig | mb.GetMethodImplementationFlags());
            module.CreateGlobalFunctions();

            object[] a_args = new object[args.Count];
            args.CopyTo(a_args, 0);

            return module.GetMethod(sig).Invoke(null,
                BindingFlags.Default | BindingFlags.InvokeMethod,
                null,
                a_args,
                CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Calls a function dynamically.
        /// </summary>
        /// <param name="name">The name of the function to call.</param>
        /// <param name="parameters">Parameters to pass when calling the function. Types must be the same.</param>
        /// <returns>The return value of the function.</returns>
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
