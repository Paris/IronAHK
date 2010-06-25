using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32;

namespace IronAHK.Rusty
{
    partial class Core
    {
        static object SafeInvoke(string name, params object[] args)
        {
            var method = FindLocalRoutine(name);

            if (method == null)
                return null;

            object value = null;

            try { value = method.Invoke(null, new object[] { args }); }
            catch (Exception) { }
            
            return value;
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

        static bool? OnOff(string mode)
        {
            switch (mode.ToLowerInvariant())
            {
                case Keyword_On:
                case "1":
                    return true;

                case Keyword_Off:
                case "0":
                    return false;

                default:
                    return null;
            }
        }

        static bool IsAnyBlank(params string[] args)
        {
            foreach (var str in args)
                if (string.IsNullOrEmpty(str))
                    return true;

            return false;
        }

        static string GuiId(ref string command)
        {
            string id = DefaultGuiId;

            if (command.Length == 0)
                return id;

            int z = command.IndexOf(':');
            string pre = string.Empty;

            if (z != -1)
            {
                pre = command.Substring(0, z).Trim();
                z++;
                command = z == command.Length ? string.Empty : command.Substring(z);
            }

            return pre.Length == 0 ? id : pre;
        }

        static Icon GetIcon(string source, int n)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                var prc = System.Diagnostics.Process.GetCurrentProcess().Handle;
                var icon = Windows.ExtractIcon(prc, source, n);

                if (icon != IntPtr.Zero)
                    return Icon.FromHandle(icon);
            }

            return Icon.ExtractAssociatedIcon(source);
        }

        static string ToString(byte[] array)
        {
            var buf = new StringBuilder(array.Length * 2);

            foreach (var b in array)
                buf.Append(b.ToString("x").PadLeft(2, '0'));

            return buf.ToString();
        }

        static byte[] ToByteArray(object value)
        {
            if (value is string)
                return Encoding.Unicode.GetBytes((string)value);
            
            if (value is byte[])
                return (byte[])value;

            if (value == null)
                return new byte[] { };

            var formatter = new BinaryFormatter();
            var writer = new MemoryStream();
            formatter.Serialize(writer, value);
            return writer.ToArray();
        }

        static byte[] Encrypt(object value, object key, bool decrypt, SymmetricAlgorithm alg)
        {
            int size = 0;

            foreach (var legal in alg.LegalKeySizes)
                size = Math.Max(size, legal.MaxSize);

            var k = new byte[size / 8];

            var keyBytes = ToByteArray(key);

            if (keyBytes.Length < k.Length)
            {
                var padded = new byte[k.Length];
                keyBytes.CopyTo(padded, 0);
                keyBytes = padded;
            }

            for (int i = 0; i < k.Length; i++)
                k[i] = keyBytes[i];

            try
            {
                alg.Key = k;
            }
            catch (CryptographicException)
            {
                ErrorLevel = 2;
                return new byte[] { };
            }

            var iv = new byte[alg.IV.Length];
            var hash = new SHA1Managed().ComputeHash(keyBytes, 0, iv.Length);

            for (int i = 0; i < Math.Min(iv.Length, hash.Length); i++)
                iv[i] = hash[i];

            alg.IV = iv;

            var trans = decrypt ? alg.CreateDecryptor() : alg.CreateEncryptor();
            var buffer = ToByteArray(value);
            var result = trans.TransformFinalBlock(buffer, 0, buffer.Length);
            return result;
        }

        static string Hash(object value, HashAlgorithm alg)
        {
            var raw = ToByteArray(value);
            var result = alg.ComputeHash(raw);
            return ToString(result);
        }

        static Color ParseColor(string name)
        {
            name = name.Trim();

            if (name.Length > 0 && name[0] == '#')
                name = name.Substring(1);
            else if (name.Length > 1 && name[0] == '0' && (name[1] == 'x' || name[1] == 'X'))
                name = name.Substring(2);

            if (name.Length != 8 || name.Length != 6)
                return Color.FromName(name);

            var argb = new[] { 0, 0, 0, 0 };

            for (int i = 0; i < argb.Length; i++)
            {
                int n = i * 2;
                var c = new string(new[] { '0', 'x', name[n], name[n + 1] });
                int.TryParse(c, out argb[i]);
            }

            return Color.FromArgb(argb[3], argb[0], argb[1], argb[2]);
        }

