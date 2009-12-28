using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Principal;

namespace IronAHK.Rusty
{
    partial class Core
    {
        /// <summary>
        /// The full path of the assembly that is currently executing.
        /// </summary>
        public static string A_AhkPath
        {
            get { return Assembly.GetExecutingAssembly().Location; }
        }

        /// <summary>
        /// The version of the assembly that is currently executing.
        /// </summary>
        public static string A_AhkVersion
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        /// <summary>
        /// The full path and name of the folder containing the current user's application-specific data. For example: <code>C:\Documents and Settings\Username\Application Data</code>
        /// </summary>
        public static string A_AppData
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); }
        }

        /// <summary>
        /// The full path and name of the folder containing the all-users application-specific data.
        /// </summary>
        public static string A_AppDataCommon
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData); }
        }

        /// <summary>
        /// The current mode (<code>On</code> or <code>Off</code>) set by <see cref="AutoTrim"/>.
        /// </summary>
        public static string A_AutoTrim
        {
            get { return _AutoTrim ?? true ? Keyword_On : Keyword_Off; }
        }

        /// <summary>
        /// (synonymous with A_NumBatchLines) The current value as set by SetBatchLines. Examples: 200 or 10ms (depending on format).
        /// </summary>
        [Obsolete]
        public static string A_BatchLines
        {
            get { return null; }
        }

        /// <summary>
        /// The current X coordinate of the caret (text insertion point). The coordinates are relative to the active window unless CoordMode is used to make them relative to the entire screen. If there is no active window or the caret position cannot be determined, these variables are blank.
        /// </summary>
        public static string A_CaretX
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// The current Y coordinate of the caret (text insertion point). The coordinates are relative to the active window unless CoordMode is used to make them relative to the entire screen. If there is no active window or the caret position cannot be determined, these variables are blank.
        /// </summary>
        public static string A_CaretY
        {
            get { return string.Empty; }
        }
        
        /// <summary>
        /// The name of the computer as seen on the network.
        /// </summary>
        public static string A_ComputerName
        {
            get { return Environment.MachineName; }
        }

        /// <summary>
        /// The current delay set by <see cref="SetControlDelay"/>.
        /// </summary>
        public static int A_ControlDelay
        {
            get { return _ControlDelay ?? 20; }
        }

        /// <summary>
        /// The type of mouse cursor currently being displayed. It will be one of the following words: AppStarting, Arrow, Cross, Help, IBeam, Icon, No, Size, SizeAll, SizeNESW, SizeNS, SizeNWSE, SizeWE, UpArrow, Wait, Unknown. The acronyms used with the size-type cursors are compass directions, e.g. NESW = NorthEast+SouthWest. The hand-shaped cursors (pointing and grabbing) are classfied as Unknown.
        /// </summary>
        public static string A_Cursor
        {
            get { return null; }
        }

        /// <summary>
        /// See <see cref="A_MDay"/>.
        /// </summary>
        public static string A_DD
        {
            get { return A_MDay; }
        }

        /// <summary>
        /// Current day of the week's 3-letter abbreviation in the current user's language, e.g. <code>Sun</code>.
        /// </summary>
        public static string A_DDD
        {
            get { return DateTime.Now.ToString("ddd"); }
        }

        /// <summary>
        /// Current day of the week's full name in the current user's language, e.g. <code>Sunday</code>.
        /// </summary>
        public static string A_DDDD
        {
            get { return DateTime.Now.ToString("dddd"); }
        }

        /// <summary>
        /// The current speed set by <see cref="SetDefaultMouseSpeed"/>.
        /// </summary>
        public static int A_DefaultMouseSpeed
        {
            get { return _DefaultMouseSpeed ?? 2; }
        }

        /// <summary>
        /// The full path and name of the folder containing the current user's desktop files.
        /// </summary>
        public static string A_Desktop
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.Desktop); }
        }

        /// <summary>
        /// The full path and name of the folder containing the all-users desktop files.
        /// </summary>
        public static string A_DesktopCommon
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory); }
        }

        /// <summary>
        /// The current mode (<code>On</code> or <code>Off</code>) set by <see cref="DetectHiddenText"/>.
        /// </summary>
        public static string A_DetectHiddenText
        {
            get { return _DetectHiddenText ?? true ? Keyword_On : Keyword_Off; }
        }

        /// <summary>
        /// The current mode (<code>On</code> or <code>Off</code>) set by <see cref="DetectHiddenWindows"/>.
        /// </summary>
        public static string A_DetectHiddenWindows
        {
            get { return _DetectHiddenWindows ?? false ? Keyword_On : Keyword_Off; }
        }

        /// <summary>
        /// The ending character that was pressed by the user to trigger the most recent non-auto-replace hotstring. If no ending character was required (due to the * option), this variable will be blank.
        /// </summary>
        public static string A_EndChar
        {
            get { return null; }
        }

        /// <summary>
        /// <para>Contains additional information about the following events:</para>
        /// <list type="">
        /// <item>The OnClipboardChange label</item>
        /// <item>Mouse wheel hotkeys (WheelDown/Up)</item>
        /// <item>RegisterCallback()</item>
        /// <item>GUI events, namely GuiContextMenu, GuiDropFiles, ListBox, ListView, TreeView, and StatusBar. If there is no additional information for an event, A_EventInfo contains 0.</item>
        /// </list>
        /// </summary>
        public static string A_EventInfo
        {
            get { return null; }
        }

        /// <summary>
        /// The most recent reason the script was asked to terminate. This variable is blank unless the script has an OnExit subroutine and that subroutine is currently running or has been called at least once by an exit attempt. See OnExit for details.
        /// </summary>
        public static string A_ExitReason
        {
            get { return null; }
        }

        /// <summary>
        /// The current floating point number format set by <see cref="SetFormat"/>.
        /// </summary>
        public static string A_FormatFloat
        {
            get { return _FormatFloat ?? "0.6"; }
            // TODO: SetFormat to set_A_FormatFloat
        }

        /// <summary>
        /// The current integer format (<code>H</code> or <code>D</code>) set by <see cref="SetFormat"/>.
        /// </summary>
        public static char A_FormatInteger
        {
            get { return _FormatInteger ?? 'D'; }
            // TODO: SetFormat to set_A_FormatInteger
        }

        /// <summary>
        /// The GUI window number that launched the current thread. This variable is blank unless a Gui control, menu bar item, or event such as GuiClose/GuiEscape launched the current thread.
        /// </summary>
        public static string A_Gui
        {
            get { return null; }
        }

        /// <summary>
        /// <para>The type of event that launched the current thread. If the thread was not launched via GUI action, this variable is blank. Otherwise, it contains one of the following strings:</para>
        /// <para>Normal: The event was triggered by a single left-click or via keystrokes (arrow keys, TAB key, space bar, underlined shortcut key, etc.). This value is also used for menu bar items and the special events such as GuiClose and GuiEscape.</para>
        /// <para>DoubleClick: The event was triggered by a double-click. Note: The first click of the click-pair will still cause a Normal event to be received first. In other words, the subroutine will be launched twice: once for the first click and again for the second.</para>
        /// <para>RightClick: Occurs only for GuiContextMenu, ListViews, and TreeViews.</para>
        /// <para>Context-sensitive values: For details see GuiContextMenu, GuiDropFiles, Slider, MonthCal, ListView, and TreeView.</para>
        /// </summary>
        public static string A_GuiEvent
        {
            get { return null; }
        }

        /// <summary>
        /// The name of the variable associated with the GUI control that launched the current thread. If that control lacks an associated variable, A_GuiControl instead contains the first 63 characters of the control's text/caption (this is most often used to avoid giving each button a variable name). A_GuiControl is blank whenever: 1) A_Gui is blank; 2) a GUI menu bar item or event such as GuiClose/GuiEscape launched the current thread; 3) the control lacks an associated variable and has no caption; or 4) The control that originally launched the current thread no longer exists (perhaps due to Gui Destroy).
        /// </summary>
        public static string A_GuiControl
        {
            get { return null; }
        }

        /// <summary>
        /// See <see cref="A_GuiEvent"/>.
        /// </summary>
        public static string A_GuiControlEvent
        {
            get { return null; }
        }

        /// <summary>
        /// These contain the GUI window's height when referenced in a GuiSize subroutine. They apply to the window's client area, which is the area excluding title bar, menu bar, and borders.
        /// </summary>
        public static string A_GuiHeight
        {
            get { return null; }
        }

        /// <summary>
        /// These contain the GUI window's width when referenced in a GuiSize subroutine. They apply to the window's client area, which is the area excluding title bar, menu bar, and borders.
        /// </summary>
        public static string A_GuiWidth
        {
            get { return null; }
        }

        /// <summary>
        /// These contain the X coordinate for GuiContextMenu and GuiDropFiles events. Coordinates are relative to the upper-left corner of the window.
        /// </summary>
        public static string A_GuiX
        {
            get { return null; }
        }

        /// <summary>
        /// These contain the Y coordinate for GuiContextMenu and GuiDropFiles events. Coordinates are relative to the upper-left corner of the window.
        /// </summary>
        public static string A_GuiY
        {
            get { return null; }
        }

        /// <summary>
        /// Current 2-digit hour (00-23) in 24-hour time (for example, 17 is 5pm).
        /// </summary>
        public static string A_Hour
        {
            get { return DateTime.Now.ToString("HH"); }
        }

        /// <summary>
        /// Blank unless a custom tray icon has been specified via Menu, tray, icon -- in which case it's the full path and name of the icon's file.
        /// </summary>
        public static string A_IconFile
        {
            get { return null; }
        }

        /// <summary>
        /// Contains 1 if the tray icon is currently hidden or 0 otherwise. The icon can be hidden via #NoTrayIcon or the Menu command.
        /// </summary>
        public static string A_IconHidden
        {
            get { return null; }
        }

        /// <summary>
        /// Blank if A_IconFile is blank. Otherwise, it's the number of the icon in A_IconFile (typically 1).
        /// </summary>
        public static string A_IconNumber
        {
            get { return null; }
        }

        /// <summary>
        /// Blank unless a custom tooltip for the tray icon has been specified via Menu, Tray, Tip -- in which case it's the text of the tip.
        /// </summary>
        public static string A_IconTip
        {
            get { return null; }
        }

        /// <summary>
        /// The number of the current loop iteration.
        /// </summary>
        public static int A_Index
        {
            get
            {
                if (loops.Count > 0)
                    return loops.Peek().index + 1;
                else
                    return default(int);
            }
        }

        /// <summary>
        /// The IP address of the first network adapter in the computer.
        /// </summary>
        [Obsolete]
        public static string A_IPAddress1
        {
            get { return A_IPAddress.Length > 0 ? A_IPAddress[0] : null; }
        }

        /// <summary>
        /// The IP address of the second network adapter in the computer.
        /// </summary>
        [Obsolete]
        public static string A_IPAddress2
        {
            get { return A_IPAddress.Length > 1 ? A_IPAddress[1] : null; }
        }

        /// <summary>
        /// The IP address of the third network adapter in the computer.
        /// </summary>
        [Obsolete]
        public static string A_IPAddress3
        {
            get { return A_IPAddress.Length > 2 ? A_IPAddress[2] : null; }
        }

        /// <summary>
        /// The IP address of the fourth network adapter in the computer.
        /// </summary>
        [Obsolete]
        public static string A_IPAddress4
        {
            get { return A_IPAddress.Length > 3 ? A_IPAddress[3] : null; }
        }

        /// <summary>
        /// The IP addresses of the network adapters in the computer.
        /// </summary>
        public static string[] A_IPAddress
        {
            get
            {
                var addr = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
                string[] ips = new string[addr.Length];

                for (int i = 0; i < addr.Length; i++)
                    if (addr[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        ips[i] = addr[i].ToString();

                return ips;
            }
        }

        /// <summary>
        /// <code>true</code> if the current user has administrator rights, <code>false</code> otherwise.
        /// </summary>
        public static bool A_ISAdmin
        {
            get { return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator); }
        }

        /// <summary>
        /// <code>true</code> if the current executing assembly is a compiled script, <code>false</code> otherwise;
        /// </summary>
        public static bool A_IsCompiled
        {
            get { return false; }
        }

        /// <summary>
        /// <code>true</code> if the script is suspended, <code>false</code> otherwise;
        /// </summary>
        public static bool A_IsSuspended
        {
            get { return false; }
        }

        /// <summary>
        /// The current delay set by <see cref="SetKeyDelay"/>.
        /// </summary>
        public static int A_KeyDelay
        {
            get { return _KeyDelay ?? 10; }
        }

        /// <summary>
        /// The system's default language code.
        /// </summary>
        public static int A_Language
        {
            get { return System.Threading.Thread.CurrentThread.CurrentCulture.LCID; }
        }

        /// <summary>
        /// The result from Windows <code>GetLastError()</code> function.
        /// </summary>
        public static int A_LastError
        {
            get { return System.Runtime.InteropServices.Marshal.GetLastWin32Error(); }
        }

        /// <summary>
        /// The full path and name of the file to which A_LineNumber belongs, which will be the same as A_ScriptFullPath unless the line belongs to one of a non-compiled script's #Include files.
        /// </summary>
        [Obsolete]
        public static string A_LineFile
        {
            get { return null; }
        }

        /// <summary>
        /// <para>The number of the currently executing line within the script (or one of its #Include files). This line number will match the one shown by ListLines; it can be useful for error reporting such as this example: MsgBox Could not write to log file (line number %A_LineNumber%).</para>
        /// <para>Since a compiled script has merged all its #Include files into one big script, its line numbering may be different than when it is run in non-compiled mode.</para>
        /// </summary>
        public static string A_LineNumber
        {
            get { return null; }
        }

        #region Loops

        /// <summary>
        /// Contains the contents of the current substring (field) from InputVar.
        /// </summary>
        public static string A_LoopField
        {
            get
            {
                if (loops.Count == 0)
                    return null;

                var stack = loops.ToArray();

                for (int i = stack.Length - 1; i > -1; i--)
                {
                    if (stack[i].type == LoopType.Parse)
                        return stack[i].result;
                }

                return null;
            }
        }

        /// <summary>
        /// The attributes of the file currently retrieved.
        /// </summary>
        public static string A_LoopFileAttrib
        {
            get { return null; }
        }

        /// <summary>
        /// The full path of the directory in which A_LoopFileName resides. However, if FilePattern contains a relative path rather than an absolute path, the path here will also be relative. A root directory will not contain a trailing backslash. For example: C:
        /// </summary>
        public static string A_LoopFileDir
        {
            get { return null; }
        }

        /// <summary>
        /// The file's extension (e.g. TXT, DOC, or EXE). The period (.) is not included.
        /// </summary>
        public static string A_LoopFileExt
        {
            get { return null; }
        }

        /// <summary>
        /// The full path and name of the file/folder currently retrieved. However, if FilePattern contains a relative path rather than an absolute path, the path here will also be relative. In addition, any short (8.3) folder names in FilePattern will still be short (see next item to get the long version).
        /// </summary>
        public static string A_LoopFileFullPath
        {
            get { return null; }
        }

        /// <summary>
        /// This is different than A_LoopFileFullPath in the following ways: 1) It always contains the absolute/complete path of the file even if FilePattern contains a relative path; 2) Any short (8.3) folder names in FilePattern itself are converted to their long names; 3) Characters in FilePattern are converted to uppercase or lowercase to match the case stored in the file system. This is useful for converting file names -- such as those passed into a script as command line parameters -- to their exact path names as shown by Explorer.
        /// </summary>
        public static string A_LoopFileLongPath
        {
            get { return null; }
        }

        /// <summary>
        /// The name of the file or folder currently retrieved (without the path).
        /// </summary>
        public static string A_LoopFileName
        {
            get { return null; }
        }

        /// <summary>
        /// The 8.3 short name, or alternate name of the file. If the file doesn't have one (due to the long name being shorter than 8.3 or perhaps because short-name generation is disabled on an NTFS file system), A_LoopFileName will be retrieved instead.
        /// </summary>
        public static string A_LoopFileShortName
        {
            get { return null; }
        }

        /// <summary>
        /// The 8.3 short path and name of the file/folder currently retrieved. For example: C:\MYDOCU~1\ADDRES~1.txt. However, if FilePattern contains a relative path rather than an absolute path, the path here will also be relative.
        /// </summary>
        public static string A_LoopFileShortPath
        {
            get { return null; }
        }

        /// <summary>
        /// The size in bytes of the file currently retrieved. Files larger than 4 gigabytes are also supported.
        /// </summary>
        public static string A_LoopFileSize
        {
            get { return null; }
        }

        /// <summary>
        /// The size in Kbytes of the file currently retrieved, rounded down to the nearest integer.
        /// </summary>
        public static string A_LoopFileSizeKB
        {
            get { return null; }
        }

        /// <summary>
        /// The size in Mbytes of the file currently retrieved, rounded down to the nearest integer.
        /// </summary>
        public static string A_LoopFileSizeMB
        {
            get { return null; }
        }

        /// <summary>
        /// The time the file was last accessed. Format YYYYMMDDHH24MISS.
        /// </summary>
        public static string A_LoopFileTimeAccessed
        {
            get { return null; }
        }

        /// <summary>
        /// The time the file was created. Format YYYYMMDDHH24MISS.
        /// </summary>
        public static string A_LoopFileTimeCreated
        {
            get { return null; }
        }

        /// <summary>
        /// The time the file was last modified. Format YYYYMMDDHH24MISS.
        /// </summary>
        public static string A_LoopFileTimeModified
        {
            get { return null; }
        }

        /// <summary>
        /// Contains the contents of the current line excluding the carriage return and linefeed (`r`n) that marks the end of the line.
        /// </summary>
        public static string A_LoopReadLine
        {
            get { return null; }
        }

        /// <summary>
        /// The name of the root key being accessed (HKEY_LOCAL_MACHINE, HKEY_USERS, HKEY_CURRENT_USER, HKEY_CLASSES_ROOT, or HKEY_CURRENT_CONFIG). For remote registry access, this value will not include the computer name.
        /// </summary>
        public static string A_LoopRegKey
        {
            get { return null; }
        }

        /// <summary>
        /// Name of the currently retrieved item, which can be either a value name or the name of a subkey. Value names displayed by Windows RegEdit as "(Default)" will be retrieved if a value has been assigned to them, but A_LoopRegName will be blank for them.
        /// </summary>
        public static string A_LoopRegName
        {
            get { return null; }
        }

        /// <summary>
        /// Name of the current SubKey. This will be the same as the Key parameter unless the Recurse parameter is being used to recursively explore other subkeys. In that case, it will be the full path of the currently retrieved item, not including the root key. For example: Software\SomeApplication\My SubKey
        /// </summary>
        public static string A_LoopRegSubkey
        {
            get { return null; }
        }

        /// <summary>
        /// The time the current subkey or any of its values was last modified. Format YYYYMMDDHH24MISS. This variable will be empty if the currently retrieved item is not a subkey (i.e. A_LoopRegType is not the word KEY) or if the operating system is Win9x (since Win9x does not track this info).
        /// </summary>
        public static string A_LoopRegTimeModified
        {
            get { return null; }
        }

        /// <summary>
        /// The type of the currently retrieved item, which is one of the following words: KEY (i.e. the currently retrieved item is a subkey not a value), REG_SZ, REG_EXPAND_SZ, REG_MULTI_SZ, REG_DWORD, REG_QWORD, REG_BINARY, REG_LINK, REG_RESOURCE_LIST, REG_FULL_RESOURCE_DESCRIPTOR, REG_RESOURCE_REQUIREMENTS_LIST, REG_DWORD_BIG_ENDIAN (probably rare on most Windows hardware). It will be empty if the currently retrieved item is of an unknown type.
        /// </summary>
        public static string A_LoopRegType
        {
            get { return null; }
        }

        #endregion

        /// <summary>
        /// Current 2-digit day of the month (01-31).
        /// </summary>
        public static string A_MDay
        {
            get { return DateTime.Now.ToString("dd"); }
        }

        /// <summary>
        /// Current 2-digit minute (00-59).
        /// </summary>
        public static string A_Min
        {
            get { return DateTime.Now.ToString("mm"); }
        }

        /// <summary>
        /// Current 2-digit month (01-12). Synonymous with <see cref="A_Mon"/>.
        /// </summary>
        public static string A_MM
        {
            get { return A_Mon; }
        }

        /// <summary>
        /// Current month's abbreviation in the current user's language, e.g. <code>Jul</code>.
        /// </summary>
        public static string A_MMM
        {
            get { return DateTime.Now.ToString("MMM"); }
        }

        /// <summary>
        /// Current month's full name in the current user's language, e.g. <code>July</code>.
        /// </summary>
        public static string A_MMMM
        {
            get { return DateTime.Now.ToString("MMMM"); }
        }

        /// <summary>
        /// Current 2-digit month (01-12).
        /// </summary>
        public static string A_Mon
        {
            get { return DateTime.Now.ToString("MM"); }
        }

        /// <summary>
        /// The current delay set by <code>SetMouseDelay</code>.
        /// </summary>
        public static int A_MouseDelay
        {
            get { return _MouseDelay ?? 10; }
        }

        /// <summary>
        /// Current 3-digit millisecond (000-999).
        /// </summary>
        public static string A_MSec
        {
            get { return DateTime.Now.ToString("fff"); }
        }

        /// <summary>
        /// The full path and name of the current user's "My Documents" folder.
        /// </summary>
        public static string A_MyDocuments
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); }
        }

        /// <summary>
        /// The current local time in YYYYMMDDHH24MISS format.
        /// </summary>
        public static string A_Now
        {
            get { return ToYYYYMMDDHH24MISS(DateTime.Now); }
        }

        /// <summary>
        /// The current Coordinated Universal Time (UTC) in YYYYMMDDHH24MISS format.
        /// </summary>
        public static string A_NowUTC
        {
            get { return ToYYYYMMDDHH24MISS(DateTime.Now.ToUniversalTime()); }
        }

        /// <summary>
        /// See <see cref="A_BatchLines"/>.
        /// </summary>
        [Obsolete]
        public static string A_NumBatchLines
        {
            get { return A_BatchLines; }
        }

        /// <summary>
        /// The type of Operating System being run, e.g. <code>WIN32_WINDOWS</code> for Windows 95/98/ME or <code>WIN32_NT</code> for Windows NT4/2000/XP/2003/Vista.
        /// </summary>
        public static string A_OSType
        {
            get { return ToOSType(Environment.OSVersion.Platform); }
        }

        /// <summary>
        /// The Operating System version, e.g. <code>WIN_VISTA</code>, <code>WIN_2003</code>, <code>WIN_XP</code>, <code>WIN_2000</code>, <code>WIN_NT4</code>, <code>WIN_95</code>, <code>WIN_98</code>, <code>WIN_ME</code>.
        /// </summary>
        public static string A_OSVersion
        {
            get { return Environment.OSVersion.VersionString; }
        }

        /// <summary>
        /// Same as above except for the previous hotkey. It will be blank if none.
        /// </summary>
        public static string A_PriorHotkey
        {
            get { return null; }
        }

        /// <summary>
        /// The Program Files directory (e.g. <code>C:\Program Files</code>).
        /// </summary>
        public static string A_ProgramFiles
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles); }
        }

        /// <summary>
        /// The full path and name of the Programs folder in the current user's Start Menu.
        /// </summary>
        public static string A_Programs
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.Programs); }
        }

        /// <summary>
        /// The full path and name of the Programs folder in the all-users Start Menu.
        /// </summary>
        public static string A_ProgramsCommon
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.Startup); }
        }

        /// <summary>
        /// <para>The width and height of the primary monitor, in pixels (e.g. 1024 and 768).</para>
        /// <para>To discover the dimensions of other monitors in a multi-monitor system, use SysGet.</para>
        /// <para>To instead discover the width and height of the entire desktop (even if it spans multiple monitors), use the following example (but on Windows 95/NT, both of the below variables will be set to 0):</para>
        /// <code>
        /// SysGet, VirtualWidth, 78
        /// SysGet, VirtualHeight, 79
        /// </code>
        /// <para>In addition, use SysGet to discover the work area of a monitor, which can be smaller than the monitor's total area because the taskbar and other registered desktop toolbars are excluded.</para>
        /// </summary>
        public static int A_ScreenHeight
        {
            get
            {
                IntPtr handle = Win32.GetDesktopWindow(); /* get the hDC of the target window */
                Win32.RECT windowRect; 
                Win32.GetWindowRect(handle, out windowRect);
                return windowRect.Bottom - windowRect.Top;
            }
        }

        /// <summary>
        /// See <see cref="A_ScreenHeight"/>.
        /// </summary>
        public static int A_ScreenWidth
        {
            get
            {
                IntPtr handle = Win32.GetDesktopWindow(); /* get the hDC of the target window */
                Win32.RECT windowRect; 
                Win32.GetWindowRect(handle, out windowRect);
                return windowRect.Right - windowRect.Left;
            }
        }

        /// <summary>
        /// The full path of the directory where the current script is located.
        /// </summary>
        [Obsolete]
        public static string A_ScriptDir
        {
            get { return null; }
        }

        /// <summary>
        /// The full path of the running script.
        /// </summary>
        public static string A_ScriptFullPath
        {
            get { return null; }
        }

        /// <summary>
        /// The file name of the current script, without its path, e.g. <code>MyScript.ahk</code>.
        /// </summary>
        public static string A_ScriptName
        {
            get { return null; }
        }

        /// <summary>
        /// Current 2-digit second (00-59).
        /// </summary>
        public static string A_Sec
        {
            get { return DateTime.Now.ToString("ss"); }
        }

        /// <summary>
        /// This variable contains a single space character.
        /// </summary>
        [Obsolete]
        public static string A_Space
        {
            get { return " "; }
        }

        /// <summary>
        /// The full path and name of the current user's Start Menu folder.
        /// </summary>
        public static string A_StartMenu
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.StartMenu); }
        }

        /// <summary>
        /// The full path and name of the all-users Start Menu folder.
        /// </summary>
        public static string A_StartMenuCommon
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.StartMenu); }
        }

        /// <summary>
        /// The full path and name of the Startup folder in the current user's Start Menu.
        /// </summary>
        public static string A_Startup
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.Startup); }
        }

        /// <summary>
        /// The full path and name of the Startup folder in the all-users Start Menu.
        /// </summary>
        public static string A_StartupCommon
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.Startup); }
        }

        /// <summary>
        /// The current mode (On, Off, or Locale) set by StringCaseSense.
        /// </summary>
        public static string A_StringCaseSense
        {
            get { return ToStringCaseSense(_StringCaseSense ?? StringComparison.OrdinalIgnoreCase); }
        }

        /// <summary>
        /// This variable contains a single tab character.
        /// </summary>
        [Obsolete]
        public static string A_Tab
        {
            get { return "\t"; }
        }

        /// <summary>
        /// The full path and name of the folder designated to hold temporary files.
        /// </summary>
        public static string A_Temp
        {
            get { return Path.GetTempPath(); }
        }

        /// <summary>
        /// Temporary file name.
        /// </summary>
        public static string A_TempFile
        {
            get { return Path.GetTempFileName(); }
        }

        /// <summary>
        /// The name of the user-defined function that is currently executing (blank if none); for example: <code>MyFunction</code>.
        /// </summary>
        public static string A_ThisFunc
        {
            get { return null; }
        }

        /// <summary>
        /// <para>The key name of the most recently executed hotkey or hotstring (blank if none), e.g. #z. This value will change if the current thread is interrupted by another hotkey, so be sure to copy it into another variable immediately if you need the original value for later use in a subroutine.</para>
        /// <para>When a hotkey is first created -- either by the Hotkey command or a double-colon label in the script -- its key name and the ordering of its modifier symbols becomes the permanent name of that hotkey. See also: A_ThisLabel</para>
        /// </summary>
        public static string A_ThisHotkey
        {
            get { return null; }
        }

        /// <summary>
        /// The name of the label (subroutine) that is currently executing (blank if none); for example: MyLabel. It is updated whenever the script executes Gosub/Return or Goto. It is also updated for automatically-called labels such as timers, GUI threads, menu items, hotkeys, hotstrings, OnClipboardChange, and OnExit, However, A_ThisLabel is not updated when execution "falls into" a label from above; when that happens, A_ThisLabel retains its previous value. See also: A_ThisHotkey
        /// </summary>
        public static string A_ThisLabel
        {
            get { return null; }
        }

        /// <summary>
        /// The name of the menu from which A_ThisMenuItem was selected.
        /// </summary>
        public static string A_ThisMenu
        {
            get { return null; }
        }

        /// <summary>
        /// The name of the most recently selected custom menu item (blank if none).
        /// </summary>
        public static string A_ThisMenuItem
        {
            get { return null; }
        }

        /// <summary>
        /// A number indicating the current position of A_ThisMenuItem within A_ThisMenu. The first item in the menu is 1, the second is 2, and so on. Menu separator lines are counted. This variable is blank if A_ThisMenuItem is blank or no longer exists within A_ThisMenu. It is also blank if A_ThisMenu itself no longer exists.
        /// </summary>
        public static string A_ThisMenuItemPos
        {
            get { return null; }
        }

        /// <summary>
        /// The number of milliseconds since the computer was rebooted.
        /// </summary>
        public static int A_TickCount
        {
            get { return Environment.TickCount; }
        }

        /// <summary>
        /// The number of milliseconds that have elapsed since the system last received keyboard, mouse, or other input. This is useful for determining whether the user is away. This variable will be blank unless the operating system is Windows 2000, XP, or beyond. Physical input from the user as well as artificial input generated by any program or script (such as the Send or MouseMove commands) will reset this value back to zero. Since this value tends to increase by increments of 10, do not check whether it is equal to another value. Instead, check whether it is greater or less than another value. For example: IfGreater, A_TimeIdle, 600000, MsgBox, The last keyboard or mouse activity was at least 10 minutes ago.
        /// </summary>
        public static string A_TimeIdle
        {
            get { return null; }
        }

        /// <summary>
        /// Same as above but ignores artificial keystrokes and/or mouse clicks whenever the corresponding hook (keyboard or mouse) is installed. If neither hook is installed, this variable is equivalent to A_TimeIdle. If only one hook is present, only that one type of artificial input will be ignored. A_TimeIdlePhysical may be more useful than A_TimeIdle for determining whether the user is truly present.
        /// </summary>
        public static string A_TimeIdlePhysical
        {
            get { return null; }
        }

        /// <summary>
        /// The number of milliseconds that have elapsed since A_PriorHotkey was pressed. It will be -1 whenever A_PriorHotkey is blank.
        /// </summary>
        public static string A_TimeSincePriorHotkey
        {
            get { return null; }
        }

        /// <summary>
        /// The number of milliseconds that have elapsed since A_ThisHotkey was pressed. It will be -1 whenever A_ThisHotkey is blank.
        /// </summary>
        public static string A_TimeSinceThisHotkey
        {
            get { return null; }
        }

        /// <summary>
        /// The current mode set by <code>SetTitleMatchMode</code>: <code>1</code>, <code>2</code>, <code>3</code>, or <code>RegEx</code>.
        /// </summary>
        public static string A_TitleMatchMode
        {
            get
            {
                int mode = _TitleMatchMode ?? 1;
                return mode == 4 ? Keyword_RegEx : mode.ToString();
            }
            // TODO: SetTitleMatchMode as A_TitleMatchMode
        }

        /// <summary>
        /// The current match speed (<code>fast</code> or <code>slow</code>) set by <code>SetTitleMatchMode</code>.
        /// </summary>
        public static string A_TitleMatchModeSpeed
        {
            get { return _TitleMatchModeSpeed ?? true ? Keyword_Fast : Keyword_Slow; }
            // TODO: SetTitleMatchMode as A_TitleMatchModeSpeed
        }

        /// <summary>
        /// The logon name of the current user.
        /// </summary>
        public static string A_UserName
        {
            get { return Environment.UserName; }
        }

        /// <summary>
        /// Current 1-digit day of the week (1-7). 1 is Sunday in all locales.
        /// </summary>
        public static string A_WDay
        {
            get { return null; }
        }

        /// <summary>
        /// The current delay set by SetWinDelay (always decimal, not hex).
        /// </summary>
        public static string A_WinDelay
        {
            get { return null; }
        }

        /// <summary>
        /// The Windows directory. For example: C:\Windows
        /// </summary>
        public static string A_WinDir
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.System); }
        }

        /// <summary>
        /// The script's current working directory, which is where files will be accessed by default. The final backslash is not included unless it is the root directory. Two examples: C:\ and C:\My Documents. Use SetWorkingDir to change the working directory.
        /// </summary>
        public static string A_WorkingDir
        {
            get { return Environment.CurrentDirectory; }
            set { Environment.CurrentDirectory = value; }
        }

        /// <summary>
        /// Current day of the year (1-366). The value is not zero-padded, e.g. 9 is retrieved, not 009. To retrieve a zero-padded value, use the following: FormatTime, OutputVar, , YDay0
        /// </summary>
        public static string A_YDay
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// Current 4-digit year (e.g. 2004). Note: To retrieve a formatted time or date appropriate for your locale and language, use "FormatTime, OutputVar" (time and long date) or "FormatTime, OutputVar,, LongDate" (retrieves long-format date).
        /// </summary>
        public static int A_Year
        {
            get { return DateTime.Now.Year; }
        }

        /// <summary>
        /// Current year and week number (e.g. 200453) according to ISO 8601. To separate the year from the week, use StringLeft, Year, A_YWeek, 4 and StringRight, Week, A_YWeek, 2. Precise definition of A_YWeek: If the week containing January 1st has four or more days in the new year, it is considered week 1. Otherwise, it is the last week of the previous year, and the next week is week 1.
        /// </summary>
        public static int A_YWeek
        {
            get { return 0; }
        }

        /// <summary>
        /// See <see cref="A_Year"/>.
        /// </summary>
        public static int A_YYYY
        {
            get { return A_Year; }
        }

        /// <summary>
        /// HTTP user agent for <see cref="URLDownloadToFile"/>.
        /// </summary>
        public static string A_UserAgent
        {
            get { return _UserAgent; }
            set { _UserAgent = value; }
        }
    }
}
