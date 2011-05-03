using System;
using System.Collections.Generic;
using System.Security;
using System.Windows.Forms;
using Timer = System.Timers.Timer;
using IronAHK.Rusty.Common;

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

        delegate void SimpleDelegate();

        /// <summary>
        /// 
        /// </summary>
        public static event EventHandler ApplicationExit;

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

        static Dictionary<string, Keyboard.HotkeyDefinition> hotkeys;

        static Dictionary<string, Keyboard.HotstringDefinition> hotstrings;

        static GenericFunction keyCondition;

        static Keyboard.KeyboardHook keyboardHook;

        static bool suspended;

        /// <summary>
        /// Is the Script currently suspended?
        /// </summary>
        public static bool Suspended {
            get { return suspended; }
        }


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

        [ThreadStatic]
        static Form dialogOwner;

        static Dictionary<long, ImageList> imageLists;

        static Dictionary<string, Form> guis;

        [ThreadStatic]
        static string defaultGui;

        static string DefaultGuiId
        {
            get { return defaultGui ?? "1"; }
            set
            {
                defaultGui = value;
                defaultTreeView = null;
            }
        }

        static Form DefaultGui
        {
            get
            {
                if (guis == null)
                    return null;

                string key = DefaultGuiId;
                return guis.ContainsKey(key) ? guis[key] : null;
            }
        }

        [ThreadStatic]
        static long lastFoundForm = 0;

        static long LastFoundForm
        {
            get
            {
                return lastFoundForm;
            }
            set
            {
                lastFoundForm = value;
            }
        }

        [ThreadStatic]
        static TreeView defaultTreeView;

        static TreeView DefaultTreeView
        {
            get
            {
                if (defaultTreeView != null)
                    return defaultTreeView;

                var gui = DefaultGui;

                if (gui == null)
                    return null;

                TreeView tv = null;

                foreach (var control in gui.Controls)
                    if (control is TreeView)
                        tv = (TreeView)control;

                return tv;
            }
            set { defaultTreeView = value; }
        }

        [ThreadStatic]
        static ListView defaultListView;

        static ListView DefaultListView
        {
            get
            {
                if (defaultListView != null)
                    return defaultListView;

                var gui = DefaultGui;

                if (gui == null)
                    return null;

                ListView lv = null;

                foreach (var control in gui.Controls)
                    if (control is ListView)
                        lv = (ListView)control;

                return lv;
            }
            set { defaultListView = value; }
        }

        static StatusBar DefaultStatusBar
        {
            get
            {
                var gui = DefaultGui;

                if (gui == null)
                    return null;

                return ((GuiInfo)gui.Tag).StatusBar;
            }
        }

        static NotifyIcon Tray;

        #endregion

        #region Dialogs

        static Dictionary<int, ProgressDialog> progressDialgos;
        static Dictionary<int, SplashDialog> splashDialogs;

        #endregion

        #region Tips

        static ToolTip persistentTooltip;

        static Form tooltip;

        #endregion

        #region RunAs

        [ThreadStatic]
        static string runUser;

        [ThreadStatic]
        static SecureString runPassword;

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
        static StringComparison? _StringComparison;

        [ThreadStatic]
        static string _FormatNumeric;

        [ThreadStatic]
        static string _UserAgent;

        #endregion

        #region Misc

        static Dictionary<string, Timer> timers;

        static EventHandler onExit;

        const int LoopFrequency = 50;

        [ThreadStatic]
        static Random randomGenerator;

        #endregion

        #region Coordmode

        [ThreadStatic]
        static CoordModes coords;

        struct CoordModes
        {
            public CoordModeType Tooltip { get; set; }
            public CoordModeType Pixel { get; set; }
            public CoordModeType Mouse { get; set; }
            public CoordModeType Caret { get; set; }
            public CoordModeType Menu { get; set; }
        }

        enum CoordModeType
        {
            Relative = 0,
            Screen
        }

        #endregion
    }
}
