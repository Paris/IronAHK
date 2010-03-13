using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Core
    {
        // TODO: organise Properties.cs

        /// <summary>
        /// A function.
        /// </summary>
        /// <param name="args">Parameters.</param>
        /// <returns>A value.</returns>
        public delegate object GenericFunction(object[] args);

        [ThreadStatic]
        static int error = 0;

        /// <summary>
        /// Indicates the success or failure of some of the command.
        /// </summary>
        static public int ErrorLevel
        {
            get { return error; }
            set { error = value; }
        }

        static Dictionary<string, HotkeyDefinition> hotkeys = null;
        static Dictionary<string, HotstringDefinition> hotstrings = null;

        static GenericFunction keyCondition = null;

        static Dictionary<string, object> variables = null;

        static Dictionary<string, System.Timers.Timer> timers = new Dictionary<string, System.Timers.Timer>();

        static KeyboardHook keyboardHook = null;

        static bool suspended = false;

        static EventHandler onExit = null;

        #region Statics

        [ThreadStatic]
        static int? _ControlDelay;

        [ThreadStatic]
        static int? _WinDelay;

        [ThreadStatic]
        static StringComparison? _StringCaseSense;

        [ThreadStatic]
        static bool? _DetectHiddenText;

        [ThreadStatic]
        static bool? _DetectHiddenWindows;

        [ThreadStatic]
        static string _FormatFloat;

        [ThreadStatic]
        static char? _FormatInteger;

        [ThreadStatic]
        static int? _KeyDelay;

        [ThreadStatic]
        static int? _KeyPressDuration;

        [ThreadStatic]
        static int? _MouseDelay;

        [ThreadStatic]
        static int? _DefaultMouseSpeed;

        [ThreadStatic]
        static int? _TitleMatchMode;

        [ThreadStatic]
        static bool? _TitleMatchModeSpeed;

        [ThreadStatic]
        static string _UserAgent;

        #endregion

        #region GUI

        static Dictionary<int, GUI> GUIs;

        static NotifyIcon Tray;

        [ThreadStatic]
        static int _DefaultGUI;

        static GUI DefaultGUI
        {
            get
            {
                if (GUIs == null)
                {
                    GUIs = new Dictionary<int, GUI>();
                    _DefaultGUI = 1;
                    GUIs.Add(_DefaultGUI, new GUI());
                }
                return GUIs[_DefaultGUI];
            }
        }

        static List<object> Handles = new List<object>();

        #endregion

        const int LoopFrequency = 50;
    }
}
