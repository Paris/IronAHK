using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        List<CodeLine> Read(TextReader source, string name)
        {
            #region Properties

            var list = new List<CodeLine>();
            string code;
            int line = 0;

            var includes = new List<string>();

            #endregion

            while ((code = source.ReadLine()) != null)
            {
                #region Line

                line++;

                if (line == 1 && code.Length > 2 && code[0] == '#' && code[1] == '!')
                    continue;

                string codeTrim = code.TrimStart(Spaces);

                #endregion

                #region Multiline comments

                if (codeTrim.Length > 1 && codeTrim[0] == MultiComA && codeTrim[1] == MultiComB)
                {
                    while ((code = source.ReadLine()) != null)
                    {
                        line++;
                        codeTrim = code.TrimStart(Spaces);
                        if (codeTrim.Length > 1 && codeTrim[0] == MultiComB && codeTrim[1] == MultiComA)
                        {
                            code = codeTrim = codeTrim.Substring(2);
                            break;
                        }
                    }
                    if (code == null)
                        continue;
                }

                #endregion

                #region Directives

                if (code.Length > 1 && code[0] == Directive)
                {
                    if (code.Length < 2)
                        throw new ParseException(ExUnknownDirv, line);
                    
                    char[] delim = new char[Spaces.Length + 1];
                    delim[0] = Multicast;
                    Spaces.CopyTo(delim, 1);
                    string[] sub = code.Split(delim, 2);
                    string[] parts = new string[] { sub[0], sub.Length > 1 ? sub[1] : string.Empty };

                    parts[1] = StripComment(parts[1]).Trim(Spaces);

                    int value;
                    bool numeric = int.TryParse(parts[1], out value);

                    bool next = true;
                    bool include = false;

                    switch (parts[0].Substring(1).ToUpperInvariant())
                    {
                        case "INCLUDE":
                            include = true;
                            goto case "INCLUDEAGAIN";

                        case "INCLUDEAGAIN":
                            bool ignore = false;
                            if (parts[1].Length > 3 && parts[1][0] == '*' && (parts[1][1] == 'i' || parts[1][1] == 'I') && IsSpace(parts[1][2]))
                            {
                                parts[1] = parts[1].Substring(3);
                                ignore = true;
                            }

                            if (include && !includes.Contains(new FileInfo(parts[1]).FullName))
                                break;
                            if (ignore && !File.Exists(parts[1]))
                                break;

                            parts[1] = new FileInfo(parts[1]).FullName;
                            var newlist = Read(new StreamReader(parts[1]), parts[1]);
                            list.AddRange(newlist);
                            if (!includes.Contains(parts[1]))
                                includes.Add(parts[1]);
                            break;

                        case "KEYHISTORY":
                            KeyHistory = numeric ? Math.Min(0, Math.Max(value, 500)) : KeyHistoryDefault;
                            break;

                        case "MAXHOTKEYSPERINTERVAL":
                            MaxHotkeysPerInterval = numeric ? value : MaxHotkeysPerIntervalDefault;
                            break;

                        case "MAXMEM":
                            MaxMem = numeric ? value : MaxMemDefault;
                            break;

                        case "MAXTHREADS":
                            MaxThreads = numeric ? Math.Min(1, Math.Max(value, 255)) : MaxThreadsDefault;
                            break;

                        case "MAXTHREADSBUFFER":
                            switch (parts[1].ToUpperInvariant())
                            {
                                case "ON":
                                    MaxThreadsBuffer = true;
                                    break;
                                case "OFF":
                                    MaxThreadsBuffer = false;
                                    break;
                                default:
                                    MaxThreadsBuffer = MaxThreadsBufferDefault;
                                    break;
                            }
                            break;

                        case "MAXTHREADSPERHOTKEY":
                            MaxThreadsPerHotkey = numeric ? Math.Min(1, Math.Max(value, 20)) : MaxThreadsPerHotkeyDefault;
                            break;

                        case "NOENV":
                            NoEnv = true;
                            break;

                        case "NOTRAYICON":
                            NoTrayIcon = true;
                            break;

                        case "PERSISTENT":
                            Persistent = true;
                            break;

                        case "SINGLEINSTANCE":
                            switch (parts[1].ToUpperInvariant())
                            {
                                case "FORCE":
                                    SingleInstance = true;
                                    break;
                                case "IGNORE":
                                    SingleInstance = null;
                                    break;
                                case "OFF":
                                    SingleInstance = false;
                                    break;
                                default:
                                    break;
                            }
                            break;

                        case "WINACTIVATEFORCE":
                            WinActivateForce = true;
                            break;

                        case "HOTKEYINTERVAL":
                            HotkeyInterval = numeric ? value : HotkeyIntervalDefault;
                            break;

                        case "HOTKEYMODIFIERTIMEOUT":
                            HotkeyModifierTimeout = numeric ? Math.Max(-1, value) : HotkeyModifierTimeoutDefault;
                            break;

                        case "HOTSTRING":
                            switch (parts[1].ToUpperInvariant())
                            {
                                case "NOMOUSE":
                                    HotstringNoMouse = true;
                                    break;
                                case "ENDCHARS":
                                    HotstringEndChars = parts[1];
                                    break;
                                case "NEWCHARS":
                                    HotstringNewChars = parts[1];
                                    break;
                                default:
                                    next = false;
                                    break;
                            }
                            break;

                        case "ALLOWSAMELINECOMMENTS":
                        case "ERRORSTDOUT":
                        case "INSTALLKEYBDHOOK":
                        case "INSTALLMOUSEHOOK":
                        case "USEHOOK":
                            // deprecated directives
                            break;

                        default:
                            next = false;
                            break;
                    }

                    if (next)
                        continue;
                }

                #endregion

                #region Mulitline strings

                if (codeTrim.Length > 0 && codeTrim[0] == ParenOpen)
                {
                    if (list.Count == 0)
                        throw new ParseException(ExUnexpected, line);

                    var buf = new StringBuilder(code.Length);
                    buf.Append(code);
                    buf.Append(Environment.NewLine);

                    while ((code = source.ReadLine()) != null)
                    {
                        codeTrim = code.TrimStart(Spaces);

                        if (codeTrim.Length > 0 && codeTrim[0] == ParenClose)
                        {
                            code = codeTrim = codeTrim.Substring(1);
                            buf.Append(ParenClose);
                            break;
                        }
                        else
                        {
                            buf.Append(code);
                            buf.Append(Environment.NewLine);
                        }
                    }

                    string str = buf.ToString();
                    string result = MultilineString(str);
                    list[list.Count - 1].Code += result + code;
                    continue;
                }

                #endregion

                #region Statement

                if (codeTrim.Length == 0)
                    continue;

                code = code.Trim(Spaces);
                Translate(ref code);

                if (code.Length == 0)
                    continue;
                else if (IsCommentLine(code))
                    continue;
                if (IsContinuationLine(code))
                {
                    if (list.Count == 0)
                        throw new ParseException(ExUnexpected, line);

                    int i = list.Count - 1;
                    var buf = new StringBuilder(list[i].Code, list[i].Code.Length + Environment.NewLine.Length + code.Length);
                    buf.Append(Environment.NewLine);
                    buf.Append(code);
                    list[i].Code = buf.ToString();
                }
                else
                    list.Add(new CodeLine(name, line, code));

                #endregion
            }

            return list;
        }
    }
}
