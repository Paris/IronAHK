using System;
using System.IO;
using System.Text;

namespace IronAHK.Rusty
{
    partial class Core
    {
        /// <summary>
        /// Deletes a value from a standard format .ini file.
        /// </summary>
        /// <param name="file">The name of the file.</param>
        /// <param name="section">The section name.</param>
        /// <param name="key">The key name. If omitted, the entire <paramref name="section"/> will be deleted.</param>
        public static void IniDelete(string file, string section, string key = null)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                ErrorLevel = WindowsAPI.WritePrivateProfileString(section, key.Length == 0 ? null : key, null, file) ? 0 : 1;
                return;
            }

            IniWrite(null, file, section, key);
        }

        /// <summary>
        /// Reads a value from a standard format .ini file.
        /// </summary>
        /// <param name="result">The variable to store the result.</param>
        /// <param name="file">The name of the file.</param>
        /// <param name="section">The section name.</param>
        /// <param name="key">The key name.</param>
        /// <param name="error">The value to store in <paramref name="result"/> if the specified <paramref name="key"/> is not found.
        /// By default this is "ERROR".</param>
        public static void IniRead(out string result, string file, string section, string key, string error = "ERROR")
        {
            result = error ?? string.Empty;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                var buf = new StringBuilder(char.MaxValue);
                WindowsAPI.GetPrivateProfileString(section, key, error, buf, (uint)buf.Capacity, file);
                result = buf.ToString();
            }

            if (!File.Exists(file) || string.IsNullOrEmpty(key))
                return;

            var within = string.IsNullOrEmpty(section);
            section = string.Format(Keyword_IniSectionOpen + "{0}]", section ?? string.Empty);

            var reader = new StreamReader(file);
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                var trimmed = line.Trim();

                if (trimmed.StartsWith(section))
                    within = true;
                else if (trimmed.StartsWith(Keyword_IniSectionOpen))
                    within = false;

                if (!within || !trimmed.StartsWith(key))
                    continue;

                trimmed = trimmed.Substring(key.Length).Trim();

                if (trimmed.Length == 0 || trimmed[0] != Keyword_IniKeyAssign)
                    continue;

                result = trimmed.Substring(1);
                break;
            }
        }

        /// <summary>
        /// Writes a value to a standard format .ini file.
        /// </summary>
        /// <param name="value">The string or number that will be written to the right of <paramref name="key"/>'s equal sign (=).</param>
        /// <param name="file">The name of the file.</param>
        /// <param name="section">The section name.</param>
        /// <param name="key">The key name.</param>
        /// <remarks><see cref="ErrorLevel"/> is set to <c>1</c> if there was a problem or <c>0</c> otherwise.</remarks>
        public static void IniWrite(string value, string file, string section, string key)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                ErrorLevel = WindowsAPI.WritePrivateProfileString(section, key, value, file) ? 0 : 1;
                return;
            }

            if (!File.Exists(file))
            {
                ErrorLevel = 1;
                return;
            }

            var within = string.IsNullOrEmpty(section); // already within blank section
            var written = false;
            section = string.Format(Keyword_IniSectionOpen + "{0}]", section ?? string.Empty);
            var sec = string.IsNullOrEmpty(key);

            var reader = new StreamReader(file);
            var writer = new StringWriter();
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                var trimmed = line.Trim();

                if (trimmed.StartsWith(section, StringComparison.OrdinalIgnoreCase))
                    within = true;
                else if (trimmed.StartsWith(Keyword_IniSectionOpen))
                {
                    if (within && !written)
                    {
                        if (value != null)
                            writer.WriteLine("{0} {1} {2}", key, Keyword_IniKeyAssign.ToString(), value);
                        written = true;
                    }
                    within = false;
                }

                if (sec && within)
                    continue;

                if (!sec && trimmed.StartsWith(key, StringComparison.OrdinalIgnoreCase))
                {
                    var post = trimmed.Substring(key.Length).Trim();

                    if (post.Length != 0 && post[0] == Keyword_IniKeyAssign)
                    {
                        if (value != null)
                            writer.WriteLine("{0} {1} {2}", key, Keyword_IniKeyAssign.ToString(), value);
                        written = true;
                        continue;
                    }
                }

                writer.WriteLine(line);
            }

            reader.Close();
            writer.Flush();
            var text = writer.ToString();

            try
            {
                File.Delete(file);
                File.WriteAllText(file, text);
                ErrorLevel = 0;
            }
            catch (IOException)
            {
                ErrorLevel = 1;
            }
        }
    }
}