        static Font ParseFont(string family, string styles)
        {
            int size = 1;
            var display = FontStyle.Regular;
            string[] options = ParseOptions(styles);

            for (int i = 0; i < options.Length ; i++)
            {
                string mode = options[i].ToLowerInvariant();

                switch (mode)
                {
                    case Keyword_Bold: display |= FontStyle.Bold; break;
                    case Keyword_Italic: display |= FontStyle.Italic; break;
                    case Keyword_Strike: display |= FontStyle.Strikeout; break;
                    case Keyword_Underline: display |= FontStyle.Underline; break;
                    case Keyword_Norm: display = FontStyle.Regular; break;

                    default:
                        if (mode.Length < 2)
                            break;

                        string prop = mode.Substring(1).Trim();
                        int n;

                        switch (mode[0])
                        {
                            case 'c':
                                break;

                            case 's':
                                if (int.TryParse(prop, out n))
                                    size = n;
                                break;

                            case 'w':
                                if (int.TryParse(prop, out n))
                                {
                                    if (n <= 400)
                                        display &= ~FontStyle.Bold;
                                    else if (n >= 700)
                                        display |= FontStyle.Bold;
                                }
                                break;
                        }
                        break;
                }
            }

            return new Font(family, size, display);
        }

        static string ToYYYYMMDDHH24MISS(DateTime time)
        {
            return time.ToString("yyyyMMddHHmmss");
        }

        static string ToOSType(PlatformID id)
        {
            switch (id)
            {
                case PlatformID.MacOSX: return "MACOSX";
                case PlatformID.Unix: return "UNIX";
                case PlatformID.Win32NT: return "WIN32_NT";
                case PlatformID.Win32S: return "WIN32_S";
                case PlatformID.Win32Windows: return "WIN32_WINDOWS";
                case PlatformID.WinCE: return "WINCE";
                case PlatformID.Xbox: return "XBOX";
                default: return "UNKNOWN";
            }
        }

        static string ToStringCaseSense(StringComparison type)
        {
            switch (type)
            {
                case StringComparison.CurrentCultureIgnoreCase: return Keyword_Locale;
                case StringComparison.Ordinal: return Keyword_On;
                case StringComparison.OrdinalIgnoreCase: return Keyword_Off;
                default: return Keyword_Off;
            }
        }

        static FileAttributes ToFileAttribs(string set, FileAttributes attribs)
        {
            char state = '+';

            foreach (var flag in set)
            {
                FileAttributes applied = FileAttributes.Normal;

                switch (flag)
                {
                    case '+':
                    case '-':
                    case '^':
                        state = flag;
                        continue;

                    case 'r':
                    case 'R':
                        applied = FileAttributes.ReadOnly;
                        break;

                    case 'a':
                    case 'A':
                        applied = FileAttributes.Archive;
                        break;

                    case 's':
                    case 'S':
                        applied = FileAttributes.System;
                        break;

                    case 'h':
                    case 'H':
                        applied = FileAttributes.Hidden;
                        break;

                    case 'n':
                    case 'N':
                        applied = FileAttributes.Normal;
                        break;

                    case 'd':
                    case 'D':
                        applied = FileAttributes.Directory;
                        break;

                    case 'o':
                    case 'O':
                        applied = FileAttributes.Offline;
                        break;

                    case 'c':
                    case 'C':
                        applied = FileAttributes.Compressed;
                        break;

                    case 't':
                    case 'T':
                        applied = FileAttributes.Temporary;
                        break;
                }

                switch (state)
                {
                    case '-':
                        attribs &= ~applied;
                        break;

                    case '^':
                        if ((attribs & applied) == applied)
                            attribs &= ~applied;
                        else
                            attribs |= applied;
                        break;

                    default:
                        attribs |= applied;
                        break;
                }

                state = '+';
            }

            return attribs;
        }

