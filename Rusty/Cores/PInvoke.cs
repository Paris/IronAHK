using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace IronAHK.Rusty
{
    partial class Core
    {
        /// <summary>
        /// Calls an unmanaged function in a DLL.
        /// </summary>
        /// <param name="function">
        /// <para>The path to the function, e.g. <c>C:\path\to\my.dll</c>. The ".dll" file extension can be omitted.</para>
        /// <para>If an absolute path is not specified on Windows the function will search the following system libraries (in order):
        /// User32.dll, Kernel32.dll, ComCtl32.dll, or Gdi32.dll.</para>
        /// </param>
        /// <param name="parameters">The type and argument list.</param>
        /// <returns>The value returned by the function.</returns>
        /// <remarks>
        /// <para><see cref="ErrorLevel"/> will be set to one of the following:</para>
        /// <list type="bullet">
        /// <item><term>0</term>: <description>success</description></item>
        /// <item><term>-3</term>: <description>file could not be accessed</description></item>
        /// <item><term>-4</term>: <description>function could not be found</description></item>
        /// </list>
        /// <para>The following types can be used:</para>
        /// <list type="bullet">
        /// <item><term><c>str</c></term>: <description>a string</description></item>
        /// <item><term><c>int64</c></term>: <description>a 64-bit integer</description></item>
        /// <item><term><c>int</c></term>: <description>a 32-bit integer</description></item>
        /// <item><term><c>short</c></term>: <description>a 16-bit integer</description></item>
        /// <item><term><c>char</c></term>: <description>an 8-bit integer</description></item>
        /// <item><term><c>float</c></term>: <description>a 32-bit floating point number</description></item>
        /// <item><term><c>double</c></term>: <description>a 64-bit floating point number</description></item>
        /// <item><term><c>*</c> or <c>P</c> suffix</term>: <description>pass the specified type by address</description></item>
        /// <item><term><c>U</c> prefix</term>: <description>use unsigned values for numeric types</description></item>
        /// </list>
        /// </remarks>
        public static object DllCall(object function, params object[] parameters)
        {
            ErrorLevel = 0;

            #region Parameters

            var types = new Type[parameters.Length / 2];
            var args = new object[types.Length];
            var returns = typeof(int);
            bool cdecl = false;

            for (int i = 0; i < parameters.Length; i++)
            {
                Type type = null;
                string name = ((string)parameters[i]).ToLowerInvariant().Trim();
                const string Cdecl = "cdecl";

                if (name.StartsWith(Cdecl))
                {
                    name = name.Substring(Cdecl.Length).Trim();

                    if (i + 1 == parameters.Length)
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

                if (i < parameters.Length)
                {
                    int n = i / 2;
                    types[n] = type;
                    args[n] = Convert.ChangeType(parameters[i], type);
                    continue;
                }

            returntype:
                returns = type;
            }

            #endregion

            #region Method

            #region DLL
            if (function is string)
            {
                string path = (string)function, name;

                int z = path.LastIndexOf(Path.DirectorySeparatorChar);

                if (z == -1)
                {
                    name = path;
                    path = string.Empty;

                    if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    {
                        foreach (var lib in new[] { "user32", "kernel32", "comctl32", "gdi32" })
                        {
                            var handle = WindowsAPI.GetModuleHandle(lib);
                            if (handle == IntPtr.Zero)
                                continue;
                            var address = WindowsAPI.GetProcAddress(handle, name);
                            if (address == IntPtr.Zero)
                                address = WindowsAPI.GetProcAddress(handle, name + "W");
                            if (address != IntPtr.Zero)
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
            else if (function is decimal || function is int || function is float)
            {
                var address = (int)function;

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
        /// Find a function in the local scope.
        /// </summary>
        /// <param name="name">The name of the function.</param>
        /// <returns>A delegate (function pointer).</returns>
        public static GenericFunction FunctionReference(string name)
        {
            var method = FindLocalMethod(name);

            if (method == null)
                return null;

            return (GenericFunction)Delegate.CreateDelegate(typeof(GenericFunction), method);
        }

        /// <summary>
        /// Returns a binary number stored at the specified address in memory.
        /// </summary>
        /// <param name="address">The address in memory.</param>
        /// <param name="offset">The offset from <paramref name="address"/>.</param>
        /// <param name="type">Any type outlined in <see cref="DllCall"/>.</param>
        /// <returns>The value stored at the address.</returns>
        public static long NumGet(long address, int offset = 0, string type = "UInt")
        {
            var adr = new IntPtr(address);
            char mode = type.Length > 1 ? type.ToLowerInvariant()[1] : '\0';

            switch (mode)
            {
                case 's': return Marshal.ReadInt16(adr, offset); // short
                case 'c': return Marshal.ReadByte(adr, offset); // char
                default: // double, int, int64
                    if (type.Contains("6"))
                        return Marshal.ReadInt64(adr, offset);
                    else return Marshal.ReadInt32(adr, offset);
            }
        }

        /// <summary>
        /// Stores a number in binary format at the specified address in memory.
        /// </summary>
        /// <param name="number">The number to store.</param>
        /// <param name="address">The address in memory.</param>
        /// <param name="offset">The offset from <paramref name="address"/>.</param>
        /// <param name="type">Any type outlined in <see cref="DllCall"/>.</param>
        /// <returns>The address of the first byte written.</returns>
        public static long NumPut(int number, long address, int offset = 0, string type = "UInt")
        {
            var adr = new IntPtr(address);
            char mode = type.Length > 1 ? type.ToLowerInvariant()[1] : '\0';

            switch (mode)
            {
                case 's': Marshal.WriteInt16(adr, offset, (char)number); break; // short
                case 'c': Marshal.WriteByte(adr, offset, (byte)number); break; // char
                default: // double, int, int64
                    if (type.Contains("6"))
                        Marshal.WriteInt64(adr, offset, number);
                    else Marshal.WriteInt32(adr, offset, number);
                    break;
            }

            return adr.ToInt64() + offset;
        }

        /// <summary>
        /// Converts a local function to a native function pointer.
        /// </summary>
        /// <param name="function">The name of the function.</param>
        /// <param name="args">Unused legacy parameters.</param>
        /// <returns>An integer address to the function callable by unmanaged code.</returns>
        public static long RegisterCallback(string function, object[] args)
        {
            var method = FindLocalMethod(function);

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
        /// Enlarges a variable's holding capacity. Usually only necessary for <see cref="DllCall"/>.
        /// </summary>
        /// <param name="variable">The variable to change.</param>
        /// <param name="capacity">Specify zero to return the current capacity.
        /// Otherwise <paramref name="variable"/> will be recreated as a byte array with this total length.</param>
        /// <param name="pad">Specify a value between 0 and 255 to initalise each index with this number.</param>
        /// <returns>The total capacity of <paramref name="variable"/>.</returns>
        public static int VarSetCapacity(ref object variable, int capacity = 0, int pad = -1)
        {
            if (capacity == 0)
                return Marshal.SizeOf(variable);

            var bytes = new byte[capacity];

            var fill = (byte)pad;
            if (pad > -1 && pad < 256)
                for (int i = 0; i < bytes.Length; i++)
                    bytes[i] = fill;

            variable = bytes;
            return bytes.Length;
        }
    }
}
