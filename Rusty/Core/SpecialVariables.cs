using System;
using System.Diagnostics;
using System.IO;

namespace IronAHK.Rusty
{
    partial class Core
    {
        /// <summary>
        /// <para>For non-compiled scripts: The full path and name of the EXE file that is actually running the current script. For example: C:\Program Files\AutoHotkey\AutoHotkey.exe</para>
        /// <para>For compiled scripts: The same as the above except the AutoHotkey directory is discovered via the registry entry HKEY_LOCAL_MACHINE\SOFTWARE\AutoHotkey\InstallDir. If there is no such entry, A_AhkPath is blank.</para>
        /// </summary>
        public static string A_AhkPath
        {
            get { return string.Empty; }
            /*set { Settings.AhkPath = value; }*/
        }

        /// <summary>
        /// In versions prior to 1.0.22, this variable is blank. Otherwise, it contains the version of AutoHotkey that is running the script, such as 1.0.22. In the case of a compiled script, the version that was originally used to compile it is reported. The formatting of the version number allows a script to check whether A_AhkVersion is greater than some minimum version number with > or >= as in this example: if A_AhkVersion >= 1.0.25.07
        /// </summary>
        public static string A_AhkVersion
        {
            get { return string.Empty; }
            /*set { Settings.AhkVersion = value; }*/
        }

        /// <summary>
        /// The full path and name of the folder containing the current user's application-specific data. For example: C:\Documents and Settings\Username\Application Data
        /// </summary>
        public static string A_AppData
        {
            get { return string.Empty; }
            /*set { Settings.AppData = value; }*/
        }

        /// <summary>
        /// The full path and name of the folder containing the all-users application-specific data.
        /// </summary>
        public static string A_AppDataCommon
        {
            get { return string.Empty; }
            /*set { Settings.AppDataCommon = value; }*/
        }

        /// <summary>
        /// The current mode (On or Off) set by AutoTrim.
        /// </summary>
        public static string A_AutoTrim
        {
            get { return string.Empty; }
            /*set { Settings.AutoTrim = value; }*/
        }

        /// <summary>
        /// (synonymous with A_NumBatchLines) The current value as set by SetBatchLines. Examples: 200 or 10ms (depending on format).
        /// </summary>
        public static string A_BatchLines
        {
            get { return string.Empty; }
            /*set { Settings.BatchLines = value; }*/
        }

        /// <summary>
        /// The current X coordinate of the caret (text insertion point). The coordinates are relative to the active window unless CoordMode is used to make them relative to the entire screen. If there is no active window or the caret position cannot be determined, these variables are blank.
        /// </summary>
        public static string A_CaretX
        {
            get { return string.Empty; }
            /*set { Settings.CaretX = value; }*/
        }

        /// <summary>
        /// The current Y coordinate of the caret (text insertion point). The coordinates are relative to the active window unless CoordMode is used to make them relative to the entire screen. If there is no active window or the caret position cannot be determined, these variables are blank.
        /// </summary>
        public static string A_CaretY
        {
            get { return string.Empty; }
            /*set { Settings.CaretY = value; }*/
        }
        
        /// <summary>
        /// The name of the computer as seen on the network.
        /// </summary>
        public static string A_ComputerName
        {
            get { return string.Empty; }
            /*set { Settings.ComputerName = value; }*/
        }

        /// <summary>
        /// The current delay set by SetControlDelay (always decimal, not hex).
        /// </summary>
        public static string A_ControlDelay
        {
            get { return string.Empty; }
            /*set { Settings.ControlDelay = value; }*/
        }

        /// <summary>
        /// The type of mouse cursor currently being displayed. It will be one of the following words: AppStarting, Arrow, Cross, Help, IBeam, Icon, No, Size, SizeAll, SizeNESW, SizeNS, SizeNWSE, SizeWE, UpArrow, Wait, Unknown. The acronyms used with the size-type cursors are compass directions, e.g. NESW = NorthEast+SouthWest. The hand-shaped cursors (pointing and grabbing) are classfied as Unknown.
        /// </summary>
        public static string A_Cursor
        {
            get { return string.Empty; }
            /*set { Settings.Cursor = value; }*/
        }

        /// <summary>
        /// See <see cref="A_MDay"/>.
        /// </summary>
        public static int A_DD { get { return A_MDay; } }

        /// <summary>
        /// Current day of the week's 3-letter abbreviation in the current user's language, e.g. Sun
        /// </summary>
        public static string A_DDD
        {
            get { return string.Empty; }
            /*set { Settings.DDD = value; }*/
        }

        /// <summary>
        /// Current day of the week's full name in the current user's language, e.g. Sunday
        /// </summary>
        public static string A_DDDD
        {
            get { return string.Empty; }
            /*set { Settings.DDDD = value; }*/
        }

