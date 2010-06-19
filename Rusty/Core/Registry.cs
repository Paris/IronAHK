using System;

namespace IronAHK.Rusty
{
    partial class Core
    {
        // TODO: organise Registry.cs

        /// <summary>
        /// Deletes a subkey or value from the registry.
        /// </summary>
        /// <param name="RootKey">Must be either HKEY_LOCAL_MACHINE, HKEY_USERS, HKEY_CURRENT_USER, HKEY_CLASSES_ROOT, or HKEY_CURRENT_CONFIG (or the abbreviations for each of these, such as HKLM). To access a remote registry, prepend the computer name and a colon, as in this example: \\workstation01:HKEY_LOCAL_MACHINE</param>
        /// <param name="SubKey">The name of the subkey (e.g. Software\SomeApplication).</param>
        /// <param name="ValueName">The name of the value to delete. If omitted, the entire SubKey will be deleted. To delete Subkey's default value -- which is the value displayed as "(Default)" by RegEdit -- use the phrase AHK_DEFAULT for this parameter.</param>
        public static void RegDelete(string RootKey, string SubKey, string ValueName)
        {
            try
            {
                string sub = SubKey;

                if (ValueName == null)
                    ToRegKey(RootKey, ref sub, true).DeleteSubKey(sub, true);
                else
                {
                    string val = (ValueName).ToLowerInvariant();
                    if (val == "(default)" || val == "ahk_default")
                        val = string.Empty;
                    ToRegKey(RootKey, ref sub, false).DeleteValue(val, true);
                }
            }
            catch (Exception) { ErrorLevel = 1; }
        }

        /// <summary>
        /// Reads a value from the registry.
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the retrieved value. If the value cannot be retrieved, the variable is made blank and ErrorLevel is set to 1.</param>
        /// <param name="RootKey">Must be either HKEY_LOCAL_MACHINE, HKEY_USERS, HKEY_CURRENT_USER, HKEY_CLASSES_ROOT, or HKEY_CURRENT_CONFIG (or the abbreviations for each of these, such as HKLM). To access a remote registry, prepend the computer name and a colon, as in this example: \\workstation01:HKEY_LOCAL_MACHINE</param>
        /// <param name="SubKey">The name of the subkey (e.g. Software\SomeApplication).</param>
        /// <param name="ValueName">The name of the value to retrieve. If omitted, Subkey's default value will be retrieved, which is the value displayed as "(Default)" by RegEdit. If there is no default value (that is, if RegEdit displays "value not set"), OutputVar is made blank and ErrorLevel is set to 1.</param>
        public static void RegRead(out string OutputVar, string RootKey, string SubKey, string ValueName)
        {
            OutputVar = string.Empty;
            try
            {
                string sub = SubKey, val = ValueName.ToLowerInvariant();
                if (val == "(default)" || val == "ahk_default")
                    val = string.Empty;

                OutputVar = ToRegKey(RootKey, ref sub, false).GetValue(val).ToString();
            }
            catch (Exception) { ErrorLevel = 1; }
        }

        /// <summary>
        /// Writes a value to the registry.
        /// </summary>
        /// <param name="ValueType">Must be either REG_SZ, REG_EXPAND_SZ, REG_MULTI_SZ, REG_DWORD, or REG_BINARY.</param>
        /// <param name="RootKey">Must be either HKEY_LOCAL_MACHINE, HKEY_USERS, HKEY_CURRENT_USER, HKEY_CLASSES_ROOT, or HKEY_CURRENT_CONFIG (or the abbreviations for each of these, such as HKLM). To access a remote registry, prepend the computer name and a colon, as in this example: \\workstation01:HKEY_LOCAL_MACHINE</param>
        /// <param name="SubKey">The name of the subkey (e.g. Software\SomeApplication). If SubKey does not exist, it is created (along with its ancestors, if necessary). If SubKey is left blank, the value is written directly into RootKey (though some operating systems might refuse to write in HKEY_CURRENT_USER's top level).</param>
        /// <param name="ValueName">The name of the value that will be written to. If blank or omitted, Subkey's default value will be used, which is the value displayed as "(Default)" by RegEdit.</param>
        /// <param name="Value">The value to be written. If omitted, it will default to an empty (blank) string, or 0, depending on ValueType.</param>
        public static void RegWrite(string ValueType, string RootKey, string SubKey, string ValueName, string Value)
        {
            try
            {
                string sub = SubKey, val = (ValueName).ToLowerInvariant();
                if (val == "(default)" || val == "ahk_default")
                    val = string.Empty;

                ToRegKey(RootKey, ref sub, false).SetValue(val, Value);
            }
            catch (Exception) { ErrorLevel = 1; }
        }
    }
}