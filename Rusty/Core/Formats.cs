using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32;

namespace IronAHK.Rusty
{
    partial class Core
    {
        static bool OnOff(ref bool? state, string mode)
        {
            switch (mode.ToLowerInvariant())
            {
                case "1":
                case Keyword_On:
                    state = true;
                    break;

                case "0":
                case Keyword_Off:
                    state = false;
                    break;

                default:
                    return false;
            }

            return true;
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

            foreach (char flag in set)
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
            StringBuilder str = new StringBuilder(9);

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
                List<string> dirlist = new List<string>();
                foreach (string file in filelist)
                {
                    string parent = Path.GetDirectoryName(file);
                    if (!dirlist.Contains(parent)) dirlist.Add(parent);
                }
                string[] dirarray = dirlist.ToArray();

                if (files)
                {
                    string[] merge = new string[dirarray.Length + filelist.Length];
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

        static string ToRGB(Color col)
        {
            StringBuilder hex = new StringBuilder(8);
            const string s = "x";
            const char p = '0';
            hex.Append(p);
            hex.Append(s);
            if (col.R < 0x10)
                hex.Append(p);
            hex.Append(col.R.ToString(s));
            if (col.G < 0x10)
                hex.Append(p);
            hex.Append(col.R.ToString(s));
            if (col.B < 0x10)
                hex.Append(p);
            hex.Append(col.B.ToString(s));
            return hex.ToString();
        }

        static int FromTime(DateTime time)
        {
            const int len = 2;
            const char pad = '0';
            StringBuilder str = new StringBuilder(4 + 2 * 5);
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

            int[] t = new int[7];

            for (int i = 0, k = 0; i < t.Length; i++, k += 2)
            {
                if (!int.TryParse(time.Substring(k, 2), out t[i]))
                    throw new ArgumentOutOfRangeException();
            }
            
            return new DateTime(t[0] * 1000 + t[1], t[2], t[3], t[4], t[5], t[6]);
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
            RegistryKey reg = ToRegRootKey(RootKey);
            
            string[] keys = SubKey.Split('/');
            int j = keys.Length - (Parent ? 1 : 0);
            for (int i = 0; i < keys.Length - (Parent ? 1 : 0); i++)
                reg = reg.OpenSubKey(keys[i].Trim());

            SubKey = Parent ? keys[keys.Length - 1] : string.Empty;

            return reg;
        }

        static RegexOptions ToRegexOptions(string sequence)
        {
            RegexOptions options = RegexOptions.None;

            foreach (char modifier in sequence)
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

        static Process ToProcess(string id)
        {
            int pid = 0;
            Process Proc = new Process();

            if (int.TryParse(id, out pid))
            {
                try
                {
                    Proc = System.Diagnostics.Process.GetProcessById(pid);
                }
                catch {}
                return Proc;
            }
            else
            {
                Process[] localByName = System.Diagnostics.Process.GetProcessesByName(id);
                if (localByName.Length > 0)
                    Proc = localByName[0];
                return Proc;
            }
        }

        static Regex ParseRegEx(string exp)
        {
            Regex mod = new Regex("^[imsxADJUXPS`nra]\\)");
            Match res = mod.Match(exp);
            if (res.Success)
            {
                string seq = res.Groups[0].Value;
                return new Regex(exp.Substring(res.Length), ToRegexOptions(seq.Substring(0, seq.Length - 1)));
            }
            else
                return new Regex(exp);
        }

        static void LV_RowOptions(ref ListViewItem row, string options)
        {
            string[] opts = options.Split(new char[] { ' ', '\t' });
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

        static bool ToggleOption(char mode, bool state)
        {
            switch (mode)
            {
                case '-': state = false; break;
                case '+': state = true; break;
                case '^': state = !state; break;
            }
            return state;
        }

        static Dictionary<char, string> KeyValues(string Options, bool Lowercase, char[] Exceptions)
        {
            var table = new Dictionary<char, string>();
            StringBuilder buf = new StringBuilder();
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

                    foreach (char ex in Exceptions)
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

        static Dictionary<string, char> ParseStateKeys(string Options)
        {
            char mode = '+';
            string key = string.Empty;
            Dictionary<string, char> table = new Dictionary<string, char>();

            foreach (char opt in Options)
            {
                if (char.IsWhiteSpace(opt))
                {
                    key = key.ToLowerInvariant();
                    if (table.ContainsKey(key))
                        table[key] = mode;
                    else
                        table.Add(key, mode);
                    key = string.Empty;
                }
                else
                {
                    if (key.Length == 0 && (opt == '+' || opt == '-' || opt == '^'))
                        mode = opt;
                    else
                        key += opt.ToString();
                }
            }

            return table;
        }

        static List<string> ParseKeys(string Options)
        {
            var list = new List<string>();
            for (int i = 0, j = 0; i < Options.Length; i++)
            {
                if (!char.IsLetterOrDigit(Options, i))
                {
                    list.Add(Options.Substring(j, i).ToLowerInvariant());
                    j = i;
                }
            }
            return list;
        }

        static List<string> ParseKeys(string[] Options)
        {
            return ParseKeys(string.Join(" ", Options));
        }
    }
}