        /// <summary>
        /// The current speed set by SetDefaultMouseSpeed (always decimal, not hex).
        /// </summary>
        public static string A_DefaultMouseSpeed
        {
            get { return string.Empty; }
            /*set { Settings.DefaultMouseSpeed = value; }*/
        }

        /// <summary>
        /// The full path and name of the folder containing the current user's desktop files.
        /// </summary>
        public static string A_Desktop
        {
            get { return string.Empty; }
            /*set { Settings.Desktop = value; }*/
        }

        /// <summary>
        /// The full path and name of the folder containing the all-users desktop files.
        /// </summary>
        public static string A_DesktopCommon
        {
            get { return string.Empty; }
            /*set { Settings.DesktopCommon = value; }*/
        }

        /// <summary>
        /// The current mode (On or Off) set by DetectHiddenText.
        /// </summary>
        public static string A_DetectHiddenText
        {
            get { return string.Empty; }
            /*set { Settings.DetectHiddenText = value; }*/
        }

        /// <summary>
        /// The current mode (On or Off) set by DetectHiddenWindows.
        /// </summary>
        public static string A_DetectHiddenWindows
        {
            get { return string.Empty; }
            /*set { Settings.DetectHiddenWindows = value; }*/
        }

        /// <summary>
        /// The ending character that was pressed by the user to trigger the most recent non-auto-replace hotstring. If no ending character was required (due to the * option), this variable will be blank.
        /// </summary>
        public static string A_EndChar
        {
            get { return string.Empty; }
            /*set { Settings.EndChar = value; }*/
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
            get { return string.Empty; }
            /*set { Settings.EventInfo = value; }*/
        }

        /// <summary>
        /// The most recent reason the script was asked to terminate. This variable is blank unless the script has an OnExit subroutine and that subroutine is currently running or has been called at least once by an exit attempt. See OnExit for details.
        /// </summary>
        public static string A_ExitReason
        {
            get { return string.Empty; }
            /*set { Settings.ExitReason = value; }*/
        }

        /// <summary>
        /// The current floating point number format set by SetFormat.
        /// </summary>
        public static string A_FormatFloat
        {
            get { return string.Empty; }
            /*set { Settings.FormatFloat = value; }*/
        }

        /// <summary>
        /// The current integer format (H or D) set by SetFormat.
        /// </summary>
        public static string A_FormatInteger
        {
            get { return string.Empty; }
            /*set { Settings.FormatInteger = value; }*/
        }

        /// <summary>
        /// The GUI window number that launched the current thread. This variable is blank unless a Gui control, menu bar item, or event such as GuiClose/GuiEscape launched the current thread.
        /// </summary>
        public static string A_Gui
        {
            get { return string.Empty; }
            /*set { Settings.Gui = value; }*/
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
            get { return string.Empty; }
            /*set { Settings.GuiEvent = value; }*/
        }

        /// <summary>
        /// The name of the variable associated with the GUI control that launched the current thread. If that control lacks an associated variable, A_GuiControl instead contains the first 63 characters of the control's text/caption (this is most often used to avoid giving each button a variable name). A_GuiControl is blank whenever: 1) A_Gui is blank; 2) a GUI menu bar item or event such as GuiClose/GuiEscape launched the current thread; 3) the control lacks an associated variable and has no caption; or 4) The control that originally launched the current thread no longer exists (perhaps due to Gui Destroy).
        /// </summary>
        public static string A_GuiControl
        {
            get { return string.Empty; }
            /*set { Settings.GuiControl = value; }*/
        }

        /// <summary>
        /// See <see cref="A_GuiEvent"/>.
        /// </summary>
        public static string A_GuiControlEvent
        {
            get { return string.Empty; }
            /*set { Settings.GuiControlEvent = value; }*/
        }

        /// <summary>
        /// These contain the GUI window's height when referenced in a GuiSize subroutine. They apply to the window's client area, which is the area excluding title bar, menu bar, and borders.
        /// </summary>
        public static string A_GuiHeight
        {
            get { return string.Empty; }
            /*set { Settings.GuiHeight = value; }*/
        }

        /// <summary>
        /// These contain the GUI window's width when referenced in a GuiSize subroutine. They apply to the window's client area, which is the area excluding title bar, menu bar, and borders.
        /// </summary>
        public static string A_GuiWidth
        {
            get { return string.Empty; }
            /*set { Settings.GuiWidth = value; }*/
        }

        /// <summary>
        /// These contain the X coordinate for GuiContextMenu and GuiDropFiles events. Coordinates are relative to the upper-left corner of the window.
        /// </summary>
        public static string A_GuiX
        {
            get { return string.Empty; }
            /*set { Settings.GuiX = value; }*/
        }

