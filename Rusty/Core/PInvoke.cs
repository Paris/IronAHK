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
                    string p = ((string)Parameters[i]).Trim().ToLowerInvariant();

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
        /// Creates a machine-code address that when called, redirects the call to a function in the script.
        /// </summary>
        /// <param name="FunctionName">A function's name, which must be enclosed in quotes if it is a literal string. This function is called automatically whenever Address is called. The function also receives the parameters that were passed to Address.</param>
        /// <param name="Options">
        /// <para>Specify zero or more of the following words. Separate each option from the next with a space (e.g. "C Fast").</para>
        /// <para>Fast or F: Avoids starting a new thread each time FunctionName is called. Although this performs much better, it must be avoided whenever the thread from which Address is called varies (e.g. when the callback is triggered by an incoming message). This is because FunctionName will be able to change global settings such as ErrorLevel, A_LastError, and the last-found window for whichever thread happens to be running at the time it is called. For more information, see Remarks.</para>
        /// <para>CDecl or C : Makes Address conform to the "C" calling convention. This is typically omitted because the standard calling convention is much more common for callbacks.</para>
        /// </param>
        /// <param name="ParamCount">The number of parameters that Address's caller will pass to it. If entirely omitted, it defaults to the number of mandatory parameters in the definition of FunctionName. In either case, ensure that the caller passes exactly this number of parameters.</param>
        /// <param name="EventInfo">An integer between 0 and 4294967295 that FunctionName will see in A_EventInfo whenever it is called via this Address. This is useful when FunctionName is called by more than one Address. If omitted, it defaults to Address. Note: Unlike other global settings, the current thread's A_EventInfo is not disturbed by the fast mode.</param>
        /// <returns>Upon success, RegisterCallback() returns a numeric address that may be called by DllCall() or anything else capable of calling a machine-code function. Upon failure, it returns an empty string. Failure occurs when FunctionName: 1) does not exist; 2) accepts too many or too few parameters according to ParamCount; or 3) accepts any ByRef parameters.</returns>
        public static int RegisterCallback(string FunctionName, string Options, string ParamCount, string EventInfo)
        {
            return 0;
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
