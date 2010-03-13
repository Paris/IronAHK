using System.CodeDom;

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
        bool? SingleInstance = null;
        bool WinActivateForce = WinActivateForceDefault;
        bool HotstringNoMouse = false;
        string HotstringEndChars = string.Empty;
        string HotstringNewOptions = string.Empty;
        bool LTrimForced = false;

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

            char[] delim = new char[Spaces.Length + 1];
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
                sub = new string[] { string.Empty, string.Empty };
            }
            else
            {
                numeric = int.TryParse(parts[1], out value);
                string[] split = parts[1].Split(new char[] { Multicast }, 2);
                sub = new string[] { split[0].Trim(Spaces), split.Length > 1 ? split[1].Trim(Spaces) : string.Empty };
            }

            string cmd = parts[0].Substring(1);
            const string res = "__IFWIN\0";

            switch (cmd.ToUpperInvariant())
            {
                case "CLIPBOARDTIMEOUT":
                    ClipboardTimeout = numeric ? value : ClipboardTimeoutDefault;
                    break;

                case "COMMENTFLAG":
                    Comment = parts[1];
                    break;

                case "ESCAPECHAR":
                    Escape = parts[1][0];
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
    }
}