        /// <summary>
        /// These contain the Y coordinate for GuiContextMenu and GuiDropFiles events. Coordinates are relative to the upper-left corner of the window.
        /// </summary>
        public static string A_GuiY
        {
            get { return string.Empty; }
            /*set { Settings.GuiY = value; }*/
        }

        /// <summary>
        /// Current 2-digit hour (00-23) in 24-hour time (for example, 17 is 5pm). To retrieve 12-hour time as well as an AM/PM indicator, follow this example: FormatTime, OutputVar, , h:mm:ss tt
        /// </summary>
        public static string A_Hour
        {
            get { return string.Empty; }
            /*set { Settings.Hour = value; }*/
        }

        /// <summary>
        /// Blank unless a custom tray icon has been specified via Menu, tray, icon -- in which case it's the full path and name of the icon's file.
        /// </summary>
        public static string A_IconFile
        {
            get { return string.Empty; }
            /*set { Settings.IconFile = value; }*/
        }

        /// <summary>
        /// Contains 1 if the tray icon is currently hidden or 0 otherwise. The icon can be hidden via #NoTrayIcon or the Menu command.
        /// </summary>
        public static string A_IconHidden
        {
            get { return string.Empty; }
            /*set { Settings.IconHidden = value; }*/
        }

        /// <summary>
        /// Blank if A_IconFile is blank. Otherwise, it's the number of the icon in A_IconFile (typically 1).
        /// </summary>
        public static string A_IconNumber
        {
            get { return string.Empty; }
            /*set { Settings.IconNumber = value; }*/
        }

        /// <summary>
        /// Blank unless a custom tooltip for the tray icon has been specified via Menu, Tray, Tip -- in which case it's the text of the tip.
        /// </summary>
        public static string A_IconTip
        {
            get { return string.Empty; }
            /*set { Settings.IconTip = value; }*/
        }

