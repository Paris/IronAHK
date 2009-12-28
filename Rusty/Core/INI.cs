using System;
using System.Text;

namespace IronAHK.Rusty
{
    partial class Core
    {
        /// <summary>
        /// Deletes a value from a standard format .ini file.
        /// </summary>
        /// <param name="Filename">The name of the .ini file.</param>
        /// <param name="Section">The section name in the .ini file, which is the heading phrase that appears in square brackets (do not include the brackets in this parameter).</param>
        /// <param name="Key">The key name in the .ini file. If omitted, the entire Section will be deleted.</param>
        public static void IniDelete(string Filename, string Section, string Key)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                return;

            error = Windows.WritePrivateProfileString(Section, Key.Length == 0 ? null : Key, null, Filename) ? 0 : 1;
        }

        /// <summary>
        /// Reads a value from a standard format .ini file.
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the retrieved value. If the value cannot be retrieved, the variable is set to the value indicated by the Default parameter (described below).</param>
        /// <param name="Filename">The name of the .ini file.</param>
        /// <param name="Section">The section name in the .ini file, which is the heading phrase that appears in square brackets (do not include the brackets in this parameter).</param>
        /// <param name="Key">The key name in the .ini file.</param>
        /// <param name="Default">The value to store in <paramref name="OutputVar"/> if the requested <paramref name="Key"/> is not found.</param>
        public static void IniRead(out string OutputVar, string Filename, string Section, string Key, string Default)
        {
            OutputVar = null;

            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                return;

            StringBuilder buf = new StringBuilder(char.MaxValue);
            Windows.GetPrivateProfileString(Section, Key, Default, buf, (uint)buf.Capacity, Filename);
            OutputVar = buf.ToString();
        }

        /// <summary>
        /// Writes a value to a standard format .ini file.
        /// </summary>
        /// <param name="Value">The string or number that will be written to the right of Key's equal sign (=).</param>
        /// <param name="Filename">The name of the .ini file.</param>
        /// <param name="Section">The section name in the .ini file, which is the heading phrase that appears in square brackets (do not include the brackets in this parameter).</param>
        /// <param name="Key">The key name in the .ini file.</param>
        /// <remarks>ErrorLevel is set to 1 if there was a problem or 0 otherwise.</remarks>
        public static void IniWrite(string Value, string Filename, string Section, string Key)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                return;

            error = Windows.WritePrivateProfileString(Section, Key, Value, Filename) ? 0 : 1;
        }
    }
}
