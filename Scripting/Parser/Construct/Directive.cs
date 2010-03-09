
namespace IronAHK.Scripting
{
    partial class Parser
    {
        #region Variables

        const int ClipboardTimeoutDefault = 1000;
        const int HotkeyIntervalDefault = 2000;
        const int HotkeyModifierTimeoutDefault = 50;
        const int MaxHotkeysPerIntervalDefault = 70;
        const int MaxMemDefault = 64;
        const int KeyHistoryDefault = 40;
        const int MaxThreadsDefault = 10;
        const bool MaxThreadsBufferDefault = false;
        const int MaxThreadsPerHotkeyDefault = 1;
        const bool NoEnvDefault = false;
        const bool NoTrayIconDefault = false;
        const bool PersistentDefault = false;
        const bool WinActivateForceDefault = false;

        #pragma warning disable 414

        int ClipboardTimeout = ClipboardTimeoutDefault;
        int HotkeyInterval = HotkeyIntervalDefault;
        int HotkeyModifierTimeout = HotkeyModifierTimeoutDefault;
        int MaxHotkeysPerInterval = MaxHotkeysPerIntervalDefault;
        int MaxMem = MaxMemDefault;
        int KeyHistory = KeyHistoryDefault;
        int MaxThreads = MaxThreadsDefault;
        bool MaxThreadsBuffer = MaxThreadsBufferDefault;
        int MaxThreadsPerHotkey = MaxThreadsPerHotkeyDefault;
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
                throw new ParseException(ExFlowArgReq);

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

            switch (parts[0].Substring(1).ToUpperInvariant())
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
                    break;

                case "IFWINEXIST":
                    IfWinExist_WinTitle = sub[0];
                    IfWinExist_WinText = sub[1];
                    break;

                case "IFWINNOTACTIVE":
                    IfWinNotExist_WinTitle = sub[0];
                    IfWinNotActive_WinText = sub[1];
                    break;

                case "IFWINNOTEXIST":
                    IfWinNotExist_WinTitle = sub[0];
                    IfWinNotExist_WinText = sub[1];
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