        /// <summary>
        /// The number of the current loop iteration.
        /// </summary>
        public static int A_Index
        {
            get { return 0; }
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
        /// The IP addresses of the network adapter in the computer.
        /// </summary>
        public static string[] A_IPAddress
        {
            get { return new string[] { string.Empty }; }
        }

        /// <summary>
        /// <para>If the current user has admin rights, this variable contains 1. Otherwise, it contains 0. Under Windows 95/98/Me, this variable always contains 1.</para>
        /// <para>On Windows Vista, some scripts might require administrator privileges to function properly (such as a script that interacts with a process or window that is run as administrator).</para>
        /// </summary>
        public static string A_ISAdmin
        {
            get { return string.Empty; }
            /*set { Settings.ISAdmin = value; }*/
        }

        /// <summary>
        /// Contains 1 if the script is running as a compiled EXE and nothing if it is not.
        /// </summary>
        public static string A_IsCompiled
        {
            get { return string.Empty; }
            /*set { Settings.IsCompiled = value; }*/
        }

        /// <summary>
        /// Contains 1 if the script is suspended and 0 otherwise.
        /// </summary>
        public static string A_IsSuspended
        {
            get { return string.Empty; }
            /*set { Settings.IsSuspended = value; }*/
        }

        /// <summary>
        /// The current delay set by SetKeyDelay (always decimal, not hex). This delay is for the traditional SendEvent mode, not SendPlay.
        /// </summary>
        public static string A_KeyDelay
        {
            get { return string.Empty; }
            /*set { Settings.KeyDelay = value; }*/
        }

        /// <summary>
        /// The system's default language, which is one of these 4-digit codes.
        /// </summary>
        public static string A_Language
        {
            get { return string.Empty; }
            /*set { Settings.Language = value; }*/
        }

        /// <summary>
        /// The result from the OS's GetLastError() function. For details, see DllCall() and Run/RunWait.
        /// </summary>
        public static string A_LastError
        {
            get { return string.Empty; }
            /*set { Settings.LastError = value; }*/
        }

        /// <summary>
        /// The full path and name of the file to which A_LineNumber belongs, which will be the same as A_ScriptFullPath unless the line belongs to one of a non-compiled script's #Include files.
        /// </summary>
        public static string A_LineFile
        {
            get { return string.Empty; }
            /*set { Settings.LineFile = value; }*/
        }

        /// <summary>
        /// <para>The number of the currently executing line within the script (or one of its #Include files). This line number will match the one shown by ListLines; it can be useful for error reporting such as this example: MsgBox Could not write to log file (line number %A_LineNumber%).</para>
        /// <para>Since a compiled script has merged all its #Include files into one big script, its line numbering may be different than when it is run in non-compiled mode.</para>
        /// </summary>
        public static string A_LineNumber
        {
            get { return string.Empty; }
            /*set { Settings.LineNumber = value; }*/
        }

        #region Loops

        /// <summary>
        /// Contains the contents of the current substring (field) from InputVar.
        /// </summary>
        public static string A_LoopField
        {
            get { return string.Empty; }
            /*set { Settings.LoopField = value; }*/
        }

        /// <summary>
        /// The attributes of the file currently retrieved.
        /// </summary>
        public static string A_LoopFileAttrib
        {
            get { return string.Empty; }
            /*set { Settings.LoopFileAttrib = value; }*/
        }

        /// <summary>
        /// The full path of the directory in which A_LoopFileName resides. However, if FilePattern contains a relative path rather than an absolute path, the path here will also be relative. A root directory will not contain a trailing backslash. For example: C:
        /// </summary>
        public static string A_LoopFileDir
        {
            get { return string.Empty; }
            /*set { Settings.LoopFileDir = value; }*/
        }

        /// <summary>
        /// The file's extension (e.g. TXT, DOC, or EXE). The period (.) is not included.
        /// </summary>
        public static string A_LoopFileExt
        {
            get { return string.Empty; }
            /*set { Settings.LoopFileExt = value; }*/
        }

        /// <summary>
        /// The full path and name of the file/folder currently retrieved. However, if FilePattern contains a relative path rather than an absolute path, the path here will also be relative. In addition, any short (8.3) folder names in FilePattern will still be short (see next item to get the long version).
        /// </summary>
        public static string A_LoopFileFullPath
        {
            get { return string.Empty; }
            /*set { Settings.LoopFileFullPath = value; }*/
        }

        /// <summary>
        /// This is different than A_LoopFileFullPath in the following ways: 1) It always contains the absolute/complete path of the file even if FilePattern contains a relative path; 2) Any short (8.3) folder names in FilePattern itself are converted to their long names; 3) Characters in FilePattern are converted to uppercase or lowercase to match the case stored in the file system. This is useful for converting file names -- such as those passed into a script as command line parameters -- to their exact path names as shown by Explorer.
        /// </summary>
        public static string A_LoopFileLongPath
        {
            get { return string.Empty; }
            /*set { Settings.LoopFileLongPath = value; }*/
        }

        /// <summary>
        /// The name of the file or folder currently retrieved (without the path).
        /// </summary>
        public static string A_LoopFileName
        {
            get { return string.Empty; }
            /*set { Settings.LoopFileName = value; }*/
        }

        /// <summary>
        /// The 8.3 short name, or alternate name of the file. If the file doesn't have one (due to the long name being shorter than 8.3 or perhaps because short-name generation is disabled on an NTFS file system), A_LoopFileName will be retrieved instead.
        /// </summary>
        public static string A_LoopFileShortName
        {
            get { return string.Empty; }
            /*set { Settings.LoopFileShortName = value; }*/
        }

        /// <summary>
        /// The 8.3 short path and name of the file/folder currently retrieved. For example: C:\MYDOCU~1\ADDRES~1.txt. However, if FilePattern contains a relative path rather than an absolute path, the path here will also be relative.
        /// </summary>
        public static string A_LoopFileShortPath
        {
            get { return string.Empty; }
            /*set { Settings.LoopFileShortPath = value; }*/
        }

        /// <summary>
        /// The size in bytes of the file currently retrieved. Files larger than 4 gigabytes are also supported.
        /// </summary>
        public static string A_LoopFileSize
        {
            get { return string.Empty; }
            /*set { Settings.LoopFileSize = value; }*/
        }

        /// <summary>
        /// The size in Kbytes of the file currently retrieved, rounded down to the nearest integer.
        /// </summary>
        public static string A_LoopFileSizeKB
        {
            get { return string.Empty; }
            /*set { Settings.LoopFileSizeKB = value; }*/
        }

        /// <summary>
        /// The size in Mbytes of the file currently retrieved, rounded down to the nearest integer.
        /// </summary>
        public static string A_LoopFileSizeMB
        {
            get { return string.Empty; }
            /*set { Settings.LoopFileSizeMB = value; }*/
        }

        /// <summary>
        /// The time the file was last accessed. Format YYYYMMDDHH24MISS.
        /// </summary>
        public static string A_LoopFileTimeAccessed
        {
            get { return string.Empty; }
            /*set { Settings.LoopFileTimeAccessed = value; }*/
        }

        /// <summary>
        /// The time the file was created. Format YYYYMMDDHH24MISS.
        /// </summary>
        public static string A_LoopFileTimeCreated
        {
            get { return string.Empty; }
            /*set { Settings.LoopFileTimeCreated = value; }*/
        }

        /// <summary>
        /// The time the file was last modified. Format YYYYMMDDHH24MISS.
        /// </summary>
        public static string A_LoopFileTimeModified
        {
            get { return string.Empty; }
            /*set { Settings.LoopFileTimeModified = value; }*/
        }

        /// <summary>
        /// Contains the contents of the current line excluding the carriage return and linefeed (`r`n) that marks the end of the line.
        /// </summary>
        public static string A_LoopReadLine
        {
            get { return string.Empty; }
            /*set { Settings.LoopReadLine = value; }*/
        }

        /// <summary>
        /// The name of the root key being accessed (HKEY_LOCAL_MACHINE, HKEY_USERS, HKEY_CURRENT_USER, HKEY_CLASSES_ROOT, or HKEY_CURRENT_CONFIG). For remote registry access, this value will not include the computer name.
        /// </summary>
        public static string A_LoopRegKey
        {
            get { return string.Empty; }
            /*set { Settings.LoopRegKey = value; }*/
        }

        /// <summary>
        /// Name of the currently retrieved item, which can be either a value name or the name of a subkey. Value names displayed by Windows RegEdit as "(Default)" will be retrieved if a value has been assigned to them, but A_LoopRegName will be blank for them.
        /// </summary>
        public static string A_LoopRegName
        {
            get { return string.Empty; }
            /*set { Settings.LoopRegName = value; }*/
        }

        /// <summary>
        /// Name of the current SubKey. This will be the same as the Key parameter unless the Recurse parameter is being used to recursively explore other subkeys. In that case, it will be the full path of the currently retrieved item, not including the root key. For example: Software\SomeApplication\My SubKey
        /// </summary>
        public static string A_LoopRegSubkey
        {
            get { return string.Empty; }
            /*set { Settings.LoopRegSubkey = value; }*/
        }

        /// <summary>
        /// The time the current subkey or any of its values was last modified. Format YYYYMMDDHH24MISS. This variable will be empty if the currently retrieved item is not a subkey (i.e. A_LoopRegType is not the word KEY) or if the operating system is Win9x (since Win9x does not track this info).
        /// </summary>
        public static string A_LoopRegTimeModified
        {
            get { return string.Empty; }
            /*set { Settings.LoopRegTimeModified = value; }*/
        }

        /// <summary>
        /// The type of the currently retrieved item, which is one of the following words: KEY (i.e. the currently retrieved item is a subkey not a value), REG_SZ, REG_EXPAND_SZ, REG_MULTI_SZ, REG_DWORD, REG_QWORD, REG_BINARY, REG_LINK, REG_RESOURCE_LIST, REG_FULL_RESOURCE_DESCRIPTOR, REG_RESOURCE_REQUIREMENTS_LIST, REG_DWORD_BIG_ENDIAN (probably rare on most Windows hardware). It will be empty if the currently retrieved item is of an unknown type.
        /// </summary>
        public static string A_LoopRegType
        {
            get { return string.Empty; }
            /*set { Settings.LoopRegType = value; }*/
        }

        #endregion

        /// <summary>
        /// Current 2-digit day of the month (01-31).
        /// </summary>
        public static int A_MDay { get { return DateTime.Now.Day; } }

        /// <summary>
        /// Current 2-digit minute (00-59).
        /// </summary>
        public static int A_Min { get { return DateTime.Now.Minute; } }

        /// <summary>
        /// Current 2-digit month (01-12). Synonymous with A_Mon.
        /// </summary>
        public static int A_MM { get { return A_Mon; } }

        /// <summary>
        /// Current month's abbreviation in the current user's language, e.g. Jul
        /// </summary>
        public static string A_MMM
        {
            get { return string.Empty; }
            /*set { Settings.MMM = value; }*/
        }

        /// <summary>
        /// Current month's full name in the current user's language, e.g. July
        /// </summary>
        public static int A_MMMM { get { return 0; } }

        /// <summary>
        /// Current 2-digit month (01-12).
        /// </summary>
        public static int A_Mon { get { return DateTime.Now.Month; } }

        /// <summary>
        /// The current delay set by SetMouseDelay (always decimal, not hex). This delay is for the traditional SendEvent mode, not SendPlay.
        /// </summary>
        public static string A_MouseDelay
        {
            get { return string.Empty; }
            /*set { Settings.MouseDelay = value; }*/
        }

        /// <summary>
        /// Current 3-digit millisecond (000-999). To remove the leading zeros, follow this example: Milliseconds := A_MSec + 0
        /// </summary>
        public static int A_MSec { get { return DateTime.Now.Millisecond; } }

        /// <summary>
        /// The full path and name of the current user's "My Documents" folder. Unlike most of the similar variables, if the folder is the root of a drive, the final backslash is not included. For example, it would contain M: rather than M:\
        /// </summary>
        public static string A_MyDocuments { get { return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); } }

