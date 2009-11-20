using System;
using System.Collections.Generic;
using System.Security;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Core
    {
        static int error = 0;

        /// <summary>
        /// A function.
        /// </summary>
        /// <param name="args">Parameters.</param>
        /// <returns>A value.</returns>
        public delegate object GenericFunction(object[] args);

        /// <summary>
        /// Indicates the success or failure of some of the command.
        /// </summary>
        static public int ErrorLevel
        {
            get { return error; }
            set { error = value; }
        }

        struct Settings
        {
            [ThreadStaticAttribute]
            public static bool AutoTrim = true;
            /// <summary>
            /// Use integer below zero for time mode.
            /// </summary>
            [ThreadStaticAttribute]
            public static int BatchLines = -10;
            [ThreadStaticAttribute]
            public static int ControlDelay = 20;
            [ThreadStaticAttribute]
            public static int WinDelay = 100;
            [ThreadStaticAttribute]
            public static bool StringCaseSense = false; // locale = on
            [ThreadStaticAttribute]
            public static bool DetectHiddenText = true;
            [ThreadStaticAttribute]
            public static bool DetectHiddenWindows = false;
            [ThreadStaticAttribute]
            public static string FormatFloat = "0.6";
            [ThreadStaticAttribute]
            public static char FormatInteger = 'd';
            [ThreadStaticAttribute]
            public static int KeyDelay = 10;
            [ThreadStaticAttribute]
            public static int KeyPressDuration = -1;
            [ThreadStaticAttribute]
            public static int MouseDelay = 10;
            [ThreadStaticAttribute]
            public static int DefaultMouseSpeed = 2;
            [ThreadStaticAttribute]
            public static int TitleMatchMode = 1;
            [ThreadStaticAttribute]
            public static bool TitleMatchModeSpeed = true;
            [ThreadStaticAttribute]
            public static char SendMode = 'e';

            [ThreadStaticAttribute]
            public static string UserAgent = "IronAHK";

            public static Dictionary<string, IntPtr[]> WinGroups = new Dictionary<string, IntPtr[]>();
            public static DialogResult MsgBox;

            #region Misc

            /// <summary>
            /// Where a value of true represents relative positioning mode to the parent window.
            /// </summary>
            public struct CoordMode
            {
                [ThreadStaticAttribute]
                public static bool ToolTip = false;
                [ThreadStaticAttribute]
                public static bool Pixel = false;
                [ThreadStaticAttribute]
                public static bool Mouse = false;
                [ThreadStaticAttribute]
                public static bool Caret = false;
                [ThreadStaticAttribute]
                public static bool Menu = false;
            }

            public struct RunAs
            {
                [ThreadStaticAttribute]
                public static string Username;
                [ThreadStaticAttribute]
                public static SecureString Password;
                [ThreadStaticAttribute]
                public static string Domain;
            }

            #endregion

            #region GUI

            public static Windows.Windows Windows = new Windows.Windows();
            public static ToolTip[] ToolTips = new ToolTip[20];
            public static GUI[] GUIs = new GUI[99];
            public static NotifyIcon Tray = null;
            public static Dictionary<string, ContextMenu> Menus = new Dictionary<string, ContextMenu>();
            [ThreadStaticAttribute]
            static int _DefaultGUI = 1;
            public static GUI GUI { get { return GUIs[DefaultGUI]; } }
            public static int DefaultGUI
            {
                get { return _DefaultGUI; }
                set
                {
                    if (value < 1 || value > 99)
                        throw new ArgumentOutOfRangeException("Should be between 1 and 99");
                    else
                        _DefaultGUI = value;
                }
            }

            #endregion

            public static List<object> Handles = new List<object>();

            public static readonly string[] OnOff = new string[] { Keywords.Off, Keywords.On };
            public static readonly char[] Spaces = new char[] { ' ', '\t', '\r', '\n', '\xA0' };
            public static readonly int LoopFrequency = 50;
        }

        class GUI : Form
        {
            StatusBar _StatusBar;
            public StatusBar StatusBar
            {
                get { return _StatusBar; }
                set { _StatusBar = value; }
            }

            ListView _ListView;
            public ListView ListView
            {
                get { return _ListView; }
                set { _ListView = value; }
            }

            TreeView _TreeView;
            public TreeView TreeView
            {
                get { return _TreeView; }
                set { _TreeView = value; }
            }
        }
    }
}
