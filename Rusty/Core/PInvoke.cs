using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace IronAHK.Rusty
{
    partial class Core
    {
        // TODO: organise PInvoke.cs

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
        public static object DllCall(object Function, params object[] Parameters)
        {
            ErrorLevel = 0;

            #region Parameters

            var types = new Type[Parameters.Length / 2];
            var refs = new bool[types.Length];
            var args = new object[types.Length];
            var returns = typeof(int);
            bool cdecl = false;

            for (int i = 0; i < Parameters.Length; i++)
            {
                Type type = null;
                string name = ((string)Parameters[i]).ToLowerInvariant().Trim();
                const string Cdecl = "cdecl";

                if (name.StartsWith(Cdecl))
                {
                    name = name.Substring(Cdecl.Length).Trim();

                    if (i + 1 == Parameters.Length)
                    {
                        cdecl = true;
                        goto returntype;
                    }
                    else
                    {
                        ErrorLevel = 2;
                        return null;
                    }
                }

                switch (name[name.Length - 1])
                {
                    case '*':
                    case 'P':
                    case 'p':
                        name = name.Substring(0, name.Length - 1).Trim();
                        ErrorLevel = -6;
                        // TODO: unmanaged pointers for pinvokes
                        return null;
                }

                switch (name)
                {
                    case "str": type = typeof(string); break;
                    case "int64": type = typeof(long); break;
                    case "uint64": type = typeof(ulong); break;
                    case "int": type = typeof(int); break;
                    case "uint": type = typeof(uint); break;
                    case "short": type = typeof(short); break;
                    case "ushort": type = typeof(ushort); break;
                    case "char": type = typeof(char); break;
                    case "uchar": type = typeof(char); break;
                    case "float": type = typeof(float); break;
                    case "double": type = typeof(double); break;

                    default:
                        ErrorLevel = 2;
                        return null;
                }

                i++;

                if (i < Parameters.Length)
                {
                    int n = i / 2;
                    types[n] = type;
                    args[n] = Convert.ChangeType(Parameters[i], type);
                    continue;
                }

            returntype:
                returns = type;
            }

            #endregion

            #region Method

            #region DLL
            if (Function is string)
            {
                string path = (string)Function, name;

                int z = path.LastIndexOf(Path.DirectorySeparatorChar);

                if (z == -1)
                {
                    name = path;
                    path = string.Empty;

                    if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    {
                        foreach (string lib in new[] { "user32", "kernel32", "comctl32", "gdi32" })
                        {
                            var handle = Windows.GetModuleHandle(lib);
                            if (handle == null || handle == IntPtr.Zero)
                                continue;
                            var address = Windows.GetProcAddress(handle, name);
                            if (!(handle == null || handle == IntPtr.Zero))
                            {
                                path = lib + ".dll";
                                break;
                            }
                        }
                    }

                    if (path.Length == 0)
                    {
                        ErrorLevel = -4;
                        return null;
                    }
                }
                else
                {
                    z++;

                    if (z >= path.Length)
                    {
                        ErrorLevel = -4;
                        return null;
                    }

                    name = path.Substring(z);
                    path = path.Substring(0, z - 1);
                }

                if (Environment.OSVersion.Platform == PlatformID.Win32NT && path.Length != 0 && !Path.HasExtension(path))
                    path += ".dll";

                var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("pinvokes"), AssemblyBuilderAccess.Run);
                var module = assembly.DefineDynamicModule("module");
                var container = module.DefineType("container", TypeAttributes.Public | TypeAttributes.UnicodeClass);

                var invoke = container.DefinePInvokeMethod(
                    name,
                    path,
                    MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.PinvokeImpl,
                    CallingConventions.Standard,
                    returns,
                    types,
                    cdecl ? CallingConvention.Cdecl : CallingConvention.Winapi,
                    CharSet.Auto);

                invoke.SetImplementationFlags(invoke.GetMethodImplementationFlags() | MethodImplAttributes.PreserveSig);
                var created = container.CreateType();
                // TODO: pinvoke method caching

                try
                {
                    var method = created.GetMethod(name);

                    if (method == null)
                    {
                        ErrorLevel = -4;
                        return null;
                    }

                    object value = method.Invoke(null, args);
                    return value;
                }
#if !DEBUG
                catch (Exception)
                {
                    ErrorLevel = -5;
                    return null;
                }
#endif
                finally { }
            }
            #endregion
            #region Address
            else if (Function is decimal || Function is int || Function is float)
            {
                int address = (int)Function;

                if (address < 0)
                {
                    ErrorLevel = -1;
                    return null;
                }

                try
                {
                    object value = Marshal.GetDelegateForFunctionPointer(new IntPtr(address), typeof(Delegate)).Method.Invoke(null, args);
                    return value;
                }
#if !DEBUG
                catch (Exception)
                {
                    ErrorLevel = -5;
                    return null;
                }
#endif
                finally { }
            }
            #endregion
            #region Uknown
            else
            {
                ErrorLevel = -3;
                return null;
            }
            #endregion

            #endregion
        }

        /// <summary>
        /// Returns the binary number stored at the specified address+offset. For VarOrAddress, passing MyVar is equivalent to passing &amp;MyVar. However, omitting the "&amp;" performs better and ensures that the target address is valid (invalid addresses return ""). By contrast, anything other than a naked variable passed to VarOrAddress is treated as a raw address; consequently, specifying MyVar+0 forces the number in MyVar to be used instead of the address of MyVar itself. For Type, specify UInt, Int, Int64, Short, UShort, Char, UChar, Double, or Float (though unlike DllCall, these must be enclosed in quotes when used as literal strings); for details see DllCall Types.
        /// </summary>
        /// <param name="VarOrAddress"></param>
        /// <param name="Offset"></param>
        /// <param name="Type"></param>
        public static void NumGet(int VarOrAddress, int Offset, string Type)
        {
            char[] type = Type.Trim().ToLowerInvariant().ToCharArray();
            IntPtr adr = new IntPtr(VarOrAddress);

            switch (type[1])
            {
                case 's': Marshal.ReadInt16(adr, Offset); break; // short
                case 'c': Marshal.ReadByte(adr, Offset); break; // char
                default: // double, int, int64
                    if (Array.Exists<char>(type, delegate(char match) { return match == '6'; }))
                        Marshal.ReadInt64(adr, Offset);
                    else Marshal.ReadInt32(adr, Offset);
                    break;
            }
        }

        /// <summary>
        /// Stores Number in binary format at the specified address+offset and returns the address to the right of the item just written. For VarOrAddress, passing MyVar is equivalent to passing &amp;MyVar. However, omitting the "&amp;" performs better and ensures that the target address is valid (invalid addresses return ""). By contrast, anything other than a naked variable passed to VarOrAddress is treated as a raw address; consequently, specifying MyVar+0 forces the number in MyVar to be used instead of the address of MyVar itself. For Type, specify UInt, Int, Int64, Short, UShort, Char, UChar, Double, or Float (though unlike DllCall, these must be enclosed in quotes when used as literal strings); for details see DllCall Types. If an integer is too large to fit in the specified Type, its most significant bytes are ignored; e.g. NumPut(257, var, 0, "Char") would store the number 1.
        /// </summary>
        /// <param name="Number"></param>
        /// <param name="VarOrAddress"></param>
        /// <param name="Offset"></param>
        /// <param name="Type"></param>
        public static void NumPut(int Number, int VarOrAddress, int Offset, string Type)
        {
            char[] type = (Type).Trim().ToLowerInvariant().ToCharArray();
            IntPtr adr = new IntPtr(VarOrAddress);

            switch (type[1])
            {
                case 's': Marshal.WriteInt16(adr, Offset, (char)Number); break; // short
                case 'c': Marshal.WriteByte(adr, Offset, (byte)Number); break; // char
                default: // double, int, int64
                    if (Array.Exists<char>(type, delegate(char match) { return match == '6'; }))
                        Marshal.WriteInt64(adr, Offset, (long)Number);
                    else Marshal.WriteInt32(adr, Offset, Number);
                    break;
            }
        }

        /// <summary>
        /// Converts a local function to a native function pointer.
        /// </summary>
        /// <param name="FunctionName">The name of the function.</param>
        /// <param name="Ignored">Unused legacy parameters.</param>
        /// <returns>An integer address to the function callable by unmanaged code.</returns>
        public static long RegisterCallback(string FunctionName, object[] Ignored)
        {
            var method = FindLocalMethod(FunctionName);

            if (method == null)
                return 0;

            try
            {
                var dlg = Delegate.CreateDelegate(typeof(GenericFunction), method);
                return Marshal.GetFunctionPointerForDelegate(dlg).ToInt64();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Enlarges a variable's holding capacity or frees its memory. Normally, this is necessary only for unusual circumstances such as DllCall.
        /// </summary>
        /// <param name="Var">The name of the variable (not in quotes). For example: VarSetCapacity(MyVar, 1000). This can also be a dynamic variable such as Array%i% or a function's ByRef parameter.</param>
        /// <param name="RequestedCapacity">
        /// <para>If omitted, the variable's current capacity will be returned and its contents will not be altered. Otherwise, anything currently in the variable is lost (the variable becomes blank).</para>
        /// <para>Specify for RequestedCapacity the length of string that the variable should be able to hold after the adjustment. This length does not include the internal zero terminator. For example, specifying 1 would allow the variable to hold up to one character in addition to its internal terminator. Note: the variable will auto-expand if the script assigns it a larger value later.</para>
        /// <para>Since this function is often called simply to ensure the variable has a certain minimum capacity, for performance reasons, it shrinks the variable only when RequestedCapacity is 0. In other words, if the variable's capacity is already greater than RequestedCapacity, it will not be reduced (but the variable will still made blank for consistency).</para>
        /// <para>Therefore, to explicitly shrink a variable, first free its memory with VarSetCapacity(Var, 0) and then use VarSetCapacity(Var, NewCapacity) -- or simply let it auto-expand from zero as needed.</para>
        /// <para>For performance reasons, freeing a variable whose previous capacity was between 1 and 63 might have no effect because its memory is of a permanent type. In this case, the current capacity will be returned rather than 0.</para>
        /// <para>For performance reasons, the memory of a variable whose capacity is under 4096 is not freed by storing an empty string in it (e.g. Var := ""). However, VarSetCapacity(Var, 0) does free it.</para>
        /// <para>Specify -1 for RequestedCapacity to update the variable's internally-stored length to the length of its current contents. This is useful in cases where the variable has been altered indirectly, such as by passing its address via DllCall(). In this mode, VarSetCapacity() returns the length rather than the capacity.</para>
        /// </param>
        /// <param name="FillByte">This parameter is normally omitted, in which case the memory of the target variable is not initialized (instead, the variable is simply made blank as described above). Otherwise, specify a number between 0 and 255. Each byte in the target variable's memory area (its current capacity) is set to that number. Zero is by far the most common value, which is useful in cases where the variable will hold raw binary data such as a DllCall structure.</param>
        /// <returns>The length of string that Var can now hold, which will be greater or equal to RequestedCapacity. If VarName is not a valid variable name (such as a literal string or number), 0 is returned. If the system has insufficient memory to make the change (very rare), an error dialog will be displayed and the current thread will exit.</returns>
        public static int VarSetCapacity(out byte[] Var, int RequestedCapacity, int FillByte)
        {
            Var = new byte[RequestedCapacity];

            byte fill = (byte)FillByte;
            if (fill != 0)
                for (int i = 0; i < Var.Length; i++)
                    Var[i] = fill;

            return Var.Length;
        }
    }
}