        /// <summary>
        /// The current local time in YYYYMMDDHH24MISS format. Note: Date and time math can be performed with EnvAdd and EnvSub. Also, FormatTime can format the date and/or time according to your locale or preferences.
        /// </summary>
        public static string A_Now
        {
            get { return string.Empty; }
            /* set { Settings.Now = value; } */
        }

        /// <summary>
        /// The current Coordinated Universal Time (UTC) in YYYYMMDDHH24MISS format. UTC is essentially the same as Greenwich Mean Time (GMT).
        /// </summary>
        public static string A_NowUTC
        {
            get
            {
                DateTime time = DateTime.UtcNow;
                string yyyy, MM, dd, HH, MI, SS;
                yyyy = time.Year.ToString();
                MM = time.Month.ToString();
                if (MM.Length==1)
                    MM = "0" + MM;
                dd = time.Day.ToString();
                if (dd.Length==1)
                    dd = "0" + dd;
                HH = time.Hour.ToString();
                if (HH.Length==1)
                    HH = "0" + HH;
                MI = time.Minute.ToString();
                if (MI.Length==1)
                    MI = "0" + MI;
                SS = time.Second.ToString();
                if (SS.Length==1)
                    SS = "0" + SS;
                return yyyy + MM + dd + HH + MI + SS;
            }
            /*set { Settings.NowUTC = value; }*/
        }