        static string FromFileAttribs(FileAttributes attribs)
        {
            var str = new StringBuilder(9);

            if ((attribs & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                str.Append('R');
            if ((attribs & FileAttributes.Archive) == FileAttributes.Archive)
                str.Append('A');
            if ((attribs & FileAttributes.System) == FileAttributes.System)
                str.Append('S');
            if ((attribs & FileAttributes.Hidden) == FileAttributes.Hidden)
                str.Append('H');
            if ((attribs & FileAttributes.Normal) == FileAttributes.Normal)
                str.Append('N');
            if ((attribs & FileAttributes.Directory) == FileAttributes.Directory)
                str.Append('D');
            if ((attribs & FileAttributes.Offline) == FileAttributes.Offline)
                str.Append('O');
            if ((attribs & FileAttributes.Compressed) == FileAttributes.Compressed)
                str.Append('C');
            if ((attribs & FileAttributes.Temporary) == FileAttributes.Temporary)
                str.Append('T');

            return str.ToString();
        }

        static string[] ToFiles(string path, bool files, bool dirs, bool recurse)
        {
            string[] filelist = Directory.GetFiles(Path.GetDirectoryName(path), Path.GetFileName(path),
                recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            if (dirs)
            {
                var dirlist = new List<string>();
                foreach (var file in filelist)
                {
                    string parent = Path.GetDirectoryName(file);
                    if (!dirlist.Contains(parent)) dirlist.Add(parent);
                }
                string[] dirarray = dirlist.ToArray();

                if (files)
                {
                    var merge = new string[dirarray.Length + filelist.Length];
                    int i;

                    for (i = 0; i < filelist.Length; i++)
                        merge[i] = filelist[i];

                    for (int j = 0; j < dirarray.Length; j++)
                        merge[i + j] = dirarray[j];

                    return merge;
                }
                else return dirarray;
            }
            else if (files)
                return filelist;

            return new string[] { };
        }

        static int FromTime(DateTime time)
        {
            const int len = 2;
            const char pad = '0';
            var str = new StringBuilder(4 + 2 * 5);
            str.Append(time.Year.ToString().PadLeft(len * 2, pad));
            str.Append(time.Month.ToString().PadLeft(len, pad));
            str.Append(time.Day.ToString().PadLeft(len, pad));
            str.Append(time.Hour.ToString().PadLeft(len, pad));
            str.Append(time.Minute.ToString().PadLeft(len, pad));
            str.Append(time.Second.ToString().PadLeft(len, pad));
            return int.Parse(str.ToString());
        }

        static DateTime ToDateTime(string time)
        {
            if (time.Length > 7)
                return DateTime.Now;

            int[] t = { DateTime.Now.Year / 100, DateTime.Now.Year % 100, 1, 1, 0, 0, 0, 0 };

            for (int i = 0, k = 0; i < t.Length; i++, k += 2)
            {
                if (k + 1 >= time.Length || !int.TryParse(time.Substring(k, 2), out t[i]))
                    break;
            }

            return new DateTime(t[0] * 100 + t[1], t[2], t[3], t[4], t[5], t[6]);
        }

        static RegistryKey ToRegRootKey(string name)
        {
            switch (name.ToLowerInvariant())
            {
                case Keyword_HKey_Local_Machine:
                case Keyword_HKLM:
                    return Registry.LocalMachine;

                case Keyword_HKey_Users:
                case Keyword_HKU:
                    return Registry.Users;

                case Keyword_HKey_Current_User:
                case Keyword_HKCU:
                    return Registry.CurrentUser;

                case Keyword_HKey_Classes_Root:
                case Keyword_HKCR:
                    return Registry.ClassesRoot;

                case Keyword_HKey_Current_Config:
                case Keyword_HKCC:
                    return Registry.CurrentConfig;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        static RegistryKey ToRegKey(string RootKey, ref string SubKey, bool Parent)
        {
            var reg = ToRegRootKey(RootKey);
            
            string[] keys = SubKey.Split('/');
            for (int i = 0; i < keys.Length - (Parent ? 1 : 0); i++)
                reg = reg.OpenSubKey(keys[i].Trim());

            SubKey = Parent ? keys[keys.Length - 1] : string.Empty;

            return reg;
        }

        static RegexOptions ToRegexOptions(string sequence)
        {
            var options = RegexOptions.None;

            foreach (var modifier in sequence)
            {
                switch (modifier)
                {
                    case 'i':
                    case 'I':
                        options |= RegexOptions.IgnoreCase;
                        break;

                    case 'm':
                    case 'M':
                        options |= RegexOptions.Multiline;
                        break;

                    case 's':
                    case 'S':
                        options |= RegexOptions.Singleline;
                        break;

                    case 'x':
                    case 'X':
                        options |= RegexOptions.IgnorePatternWhitespace;
                        break;
                }
            }

            return options;
        }

        static Process FindProcess(string name)
        {
            int id;

            if (int.TryParse(name, out id))
                return System.Diagnostics.Process.GetProcessById(id);

            var prc = System.Diagnostics.Process.GetProcessesByName(name);
            return prc.Length > 0 ? prc[0] : null;
        }

        static Regex ParseRegEx(string exp, bool reverse)
        {
            var mod = new Regex("^[imsxADJUXPS`nra]\\)");
            var res = mod.Match(exp);
            var opts = reverse ? RegexOptions.RightToLeft : RegexOptions.None;

            if (res.Success)
            {
                string seq = res.Groups[0].Value;
                opts |= ToRegexOptions(seq.Substring(0, seq.Length - 1));
                return new Regex(exp.Substring(res.Length), opts);
            }
            else
                return new Regex(exp, opts);
        }

        static void LV_RowOptions(ref ListViewItem row, string options)
        {
            string[] opts = options.Split(new[] { ' ', '\t' });

            for (int i = 0; i < opts.Length; i++)
            {
                bool enable = true;
                char state = opts[i][0];
                if (state == '-')
                    enable = false;
                if (!enable || state == '+')
                    opts[i] = opts[i].Substring(1);

                string mode = opts[i].Trim().ToLowerInvariant();
                switch (opts[i].Trim().ToLowerInvariant())
                {
                    case Keyword_Check: row.Checked = enable; break;
                    case Keyword_Focus: row.Focused = enable; break;
                    case Keyword_Icon: row.ImageIndex = int.Parse(mode.Substring(4)); break;
                    case Keyword_Select: row.Selected = enable; break;
                    case Keyword_Vis: row.EnsureVisible(); break;
                }
            }
        }

        static void LV_ColOptions(ref ColumnHeader col, string options)
        {
            
        }

        static void TV_NodeOptions(ref TreeNode node, string options)
        {

        }

        static Dictionary<char, string> KeyValues(string Options, bool Lowercase, char[] Exceptions)
        {
            var table = new Dictionary<char, string>();
            var buf = new StringBuilder();
            int i = 0;
            bool exp = false;
            char key = default(char);
            string val;

            if (Lowercase)
                for (i = 0; i < Exceptions.Length; i++)
                    Exceptions[i] = char.ToLowerInvariant(Exceptions[i]);
            i = 0;

            while (i < Options.Length)
            {
                char sym = Options[i];
                i++;
                if (char.IsWhiteSpace(sym) || i == Options.Length)
                {
                    if (buf.Length == 0)
                        continue;

                    if (exp)
                    {
                        exp = false;
                        val = buf.ToString();
                        buf.Length = 0;
                        if (table.ContainsKey(key))
                            table[key] = val;
                        else
                            table.Add(key, val);
                        continue;
                    }

                    key = buf[0];
                    if (Lowercase)
                        key = char.ToLowerInvariant(key);

                    foreach (var ex in Exceptions)
                        if (key == ex)
                        {
                            exp = true;
                            buf.Length = 0;
                            continue;
                        }

                    val = buf.Length > 1 ? buf.Remove(0, 1).ToString() : string.Empty;

                    if (table.ContainsKey(key))
                        table[key] = val;
                    else
                        table.Add(key, val);

                    buf.Length = 0;
                }
                else
                    buf.Append(sym);
            }

            if (exp && key != default(char))
            {
                if (table.ContainsKey(key))
                    table[key] = null;
                else
                    table.Add(key, null);
            }

            return table;
        }

        static string[] ParseOptions(string options)
        {
            return options.Split(Keyword_Spaces, StringSplitOptions.RemoveEmptyEntries);
        }

        static bool IsOption(string options, string search)
        {
            if (string.IsNullOrEmpty(options) || string.IsNullOrEmpty(search))
                return false;

            return options.Trim().Equals(search, StringComparison.OrdinalIgnoreCase);
        }
    }
}
