using System;
using System.CodeDom;
using System.Reflection;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        #region Variables

        const int ClipboardTimeoutDefault = 1000;
        const bool NoEnvDefault = false;
        const bool NoTrayIconDefault = false;
        const bool PersistentDefault = false;
        const bool WinActivateForceDefault = false;

        #pragma warning disable 414

        int ClipboardTimeout = ClipboardTimeoutDefault;
        bool NoEnv = NoEnvDefault;
        bool NoTrayIcon = NoTrayIconDefault;
        bool Persistent = PersistentDefault;
        bool? SingleInstance;
        bool WinActivateForce = WinActivateForceDefault;
        bool HotstringNoMouse;
        string HotstringEndChars = string.Empty;
        string HotstringNewOptions = string.Empty;
        bool LTrimForced;

        string IfWinActive_WinTitle = string.Empty;
        string IfWinActive_WinText = string.Empty;
        string IfWinExist_WinTitle = string.Empty;
        string IfWinExist_WinText = string.Empty;
        string IfWinNotActive_WinTitle = string.Empty;
        string IfWinNotActive_WinText = string.Empty;
        string IfWinNotExist_WinTitle = string.Empty;
        string IfWinNotExist_WinText = string.Empty;

        #pragma warning restore 414

        #endregion

        void ParseDirective(string code)
        {
            if (code.Length < 2)
                throw new ParseException(ExUnknownDirv);

            var delim = new char[Spaces.Length + 1];
            delim[0] = Multicast;
            Spaces.CopyTo(delim, 1);
            string[] parts = code.Split(delim, 2);

            if (parts.Length != 2)
                parts = new[] { parts[0], string.Empty };

            parts[1] = StripComment(parts[1]).Trim(Spaces);

            int value = default(int);
            bool numeric;
            string[] sub;

            if (parts[1].Length == 0)
            {
                numeric = false;
                sub = new[] { string.Empty, string.Empty };
            }
            else
            {
                numeric = int.TryParse(parts[1], out value);
                string[] split = parts[1].Split(new[] { Multicast }, 2);
                sub = new[] { split[0].Trim(Spaces), split.Length > 1 ? split[1].Trim(Spaces) : string.Empty };
            }

            string cmd = parts[0].Substring(1);
            const string res = "__IFWIN\0";

            switch (cmd.ToUpperInvariant())
            {
                #region Assembly manifest

                case "ASSEMBLYTITLE":
                    if (!string.IsNullOrEmpty(parts[1]))
                        AddAssemblyAttribute(typeof(AssemblyTitleAttribute), parts[1]);
                    break;

                case "ASSEMBLYDESCRIPTION":
                    if (!string.IsNullOrEmpty(parts[1]))
                        AddAssemblyAttribute(typeof(AssemblyDescriptionAttribute), parts[1]);
                    break;

                case "ASSEMBLYCONFIGURATION":
                    if (!string.IsNullOrEmpty(parts[1]))
                        AddAssemblyAttribute(typeof(AssemblyConfigurationAttribute), parts[1]);
                    break;

                case "ASSEMBLYCOMPANY":
                    if (!string.IsNullOrEmpty(parts[1]))
                        AddAssemblyAttribute(typeof(AssemblyCompanyAttribute), parts[1]);
                    break;

                case "ASSEMBLYPRODUCT":
                    if (!string.IsNullOrEmpty(parts[1]))
                        AddAssemblyAttribute(typeof(AssemblyProductAttribute), parts[1]);
                    break;

                case "ASSEMBLYCOPYRIGHT":
                    if (!string.IsNullOrEmpty(parts[1]))
                        AddAssemblyAttribute(typeof(AssemblyCopyrightAttribute), parts[1]);
                    break;

                case "ASSEMBLYTRADEMARK":
                    if (!string.IsNullOrEmpty(parts[1]))
                        AddAssemblyAttribute(typeof(AssemblyTrademarkAttribute), parts[1]);
                    break;

                case "ASSEMBLYCULTURE":
                    if (!string.IsNullOrEmpty(parts[1]))
                        AddAssemblyAttribute(typeof(AssemblyCultureAttribute), parts[1]);
                    break;

                case "ASSEMBLYVERSION":
                    if (!string.IsNullOrEmpty(parts[1]))
                        AddAssemblyAttribute(typeof(AssemblyVersionAttribute), parts[1]);
                    break;

                case "ASSEMBLYMERGE":
                    if (CompilerParameters is IACompilerParameters)
                    {
                        var options = (IACompilerParameters)CompilerParameters;
                        options.Merge = true;

                        switch (parts[1].ToUpperInvariant())
                        {
                            case "FORCE":
                                options.MergeFallbackToLink = false;
                                break;

                            case "OFF":
                                options.Merge = false;
                                break;
                        }
                    }
                    break;

                #endregion

                case "CLIPBOARDTIMEOUT":
                    ClipboardTimeout = numeric ? value : ClipboardTimeoutDefault;   
                    break;

                case "COMMENTFLAG":
                    if (parts[1].Length == 2 && parts[1][0] == MultiComA && parts[1][1] == MultiComB)
                        throw new ParseException(ExIllegalCommentFlag);
                    Comment = parts[1];
                    break;

                case "DEREFCHAR":
                    Resolve = parts[1][0];
                    break;

                case "ESCAPECHAR":
                    Escape = parts[1][0];
                    break;

                case "DELIMITER":
                    Multicast = parts[1][0];
                    break;

                case "HOTSTRING":
                    HotstringNewOptions = parts[1];
                    break;

                case "IFWINACTIVE":
                    IfWinActive_WinTitle = sub[0];
                    IfWinActive_WinText = sub[1];
                    goto case res;

                case "IFWINEXIST":
                    IfWinExist_WinTitle = sub[0];
                    IfWinExist_WinText = sub[1];
                    goto case res;

                case "IFWINNOTACTIVE":
                    IfWinNotExist_WinTitle = sub[0];
                    IfWinNotActive_WinText = sub[1];
                    goto case res;

                case "IFWINNOTEXIST":
                    IfWinNotExist_WinTitle = sub[0];
                    IfWinNotExist_WinText = sub[1];
                    goto case res;

                case res:
                    var cond = (CodeMethodInvokeExpression)InternalMethods.Hotkey;
                    cond.Parameters.Add(new CodePrimitiveExpression(cmd));
                    cond.Parameters.Add(new CodePrimitiveExpression(sub[0]));
                    cond.Parameters.Add(new CodePrimitiveExpression(sub[1]));
                    prepend.Add(cond);
                    break;

                case "LTRIM":
                    switch (sub[0].ToUpperInvariant())
                    {
                        case "":
                        case "ON":
                            LTrimForced = true;
                            break;

                        case "OFF":
                            LTrimForced = false;
                            break;

                        default:
                            throw new ParseException("Directive parameter must be either \"on\" or \"off\"");
                    }
                    break;

                default:
                    throw new ParseException(ExUnknownDirv);
            }
        }

        void AddAssemblyAttribute(Type attribute, object value)
        {
            var type = new CodeTypeReference(attribute);
            type.UserData.Add(RawData, attribute);
            var arg = new CodeAttributeArgument(new CodePrimitiveExpression(value));
            var dec = new CodeAttributeDeclaration(type, arg);
            assemblyAttributes.Add(dec);
        }
    }
}