        /// <summary>
        /// See <see cref="A_BatchLines"/>.
        /// </summary>
        [Obsolete]
        public static string A_NumBatchLines
        {
            get { return A_BatchLines; }
            /*set { Settings.NumBatchLines = value; }*/
        }

        /// <summary>
        /// The type of Operating System being run.  Either WIN32_WINDOWS (i.e. Windows 95/98/ME) or WIN32_NT (i.e. Windows NT4/2000/XP/2003/Vista).
        /// </summary>
        public static string A_OSType
        {
            get { return string.Empty; }
            /*set { Settings.OSType = value; }*/
        }

        /// <summary>
        /// One of the following strings: WIN_VISTA [requires v1.0.44.13+], WIN_2003, WIN_XP, WIN_2000, WIN_NT4, WIN_95, WIN_98, WIN_ME.
        /// </summary>
        public static string A_OSVersion
        {
            get { return string.Empty; }
            /*set { Settings.OSVersion = value; }*/
        }

        /// <summary>
        /// Same as above except for the previous hotkey. It will be blank if none.
        /// </summary>
        public static string A_PriorHotkey
        {
            get { return string.Empty; }
            /*set { Settings.PriorHotkey = value; }*/
        }

        /// <summary>
        /// The Program Files directory (e.g. C:\Program Files). In v1.0.43.08+, the A_ prefix may be omitted, which helps ease the transition to #NoEnv.
        /// </summary>
        public static string A_ProgramFiles { get { return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles); } }

        /// <summary>
        /// The full path and name of the Programs folder in the current user's Start Menu.
        /// </summary>
        public static string A_Programs { get { return Environment.GetFolderPath(Environment.SpecialFolder.Programs); } }

