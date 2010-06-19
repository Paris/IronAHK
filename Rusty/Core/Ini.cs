using System;
using System.Text;

namespace IronAHK.Rusty
{
    partial class Core
    {
        // TODO: cross platform Ini commands

        /// <summary>
        /// Deletes a value from a standard format .ini file.
        /// </summary>
        /// <param name="filename">The name of the file.</param>
        /// <param name="section">The section name, which is the heading phrase that appears in square brackets.</param>
        /// <param name="key">The key name. If omitted, the entire <paramref name="section"/> will be deleted.</param>
        public static void IniDelete(string filename, string section, string key)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                return;

            ErrorLevel = Windows.WritePrivateProfileString(section, key.Length == 0 ? null : key, null, filename) ? 0 : 1;
        }

        /// <summary>
        /// Reads a value from a standard format .ini file.
        /// </summary>
        /// <param name="output">The variable to store the result.</param>
        /// <param name="filename">The name of the file.</param>
        /// <param name="section">The section name, which is the heading phrase that appears in square brackets.</param>
        /// <param name="key">The key name.</param>
        /// <param name="error">The value to store in <paramref name="output"/> if the specified <paramref name="key"/> is not found.</param>
        public static void IniRead(out string output, string filename, string section, string key, string error)
        {
            output = null;

            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                return;

            var buf = new StringBuilder(char.MaxValue);
            Windows.GetPrivateProfileString(section, key, error, buf, (uint)buf.Capacity, filename);
            output = buf.ToString();
        }

        /// <summary>
        /// Writes a value to a standard format .ini file.
        /// </summary>
        /// <param name="value">The string or number that will be written to the right of <paramref name="key"/>'s equal sign (=).</param>
        /// <param name="filename">The name of the file.</param>
        /// <param name="section">The section name, which is the heading phrase that appears in square brackets.</param>
        /// <param name="key">The key name.</param>
        /// <remarks><see cref="ErrorLevel"/> is set to <c>1</c> if there was a problem or <c>0</c> otherwise.</remarks>
        public static void IniWrite(string value, string filename, string section, string key)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                return;

            ErrorLevel = Windows.WritePrivateProfileString(section, key, value, filename) ? 0 : 1;
        }
    }
}
