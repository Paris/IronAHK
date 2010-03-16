using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Core
    {
        #region Delegates

        /// <summary>
        /// A function.
        /// </summary>
        /// <param name="args">Parameters.</param>
        /// <returns>A value.</returns>
        public delegate object GenericFunction(object[] args);

        #endregion

        #region Error

        [ThreadStatic]
        static int error;

        /// <summary>
        /// Indicates the success or failure of a command.
        /// </summary>
        static public int ErrorLevel
        {
            get { return error; }
            set { error = value; }
        }

        #endregion

        #region Hooks

        static Dictionary<string, object> variables;

        static Dictionary<string, HotkeyDefinition> hotkeys;

        static Dictionary<string, HotstringDefinition> hotstrings;

        static GenericFunction keyCondition;

        static KeyboardHook keyboardHook;

        static bool suspended = false;

        [ThreadStatic]
        static int? _KeyDelay;

        [ThreadStatic]
        static int? _KeyPressDuration;

        [ThreadStatic]
        static int? _MouseDelay;

        [ThreadStatic]
        static int? _DefaultMouseSpeed;

        #endregion

        #region Guis

        static Dictionary<string, BaseGui.Window> guis;

        [ThreadStatic]
        static string defaultGui;

        static NotifyIcon Tray;

        #endregion

        #region RunAs

        [ThreadStatic]
        static string runUser;

        [ThreadStatic]
        static System.Security.SecureString runPassword;

        [ThreadStatic]
        static string runDomain;

        #endregion

        #region Windows

        [ThreadStatic]
        static int? _ControlDelay;

        [ThreadStatic]
        static int? _WinDelay;

        [ThreadStatic]
        static bool? _DetectHiddenText;

        [ThreadStatic]
        static bool? _DetectHiddenWindows;

        [ThreadStatic]
        static int? _TitleMatchMode;

        [ThreadStatic]
        static bool? _TitleMatchModeSpeed;

        #endregion

        #region Strings

        [ThreadStatic]
        static StringComparison? _StringCaseSense;

        [ThreadStatic]
        static string _FormatFloat;

        [ThreadStatic]
        static char? _FormatInteger;

        [ThreadStatic]
        static string _UserAgent;

        #endregion

        #region Misc

        static Dictionary<string, System.Timers.Timer> timers;

        static EventHandler onExit;

        const int LoopFrequency = 50;

        #endregion
    }
}