        /// <summary>
        /// The full path and name of the Programs folder in the all-users Start Menu.
        /// </summary>
        public static string A_ProgramsCommon
        {
            get { return string.Empty; }
            /*set { Settings.ProgramsCommon = value; }*/
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
                IntPtr handle = Windows.Windows.GetDesktopWindow(); /* get the hDC of the target window */
                Windows.Windows.RECT windowRect; 
                Windows.Windows.GetWindowRect(handle, out windowRect);
                return windowRect.Bottom - windowRect.Top;
            }
            /*set { Settings.ScreenHeight = value; }*/
        }

        /// <summary>
        /// See <see cref="A_ScreenHeight"/>.
        /// </summary>
        public static int A_ScreenWidth
        {
            get
            {
                IntPtr handle = Windows.Windows.GetDesktopWindow(); /* get the hDC of the target window */
                Windows.Windows.RECT windowRect; 
                Windows.Windows.GetWindowRect(handle, out windowRect);
                return windowRect.Right - windowRect.Left;
            }
                //return ""; }
            /*set { Settings.ScreenWidth = value; }*/
        }

        /// <summary>
        /// The full path of the directory where the current script is located. For backward compatibility with AutoIt v2, the final backslash is included only for .aut scripts (even for root drives). An example for .aut scripts is C:\My Documents\
        /// </summary>
        public static string A_ScriptDir
        {
            get { return string.Empty; }
            /*set { Settings.ScriptDir = value; }*/
        }

        /// <summary>
        /// The combination of the above two variables to give the complete file specification of the script, e.g. C:\My Documents\My Script.ahk
        /// </summary>
        public static string A_ScriptFullPath
        {
            get { return string.Empty; }
            /*set { Settings.ScriptFullPath = value; }*/
        }

        /// <summary>
        /// The file name of the current script, without its path, e.g. MyScript.ahk.
        /// </summary>
        public static string A_ScriptName
        {
            get { return string.Empty; }
            /*set { Settings.ScriptName = value; }*/
        }

        /// <summary>
        /// Current 2-digit second (00-59).
        /// </summary>
        public static int A_Sec { get { return DateTime.Now.Second; } }

        /// <summary>
        /// This variable contains a single space character. See AutoTrim for details.
        /// </summary>
        public static string A_Space { get { return " "; } }

        /// <summary>
        /// The full path and name of the current user's Start Menu folder.
        /// </summary>
        public static string A_StartMenu
        {
            get { return string.Empty; }
            /*set { Settings.StartMenu = value; }*/
        }

        /// <summary>
        /// The full path and name of the all-users Start Menu folder.
        /// </summary>
        public static string A_StartMenuCommon
        {
            get { return string.Empty; }
            /*set { Settings.StartMenuCommon = value; }*/
        }

        /// <summary>
        /// The full path and name of the Startup folder in the current user's Start Menu.
        /// </summary>
        public static string A_Startup { get { return Environment.GetFolderPath(Environment.SpecialFolder.Startup); } }

        /// <summary>
        /// The full path and name of the Startup folder in the all-users Start Menu.
        /// </summary>
        public static string A_StartupCommon { get { return string.Empty; } }

        /// <summary>
        /// The current mode (On, Off, or Locale) set by StringCaseSense.
        /// </summary>
        public static string A_StringCaseSense
        {
            get { return string.Empty; }
            /*set { Settings.StringCaseSense = value; }*/
        }

        /// <summary>
        /// This variable contains a single tab character. See AutoTrim for details.
        /// </summary>
        public static string A_Tab { get { return "\t"; } }

        /// <summary>
        /// The full path and name of the folder designated to hold temporary files (e.g. C:\DOCUME~1\UserName\LOCALS~1\Temp). It is retrieved from one of the following locations (in order): 1) the environment variables TMP, TEMP, or USERPROFILE; 2) the Windows directory. On Windows 9x, A_WorkingDir is returned if neither TMP nor TEMP exists.
        /// </summary>
        public static string A_Temp { get { return Path.GetTempPath(); } }

        /// <summary>
        /// Temporary file name.
        /// </summary>
        public static string A_TempFile { get { return Path.GetTempFileName(); } }

        /// <summary>
        /// The name of the user-defined function that is currently executing (blank if none); for example: MyFunction
        /// </summary>
        public static string A_ThisFunc
        {
            get { return string.Empty; }
            /*set { Settings.ThisFunc = value; }*/
        }

        /// <summary>
        /// <para>The key name of the most recently executed hotkey or hotstring (blank if none), e.g. #z. This value will change if the current thread is interrupted by another hotkey, so be sure to copy it into another variable immediately if you need the original value for later use in a subroutine.</para>
        /// <para>When a hotkey is first created -- either by the Hotkey command or a double-colon label in the script -- its key name and the ordering of its modifier symbols becomes the permanent name of that hotkey. See also: A_ThisLabel</para>
        /// </summary>
        public static string A_ThisHotkey
        {
            get { return string.Empty; }
            /*set { Settings.ThisHotkey = value; }*/
        }

        /// <summary>
        /// The name of the label (subroutine) that is currently executing (blank if none); for example: MyLabel. It is updated whenever the script executes Gosub/Return or Goto. It is also updated for automatically-called labels such as timers, GUI threads, menu items, hotkeys, hotstrings, OnClipboardChange, and OnExit, However, A_ThisLabel is not updated when execution "falls into" a label from above; when that happens, A_ThisLabel retains its previous value. See also: A_ThisHotkey
        /// </summary>
        public static string A_ThisLabel
        {
            get { return string.Empty; }
            /*set { Settings.ThisLabel = value; }*/
        }

        /// <summary>
        /// The name of the menu from which A_ThisMenuItem was selected.
        /// </summary>
        public static string A_ThisMenu
        {
            get { return string.Empty; }
            /*set { Settings.ThisMenu = value; }*/
        }

        /// <summary>
        /// The name of the most recently selected custom menu item (blank if none).
        /// </summary>
        public static string A_ThisMenuItem
        {
            get { return string.Empty; }
            /*set { Settings.ThisMenuItem = value; }*/
        }

        /// <summary>
        /// A number indicating the current position of A_ThisMenuItem within A_ThisMenu. The first item in the menu is 1, the second is 2, and so on. Menu separator lines are counted. This variable is blank if A_ThisMenuItem is blank or no longer exists within A_ThisMenu. It is also blank if A_ThisMenu itself no longer exists.
        /// </summary>
        public static string A_ThisMenuItemPos
        {
            get { return string.Empty; }
            /*set { Settings.ThisMenuItemPos = value; }*/
        }

        /// <summary>
        /// The number of milliseconds since the computer was rebooted.
        /// </summary>
        public static int A_TickCount { get { return Environment.TickCount; } }

        /// <summary>
        /// The number of milliseconds that have elapsed since the system last received keyboard, mouse, or other input. This is useful for determining whether the user is away. This variable will be blank unless the operating system is Windows 2000, XP, or beyond. Physical input from the user as well as artificial input generated by any program or script (such as the Send or MouseMove commands) will reset this value back to zero. Since this value tends to increase by increments of 10, do not check whether it is equal to another value. Instead, check whether it is greater or less than another value. For example: IfGreater, A_TimeIdle, 600000, MsgBox, The last keyboard or mouse activity was at least 10 minutes ago.
        /// </summary>
        public static string A_TimeIdle
        {
            get { return string.Empty; }
            /*set { Settings.TimeIdle = value; }*/
        }

        /// <summary>
        /// Same as above but ignores artificial keystrokes and/or mouse clicks whenever the corresponding hook (keyboard or mouse) is installed. If neither hook is installed, this variable is equivalent to A_TimeIdle. If only one hook is present, only that one type of artificial input will be ignored. A_TimeIdlePhysical may be more useful than A_TimeIdle for determining whether the user is truly present.
        /// </summary>
        public static string A_TimeIdlePhysical
        {
            get { return string.Empty; }
            /*set { Settings.TimeIdlePhysical = value; }*/
        }

        /// <summary>
        /// The number of milliseconds that have elapsed since A_PriorHotkey was pressed. It will be -1 whenever A_PriorHotkey is blank.
        /// </summary>
        public static string A_TimeSincePriorHotkey
        {
            get { return string.Empty; }
            /*set { Settings.TimeSincePriorHotkey = value; }*/
        }

        /// <summary>
        /// The number of milliseconds that have elapsed since A_ThisHotkey was pressed. It will be -1 whenever A_ThisHotkey is blank.
        /// </summary>
        public static string A_TimeSinceThisHotkey
        {
            get { return string.Empty; }
            /*set { Settings.TimeSinceThisHotkey = value; }*/
        }

        /// <summary>
        /// The current mode set by SetTitleMatchMode: 1, 2, 3, or RegEx.
        /// </summary>
        public static string A_TitleMatchMode
        {
            get { return string.Empty; }
            /*set { Settings.TitleMatchMode = value; }*/
        }

        /// <summary>
        /// The current match speed (fast or slow) set by SetTitleMatchMode.
        /// </summary>
        public static string A_TitleMatchModeSpeed
        {
            get { return string.Empty; }
            /*set { Settings.TitleMatchModeSpeed = value; }*/
        }

        /// <summary>
        /// The logon name of the current user.
        /// </summary>
        public static string A_UserName
        {
            get { return string.Empty; }
            /*set { Settings.UserName = value; }*/
        }

        /// <summary>
        /// Current 1-digit day of the week (1-7). 1 is Sunday in all locales.
        /// </summary>
        public static string A_WDay
        {
            get { return string.Empty; }
            /*set { Settings.WDay = value; }*/
        }

        /// <summary>
        /// The current delay set by SetWinDelay (always decimal, not hex).
        /// </summary>
        public static string A_WinDelay
        {
            get { return Settings.WinDelay.ToString(); }
            /*set { Settings.WinDelay = value; }*/
        }

        /// <summary>
        /// The Windows directory. For example: C:\Windows
        /// </summary>
        public static string A_WinDir { get { return Environment.GetFolderPath(Environment.SpecialFolder.System); } }

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
        public static string A_YDay { get { return string.Empty; } }

        /// <summary>
        /// Current 4-digit year (e.g. 2004). Note: To retrieve a formatted time or date appropriate for your locale and language, use "FormatTime, OutputVar" (time and long date) or "FormatTime, OutputVar,, LongDate" (retrieves long-format date).
        /// </summary>
        public static int A_Year { get { return DateTime.Now.Year; } }

        /// <summary>
        /// Current year and week number (e.g. 200453) according to ISO 8601. To separate the year from the week, use StringLeft, Year, A_YWeek, 4 and StringRight, Week, A_YWeek, 2. Precise definition of A_YWeek: If the week containing January 1st has four or more days in the new year, it is considered week 1. Otherwise, it is the last week of the previous year, and the next week is week 1.
        /// </summary>
        public static int A_YWeek { get { return 0; } }

        /// <summary>
        /// See <see cref="A_Year"/>.
        /// </summary>
        public static int A_YYYY { get { return A_Year; } }

        /// <summary>
        /// HTTP User-Agent.
        /// </summary>
        public static string A_UserAgent
        {
            get { return Settings.UserAgent; }
            set { Settings.UserAgent = value; }
        }
    }
}
