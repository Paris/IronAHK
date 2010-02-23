using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace IronAHK.Rusty
{
    partial class Core
    {
        // TODO: organise Disk.cs

        /// <summary>
        /// Ejects/retracts the tray in a CD or DVD drive, or sets a drive's volume label.
        /// </summary>
        /// <param name="Command"></param>
        /// <param name="Drive">The drive letter followed by a colon and an optional backslash (might also work on UNCs and mapped drives), e.g. C: or D:\</param>
        /// <param name="Value"></param>
        public static void Drive(string Command, string Drive, string Value)
        {

        }

        /// <summary>
        /// Retrieves various types of information about the computer's drive(s).
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the result of <paramref name="Cmd"/>.</param>
        /// <param name="Cmd"></param>
        /// <param name="Value"></param>
        public static void DriveGet(out string OutputVar, string Cmd, string Value)
        {
            OutputVar = null;
        }

        /// <summary>
        /// Retrieves the free disk space of a drive, in Megabytes.
        /// </summary>
        /// <param name="OutputVar">The variable in which to store the result, which is rounded down to the nearest whole number.</param>
        /// <param name="Path">Path of drive to receive information from. Since NTFS supports mounted volumes and directory junctions, different amounts of free space might be available in different folders of the same "drive" in some cases.</param>
        public static void DriveSpaceFree(out double OutputVar, string Path)
        {
            OutputVar = Math.Floor((double)(new DriveInfo(Path)).TotalFreeSpace / 1024 / 1024);
        }
        
        /// <summary>
        /// Writes text to the end of a file, creating it first if necessary.
        /// </summary>
        /// <param name="Text">The text to append to the file.</param>
        /// <param name="Filename">The name of the file to be appended.
        /// <list type="bullet">
        /// <item><description>Binary mode: to append in binary mode rather than text mode, prepend an asterisk to the <paramref name="Filename"/>.</description></item>
        /// <item><description>Standard output (stdout): specifying an asterisk (*) for <paramref name="Filename"/> causes <paramref name="Text"/> to be written to the console.</description></item>
        /// </list>
        /// </param>
        public static void FileAppend(string Text, string Filename)
        {
            try
            {
                if (Filename == "*")
                    Console.Write(Text);
                else
                {
                    if (Filename.Length > 0 && Filename[0] == '*')
                    {
                        Filename = Filename.Substring(1);
                        var writer = new BinaryWriter(File.Open(Filename, FileMode.OpenOrCreate));
                        writer.Write(Text);
                    }
                    else
                        File.AppendAllText(Filename, Text);
                }

                error = 0;
            }
            catch (Exception)
            {
                error = 1;
            }
        }

        /// <summary>
        /// Copies one or more files.
        /// </summary>
        /// <param name="Source">The name of a single file or folder, or a wildcard pattern such as C:\Temp\*.tmp. SourcePattern is assumed to be in %A_WorkingDir% if an absolute path isn't specified.</param>
        /// <param name="Dest">The name or pattern of the destination, which is assumed to be in %A_WorkingDir% if an absolute path isn't specified. To perform a simple copy -- retaining the existing file name(s) -- specify only the folder name as shown in these functionally identical examples:
        /// <example>
        /// FileCopy, C:\*.txt, C:\My Folder
        /// FileCopy, C:\*.txt, C:\My Folder\*.*
        /// </example>
        /// </param>
        /// <param name="Flag">
        /// 0 = (default) do not overwrite existing files
        /// 1 = overwrite existing files
        /// </param>
        public static void FileCopy(string Source, string Dest, int Flag)
        {
            try
            {
                File.Copy(Source, Dest, Flag != 0);
                error = 0;
            }
            catch (Exception) { error = 1; }
        }

        /// <summary>
        /// Copies a folder along with all its sub-folders and files (similar to <code>xcopy</code>).
        /// </summary>
        /// <param name="Source">Name of the source directory (with no trailing backslash), which is assumed to be in %A_WorkingDir% if an absolute path isn't specified. For example: <code>C:\My Folder</code></param>
        /// <param name="Dest">Name of the destination dir (with no trailing baskslash), which is assumed to be in %A_WorkingDir% if an absolute path isn't specified. For example: <code>C:\Copy of My Folder</code></param>
        /// <param name="Flag">
        /// 0 (default): Do not overwrite existing files. The operation will fail and have no effect if Dest already exists as a file or directory.
        /// 1: Overwrite existing files. However, any files or subfolders inside Dest that do not have a counterpart in Source will not be deleted.
        /// </param>
        public static void FileCopyDir(string Source, string Dest, int Flag)
        {
            String[] Files;

            if (Dest[Dest.Length - 1] != Path.DirectorySeparatorChar)
                Dest += Path.DirectorySeparatorChar;
            if (!Directory.Exists(Dest)) Directory.CreateDirectory(Dest);
            Files = Directory.GetFileSystemEntries(Source);
            foreach (string Element in Files)
            {
                // Sub directories

                if (Directory.Exists(Element))
                    FileCopyDir(Element, Dest + Path.GetFileName(Element), Flag);
                // Files in directory

                else
                {
                    try
                    {
                        File.Copy(Element, Dest + Path.GetFileName(Element), Flag == 1);
                    }
                    catch (Exception) { };
                }
            }
        }

        static void FileCopyDir(string Source, string Dest)
        {
            String[] Files;

            if (Dest[Dest.Length - 1] != Path.DirectorySeparatorChar)
                Dest += Path.DirectorySeparatorChar;
            if (!Directory.Exists(Dest)) Directory.CreateDirectory(Dest);
            Files = Directory.GetFileSystemEntries(Source);
            foreach (string Element in Files)
            {
                // Sub directories

                if (Directory.Exists(Element))
                    FileCopyDir(Element, Dest + Path.GetFileName(Element), 0);
                // Files in directory

                else
                {
                    try
                    {
                        File.Copy(Element, Dest + Path.GetFileName(Element), false);
                    }
                    catch (Exception) { };
                }
            }
        }

        /// <summary>
        /// Creates a directory/folder.
        /// </summary>
        /// <param name="Path">Name of the directory to create, which is assumed to be in %A_WorkingDir% if an absolute path isn't specified.</param>
        public static void FileCreateDir(string Path)
        {
            try
            {
                Directory.CreateDirectory(Path);
                error = 0;
            }
            catch (Exception) { error = 1; }
        }

        /// <summary>
        /// Creates a shortcut (.lnk) file.
        /// </summary>
        /// <param name="Target">Name of the file that the shortcut refers to, which should include an absolute path unless the file is integrated with the system (e.g. Notepad.exe). The file does not have to exist at the time the shortcut is created; in other words, shortcuts to invalid targets can be created.</param>
        /// <param name="LinkFile">Name of the shortcut file to be created, which is assumed to be in %A_WorkingDir% if an absolute path isn't specified. Be sure to include the .lnk extension. If the file already exists, it will be overwritten.</param>
        /// <param name="WorkingDir">Directory that will become Target's current working directory when the shortcut is launched. If blank or omitted, the shortcut will have a blank "Start in" field and the system will provide a default working directory when the shortcut is launched.</param>
        /// <param name="Args">Parameters that will be passed to Target when it is launched. Separate parameters with spaces. If a parameter contains spaces, enclose it in double quotes.</param>
        /// <param name="Description">Comments that describe the shortcut (used by the OS to display a tooltip, etc.)</param>
        /// <param name="IconFile">The full path and name of the icon to be displayed for LinkFile. It must either be an ico file or the very first icon of an EXE or DLL.</param>
        /// <param name="ShortcutKey">
        /// <para>A single letter, number, or the name of a single key from the key list (mouse buttons and other non-standard keys might not be supported). Do not include modifier symbols. Currently, all shortcut keys are created as CTRL+ALT shortcuts. For example, if the letter B is specified for this parameter, the shortcut key will be CTRL-ALT-B.</para>
        /// <para>For Windows 9x: A reboot might be required to get the shortcut key into effect. Alternatively, you can open the properties dialog for the shortcut and recreate the shortcut key to get it into effect immediately.</para>
        /// </param>
        /// <param name="IconNumber">To use an icon in IconFile other than the first, specify that number here (can be an expression). For example, 2 is the second icon.</param>
        /// <param name="RunState">
        /// <para>To have Target launched minimized or maximized, specify one of the following digits:</para>
        /// <list type="">
        /// <item>1 - Normal (this is the default)</item>
        /// <item>3 - Maximized</item>
        /// <item>7 - Minimized</item>
        /// </list>
        /// </param>
        public static void FileCreateShortcut(string Target, string LinkFile, string WorkingDir, string Args, string Description, string IconFile, string ShortcutKey, int IconNumber, int RunState)
        {

        }

        /// <summary>
        /// Deletes one or more files.
        /// </summary>
        /// <param name="FilePattern">
        /// <para>The name of a single file or a wildcard pattern such as C:\Temp\*.tmp. FilePattern is assumed to be in %A_WorkingDir% if an absolute path isn't specified.</para>
        /// <para>To remove an entire folder, along with all its sub-folders and files, use FileRemoveDir.</para>
        /// </param>
        public static void FileDelete(string FilePattern)
        {
            try
            {
                File.Delete(FilePattern);
                error = 0;
            }
            catch (Exception) { error = 1; }
        }

        /// <summary>
        /// Returns a blank value (empty string) if FilePattern does not exist (FilePattern is assumed to be in A_WorkingDir if an absolute path isn't specified). Otherwise, it returns the attribute string (a subset of "RASHNDOCT") of the first matching file or folder. If the file has no attributes (rare), "X" is returned. FilePattern may be the exact name of a file or folder, or it may contain wildcards (* or ?). Since an empty string is seen as "false", the function's return value can always be used as a quasi-boolean value. For example, the statement if FileExist("C:\My File.txt") would be true if the file exists and false otherwise. Similarly, the statement if InStr(FileExist("C:\My Folder"), "D") would be true only if the file exists and is a directory. Corresponding commands: IfExist and FileGetAttrib.
        /// </summary>
        /// <param name="FilePattern"></param>
        /// <returns></returns>
        public static string FileExist(string FilePattern)
        {
            try { return FromFileAttribs(File.GetAttributes(FilePattern)); }
            catch (Exception) { return string.Empty; }
        }

        /// <summary>
        /// Reports whether a file or folder is read-only, hidden, etc.
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the retrieved text.</param>
        /// <param name="Filename">The name of the target file, which is assumed to be in %A_WorkingDir% if an absolute path isn't specified. If omitted, the current file of the innermost enclosing File-Loop will be used instead.</param>
        public static void FileGetAttrib(out string OutputVar, string Filename)
        {
            OutputVar = string.Empty;
            try
            {
                OutputVar = FromFileAttribs(File.GetAttributes(Filename));
                error = 0;
            }
            catch (Exception) { error = 1; }
        }

        /// <summary>
        /// Retrieves information about a shortcut (.lnk) file, such as its target file.
        /// </summary>
        /// <param name="LinkFile">Name of the shortcut file to be analyzed, which is assumed to be in %A_WorkingDir% if an absolute path isn't specified. Be sure to include the .lnk extension.</param>
        /// <param name="OutTarget">Name of the variable in which to store the shortcut's target (not including any arguments it might have). For example: C:\WINDOWS\system32\notepad.exe</param>
        /// <param name="OutDir">Name of the variable in which to store the shortcut's working directory. For example: C:\My Documents. If environment variables such as %WinDir% are present in the string, one way to resolve them is via StringReplace. For example: StringReplace, OutDir, OutDir, `%WinDir`%, %A_WinDir%</param>
        /// <param name="OutArgs">Name of the variable in which to store the shortcut's parameters (blank if none).</param>
        /// <param name="OutDescription">Name of the variable in which to store the shortcut's comments (blank if none).</param>
        /// <param name="OutIcon">Name of the variable in which to store the filename of the shortcut's icon (blank if none).</param>
        /// <param name="OutIconNum">Name of the variable in which to store the shortcut's icon number within the icon file (blank if none). This value is most often 1, which means the first icon. </param>
        /// <param name="OutRunState">
        /// <para>Name of the variable in which to store the shortcut's initial launch state, which is one of the following digits:</para>
        /// <list type="">
        /// <item>1: Normal</item>
        /// <item>3: Maximized</item>
        /// <item>7: Minimized</item>
        /// </list>
        /// </param>
        public static void FileGetShortcut(string LinkFile, string OutTarget, string OutDir, string OutArgs, string OutDescription, string OutIcon, string OutIconNum, string OutRunState)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves the size of a file.
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the retrieved size (rounded down to the nearest whole number).</param>
        /// <param name="Filename">The name of the target file, which is assumed to be in %A_WorkingDir% if an absolute path isn't specified. If omitted, the current file of the innermost enclosing File-Loop will be used instead.</param>
        /// <param name="Units">
        /// <para>If present, this parameter causes the result to be returned in units other than bytes:</para>
        /// <list type="">
        /// <item>K = kilobytes</item>
        /// <item>M = megabytes</item>
        /// </list>
        /// </param>
        public static void FileGetSize(out long OutputVar, string Filename, string Units)
        {
            try
            {
                long size = (new FileInfo(Filename)).Length;

                if (Units.Length != 0)
                {
                    switch (Units[0])
                    {
                        case 'k':
                        case 'K':
                            size /= 1024;
                            break;

                        case 'm':
                        case 'M':
                            size /= 1024 * 1024;
                            break;

                        case 'g':
                        case 'G':
                            size /= 1024 * 1024 * 1024;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                OutputVar = size;
            }
            catch (Exception)
            {
                OutputVar = 0;
                error = 1;
            }
        }

        /// <summary>
        /// Retrieves the datetime stamp of a file or folder.
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the retrieved date-time in format YYYYMMDDHH24MISS. The time is your own local time, not UTC/GMT.</param>
        /// <param name="Filename">The name of the target file or folder, which is assumed to be in %A_WorkingDir% if an absolute path isn't specified. If omitted, the current file of the innermost enclosing File-Loop will be used instead.</param>
        /// <param name="WhichTime">
        /// <para>Which timestamp to retrieve:</para>
        /// <list type="">
        /// <item>M = Modification time (this is the default if the parameter is omitted)</item>
        /// <item>C = Creation time</item>
        /// <item>A = Last access time </item>
        /// </list>
        /// </param>
        public static void FileGetTime(out string OutputVar, string Filename, string WhichTime)
        {
            FileInfo file = new FileInfo(Filename);
            DateTime time = new DateTime();

            switch (WhichTime[0])
            {
                case 'm':
                case 'M':
                    time = file.LastWriteTime;
                    break;

                case 'c':
                case 'C':
                    time = file.CreationTime;
                    break;

                case 'a':
                case 'A':
                    time = file.LastAccessTime;
                    break;
            }

            OutputVar = FromTime(time).ToString();
        }

        /// <summary>
        /// Retrieves the version of a file.
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the version number/string.</param>
        /// <param name="Filename">The name of the target file, which is assumed to be in %A_WorkingDir% if an absolute path isn't specified. If omitted, the current file of the innermost enclosing File-Loop will be used instead.</param>
        /// <remarks>Most non-executable files (and even some EXEs) won't have a version, and thus the <paramref name="OutputVar"/> will be blank in these cases.</remarks>
        public static void FileGetVersion(out string OutputVar, string Filename)
        {
            OutputVar = string.Empty;
            try
            {
                FileVersionInfo info = FileVersionInfo.GetVersionInfo(Filename);
                OutputVar = info.FileVersion;
                error = 0;
            }
            catch (Exception) { error = 1; }
        }



        /// <summary>
        /// Moves or renames one or more files.
        /// </summary>
        /// <param name="Source">The name of a single file or a wildcard pattern such as C:\Temp\*.tmp. SourcePattern is assumed to be in %A_WorkingDir% if an absolute path isn't specified.</param>
        /// <param name="Dest">The name or pattern of the destination, which is assumed to be in %A_WorkingDir% if an absolute path isn't specified. To perform a simple move -- retaining the existing file name(s) -- specify only the folder name as shown in these functionally identical examples:
        /// <code>FileMove, C:\*.txt, C:\My Folder</code>
        /// <code>FileMove, C:\*.txt, C:\My Folder\*.*</code>
        /// </param>
        /// <param name="Flag">
        /// <para>(optional) this flag determines whether to overwrite files if they already exist:</para>
        /// <list type="">
        /// <item>0 = (default) do not overwrite existing files</item>
        /// <item>1 = overwrite existing files</item>
        /// </list>
        /// </param>
        public static void FileMove(string Source, string Dest, string Flag)
        {
            try
            {
                File.Move(Source, Dest);
                error = 0;
            }
            catch (Exception) { error = 1; }
        }

        /// <summary>
        /// Moves a folder along with all its sub-folders and files. It can also rename a folder.
        /// </summary>
        /// <param name="Source">Name of the source directory (with no trailing backslash), which is assumed to be in %A_WorkingDir% if an absolute path isn't specified. For example: C:\My Folder </param>
        /// <param name="Dest">The new path and name of the directory (with no trailing baskslash), which is assumed to be in %A_WorkingDir% if an absolute path isn't specified. For example: D:\My Folder. Note: Dest is the actual path and name that the directory will have after it is moved; it is not the directory into which Source is moved (except for the known limitation mentioned below). </param>
        /// <param name="Flag">
        /// <para>(options) Specify one of the following single characters:</para>
        /// <para>0 (default): Do not overwrite existing files. The operation will fail if Dest already exists as a file or directory.</para>
        /// <para>1: Overwrite existing files. However, any files or subfolders inside Dest that do not have a counterpart in Source will not be deleted. Known limitation: If Dest already exists as a folder and it is on the same volume as Source, Source will be moved into it rather than overwriting it. To avoid this, see the next option.</para>
        /// <para>2: The same as mode 1 above except that the limitation is absent.</para>
        /// <para>R: Rename the directory rather than moving it. Although renaming normally has the same effect as moving, it is helpful in cases where you want "all or none" behavior; that is, when you don't want the operation to be only partially successful when Source or one of its files is locked (in use). Although this method cannot move Source onto a different volume, it can move it to any other directory on its own volume. The operation will fail if Dest already exists as a file or directory.</para>
        /// </param>
        public static void FileMoveDir(string Source, string Dest, string Flag)
        {
            error = 0;

            switch (Flag)
            {
                case "0":
                    if (Directory.Exists(Dest))
                        return;
                    break;

                default:
                    error = 1;
                    return;
            }

            Directory.Move(Source, Dest);
        }

        /// <summary>
        /// Reads a file's text into a variable.
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the retrieved text. OutputVar will be made blank if a problem occurs such as the file being "in use" or not existing (in which case ErrorLevel is set to 1). It will also be made blank if Filename is an empty file (in which case ErrorLevel is set to 0).</param>
        /// <param name="Filename">
        /// <para>The name of the file to read, which is assumed to be in %A_WorkingDir% if an absolute path isn't specified.</para>
        /// <para>Options: Zero or more of the following strings may be also be present immediately before the name of the file. Separate each option from the next with a single space or tab. For example: *t *m5000 C:\Log Files\200601.txt</para>
        /// <para>*c: Load a ClipboardAll file. All other options are ignored when *c is present.</para>
        /// <para>*m1024: If this option is omitted, the entire file is loaded unless the file is larger than 1 GB, in which case it is not loaded at all. Otherwise, replace 1024 with a decimal or hexadecimal number of bytes between 1 and 1073741824 (1 GB). If the file is larger than this, only its leading part is loaded. Note: This might result in the last line ending in a naked carriage return (`r) rather than `r`n.</para>
        /// <para>*t: Replaces any/all occurrences of carriage return &amp; linefeed (`r`n) with linefeed (`n). However, this translation reduces performance and is usually not necessary. For example, text containing `r`n is already in the right format to be added to a Gui Edit control. Similarly, FileAppend detects the presence of `r`n when it opens a new file; it knows to write each `r`n as-is rather than translating it to `r`r`n. Finally, the following parsing loop will work correctly regardless of whether each line ends in `r`n or just `n: Loop, parse, MyFileContents, `n, `r</para>
        /// </param>
        public static void FileRead(out string OutputVar, string Filename)
        {
            string path = Filename;
            bool lf = false;
            int pos = 0, cut = -1, size = -1;

            while (0 != (pos = path.IndexOf('*')))
            {
                switch (path.Substring(pos, 1))
                {
                    //case "c": binary = true; break;
                    case "t": lf = true; break;
                    case "m":
                        int n, i = 0;
                        while (int.TryParse(path.Substring(++i + pos, 1), out n)) ;
                        size = int.Parse(path.Substring(pos, i)); // CHECK if not pos+1
                        break;
                }
                cut = pos;
            }

            OutputVar = (new StreamReader(cut == -1 ? path : path.Substring(cut))).ReadToEnd();
            if (size != -1)
                OutputVar = (OutputVar).Substring(0, size * 1024 * 1024);

            if (lf)
                OutputVar = (OutputVar).Replace("\r\n", "\n");
        }

        /// <summary>
        /// Reads the specified line from a file and stores the text in a variable.
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the retrieved text.</param>
        /// <param name="Filename">The name of the file to access, which is assumed to be in %A_WorkingDir% if an absolute path isn't specified. Windows and Unix formats are supported; that is, the file's lines may end in either carriage return and linefeed (`r`n) or just linefeed (`n).</param>
        /// <param name="LineNum">Which line to read (1 is the first, 2 the second, and so on).</param>
        public static void FileReadLine(out string OutputVar, string Filename, int LineNum)
        {
            OutputVar = string.Empty;
            try
            {
                StreamReader sr = new StreamReader(Filename);
                string line = string.Empty;
                for (int i = 0; i < LineNum; i++)
                    line = sr.ReadLine();
                sr.Close();
                OutputVar = line;
                error = 0;
            }
            catch (Exception) { error = 1; }
        }

        /// <summary>
        /// Sends a file or directory to the recycle bin, if possible.
        /// </summary>
        /// <param name="FilePattern">
        /// <para>The name of a single file or a wildcard pattern such as C:\Temp\*.tmp. FilePattern is assumed to be in %A_WorkingDir% if an absolute path isn't specified.</para>
        /// <para>To recycle an entire directory, provide its name without a trailing backslash.</para>
        /// </param>
        public static void FileRecycle(string FilePattern)
        {

        }

        /// <summary>
        /// Empties the recycle bin.
        /// </summary>
        /// <param name="Root">If omitted, the recycle bin for all drives is emptied. Otherwise, specify a drive letter such as C:\</param>
        public static void FileRecycleEmpty(string Root)
        {
            try
            {
                Windows.SHEmptyRecycleBin(IntPtr.Zero, Root, Windows.SHERB_NOCONFIRMATION | Windows.SHERB_NOPROGRESSUI | Windows.SHERB_NOSOUND);
                error = 0;
            }
            catch (Exception) { error = 1; }
        }

        /// <summary>
        /// Deletes a folder.
        /// </summary>
        /// <param name="Path">Name of the directory to delete, which is assumed to be in %A_WorkingDir% if an absolute path isn't specified.</param>
        /// <param name="Recurse">
        /// <list type="">
        /// <item>0 (default): Do not remove files and sub-directories contained in DirName. In this case, if DirName is not empty, no action will be taken and ErrorLevel will be set to 1.</item>
        /// <item>1: Remove all files and subdirectories (like the DOS DelTree command).</item>
        /// </list>
        /// </param>
        public static void FileRemoveDir(string Path, bool Recurse)
        {
            try
            {
                Directory.Delete(Path, Recurse);
                error = 0;
            }
            catch (Exception) { error = 1; }
        }

        /// <summary>
        /// Displays a standard dialog that allows the user to open or save file(s).
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the filename(s) selected by the user. This will be made blank if the user cancels the dialog (i.e. does not wish to select a file).</param>
        /// <param name="Options">
        /// <para>If omitted, it will default to zero, which is the same as having none of the options below.</para>
        /// <para>M: Multi-select. Specify the letter M to allow the user to select more than one file via shift-click, control-click, or other means. M may optionally be followed by a number as described below (for example, both M and M1 are valid). To extract the individual files, see the example at the bottom of this page.</para>
        /// <para>S: Save button. Specify the letter S to cause the dialog to always contain a Save button instead of an Open button. S may optionally be followed by a number (or sum of numbers) as described below (for example, both S and S24 are valid).</para>
        /// <para>Even if M and S are absent, the following numbers can be used. To put more than one of them into effect, add them up. For example, to use 8 and 16, specify the number 24.</para>
        /// <list type="">
        /// <item>1: File Must Exist</item>
        /// <item>2: Path Must Exist</item>
        /// <item>8: Prompt to Create New File</item>
        /// <item>16: Prompt to OverWrite File</item>
        /// <item>32 [v1.0.43.09+]: Shortcuts (.lnk files) are selected as-is rather than being resolved to their targets. This option also prevents navigation into a folder via a folder shortcut.</item>
        /// </list>
        /// <para>If the "Prompt to Overwrite" option is present without the "Prompt to Create" option, the dialog will contain a Save button rather than an Open button. This behavior is due to a quirk in Windows.</para>
        /// </param>
        /// <param name="RootDir">
        /// <para>If present, this parameter contains one or both of the following:</para>
        /// <para>RootDir: The root (starting) directory, which is assumed to be a subfolder in %A_WorkingDir% if an absolute path is not specified. If omitted or blank, the starting directory will be a default that might depend on the OS version (in WinNT/2k/XP and beyond, it will likely be the directory most recently selected by the user during a prior use of FileSelectFile). In v1.0.43.10+, a CLSID such as ::{20d04fe0-3aea-1069-a2d8-08002b30309d} (i.e. My Computer) may also be specified, in which case any subdirectory present after the CLSID should end in a backslash (otherwise, the string after the last backslash will be interpreted as the default filename, below).</para>
        /// <para>Filename: The default filename to initially show in the dialog's edit field. Only the naked filename (with no path) will be shown. To ensure that the dialog is properly shown, ensure that no illegal characters are present (such as /&lt;|:").</para>
        /// </param>
        /// <param name="Prompt">Text displayed in the window to instruct the user what to do. If omitted or blank, it will default to "Select File - %A_SCRIPTNAME%" (i.e. the name of the current script).</param>
        /// <param name="Filter">
        /// <para>Indicates which types of files are shown by the dialog.</para>
        /// <para>Example: Documents (*.txt)
        /// Example: Audio (*.wav; *.mp2; *.mp3)</para>
        /// <para>If omitted, the filter defaults to All Files (*.*). An option for Text Documents (*.txt) will also be available in the dialog's "files of type" menu.</para>
        /// <para>Otherwise, the filter uses the indicated string but also provides an option for All Files (*.*) in the dialog's "files of type" drop-down list. To include more than one file extension in the filter, separate them with semicolons as illustrated in the example above.</para>
        /// </param>
        public static void FileSelectFile(out string OutputVar, string Options, string RootDir, string Prompt, string Filter)
        {
            OutputVar = null;
        }

        /// <summary>
        /// Displays a standard dialog that allows the user to select a folder.
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the user's selected folder. This will be made blank if the user cancels the dialog (i.e. does not wish to select a folder). If the user selects a root directory (such as C:\), OutputVar will contain a trailing backslash. If this is undesirable, remove it as follows:
        /// <code>FileSelectFolder, Folder
        /// Folder := RegExReplace(Folder, "\\$")  ; Removes the trailing backslash, if present.</code>
        /// </param>
        /// <param name="StartingFolder">
        /// <para>If blank or omitted, the dialog's initial selection will be the user's My Documents folder (or possibly My Computer). A CLSID folder such as ::{20d04fe0-3aea-1069-a2d8-08002b30309d} (i.e. My Computer) may be specified start navigation at a specific special folder.</para>
        /// <para>Otherwise, the most common usage of this parameter is an asterisk followed immediately by the absolute path of the drive or folder to be initially selected. For example, *C:\ would initially select the C drive. Similarly, *C:\My Folder would initially select that particular folder.</para>
        /// <para>The asterisk indicates that the user is permitted to navigate upward (closer to the root) from the starting folder. Without the asterisk, the user would be forced to select a folder inside StartingFolder (or StartingFolder itself). One benefit of omitting the asterisk is that StartingFolder is initially shown in a tree-expanded state, which may save the user from having to click the first plus sign.</para>
        /// <para>If the asterisk is present, upward navigation may optionally be restricted to a folder other than Desktop. This is done by preceding the asterisk with the absolute path of the uppermost folder followed by exactly one space or tab. In the following example, the user would not be allowed to navigate any higher than C:\My Folder (but the initial selection would be C:\My Folder\Projects):
        /// <code>C:\My Folder *C:\My Folder\Projects</code></para>
        /// </param>
        /// <param name="Options">
        /// <para>One of the following numbers:</para>
        /// <para>0: The options below are all disabled (except on Windows 2000, where the "make new folder" button might appear anyway).</para>
        /// <para>1 (default): A button is provided that allows the user to create new folders. However, the button will not be present on Windows 95/98/NT.</para>
        /// <para>Add 2 to the above number to provide an edit field that allows the user to type the name of a folder. For example, a value of 3 for this parameter provides both an edit field and a "make new folder" button.</para>
        /// <para>If the user types an invalid folder name in the edit field, OutputVar will be set to the folder selected in the navigation tree rather than what the user entered, at least on Windows XP.</para>
        /// </param>
        /// <param name="Prompt">Text displayed in the window to instruct the user what to do. If omitted or blank, it will default to "Select Folder - %A_SCRIPTNAME%" (i.e. the name of the current script).</param>
        public static void FileSelectFolder(out string OutputVar, string StartingFolder, string Options, string Prompt)
        {
            OutputVar = null;
        }

        /// <summary>
        /// Changes the attributes of one or more files or folders. Wildcards are supported.
        /// </summary>
        /// <param name="Attributes">The attributes to change (see Remarks).</param>
        /// <param name="FilePattern">
        /// <para>The name of a single file or folder, or a wildcard pattern such as C:\Temp\*.tmp. FilePattern is assumed to be in %A_WorkingDir% if an absolute path isn't specified.</para>
        /// <para>If omitted, the current file of the innermost enclosing File-Loop will be used instead.</para>
        /// </param>
        /// <param name="OperateOnFolders">
        /// <list type="">
        /// <item>0 (default) Folders are not operated upon (only files).</item>
        /// <item>1 All files and folders that match the wildcard pattern are operated upon.</item>
        /// <item>2 Only folders are operated upon (no files).</item>
        /// </list>
        /// <para>Note: If FilePattern is a single folder rather than a wildcard pattern, it will always be operated upon regardless of this setting.</para>
        /// </param>
        /// <param name="Recurse">
        /// <list type="">
        /// <item>0 (default) Subfolders are not recursed into.</item>
        /// <item>1 Subfolders are recursed into so that files and folders contained therein are operated upon if they match FilePattern. All subfolders will be recursed into, not just those whose names match FilePattern. However, files and folders with a complete path name longer than 259 characters are skipped over as though they do not exist. Such files are rare because normally, the operating system does not allow their creation.</item>
        /// </list>
        /// </param>
        public static void FileSetAttrib(string Attributes, string FilePattern, int OperateOnFolders, int Recurse)
        {
            try
            {
                error = 0;
                foreach (string path in ToFiles(FilePattern, OperateOnFolders != 2, OperateOnFolders != 0, Recurse != 0))
                {
                    FileAttributes set = ToFileAttribs(Attributes, File.GetAttributes(path));
                    File.SetAttributes(path, set);
                    if (File.GetAttributes(path) != set)
                        error++;
                }
            }
            catch (Exception) { error = 1; }
        }

        /// <summary>
        /// Changes the datetime stamp of one or more files or folders. Wildcards are supported.
        /// </summary>
        /// <param name="YYYYMMDDHH24MISS">If blank or omitted, it defaults to the current time. Otherwise, specify the time to use for the operation (see Remarks for the format). Years prior to 1601 are not supported.</param>
        /// <param name="FilePattern">
        /// <para>The name of a single file or folder, or a wildcard pattern such as C:\Temp\*.tmp. FilePattern is assumed to be in %A_WorkingDir% if an absolute path isn't specified.</para>
        /// <para>If omitted, the current file of the innermost enclosing File-Loop will be used instead.</para>
        /// </param>
        /// <param name="WhichTime">Which timestamp to set:
        /// <list type="">
        /// <item>M = Modification time (this is the default if the parameter is blank or omitted)</item>
        /// <item>C = Creation time</item>
        /// <item>A = Last access time </item>
        /// </list>
        /// </param>
        /// <param name="OperateOnFolders">
        /// <list type="">
        /// <item>0 (default) Folders are not operated upon (only files).</item>
        /// <item>1 All files and folders that match the wildcard pattern are operated upon.</item>
        /// <item>2 Only folders are operated upon (no files).</item>
        /// <para>Note: If FilePattern is a single folder rather than a wildcard pattern, it will always be operated upon regardless of this setting.</para>
        /// </list>
        /// </param>
        /// <param name="Recurse">
        /// <list type="">
        /// <item>0 (default) Subfolders are not recursed into.</item>
        /// <item>1 Subfolders are recursed into so that files and folders contained therein are operated upon if they match FilePattern. All subfolders will be recursed into, not just those whose names match FilePattern. However, files and folders with a complete path name longer than 259 characters are skipped over as though they do not exist. Such files are rare because normally, the operating system does not allow their creation.</item>
        /// </list>
        /// </param>
        public static void FileSetTime(string YYYYMMDDHH24MISS, string FilePattern, string WhichTime, int OperateOnFolders, int Recurse)
        {
            DateTime time = ToDateTime(YYYYMMDDHH24MISS);

            try
            {
                error = 0;
                foreach (string path in ToFiles(FilePattern, OperateOnFolders != 2, OperateOnFolders != 0, Recurse != 0))
                {
                    DateTime set = new DateTime();

                    switch (WhichTime[0])
                    {
                        case 'm':
                        case 'M':
                            File.SetLastWriteTime(path, time);
                            set = File.GetLastWriteTime(path);
                            break;

                        case 'c':
                        case 'C':
                            File.SetCreationTime(path, time);
                            set = File.GetCreationTime(path);
                            break;

                        case 'a':
                        case 'A':
                            File.SetLastAccessTime(path, time);
                            set = File.GetLastAccessTime(path);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (set != time)
                        error++;
                }
            }
            catch (Exception) { error = 1; }
        }

        /// <summary>
        /// Changes the script's current working directory.
        /// </summary>
        /// <param name="DirName">The name of the new working directory, which is assumed to be a subfolder of the current %A_WorkingDir% if an absolute path isn't specified.</param>
        public static void SetWorkingDir(string DirName)
        {
            Environment.CurrentDirectory = DirName;
        }

        /// <summary>
        /// Separates a file name or URL into its name, directory, extension, and drive.
        /// </summary>
        /// <param name="InputVar">Name of the variable containing the file name to be analyzed.</param>
        /// <param name="OutFileName">Name of the variable in which to store the file name without its path. The file's extension is included.</param>
        /// <param name="OutDir">Name of the variable in which to store the directory of the file, including drive letter or share name (if present). The final backslash is not included even if the file is located in a drive's root directory.</param>
        /// <param name="OutExtension">Name of the variable in which to store the file's extension (e.g. TXT, DOC, or EXE). The dot is not included.</param>
        /// <param name="OutNameNoExt">Name of the variable in which to store the file name without its path, dot and extension.</param>
        /// <param name="OutDrive">Name of the variable in which to store the drive letter or server name of the file. If the file is on a local or mapped drive, the variable will be set to the drive letter followed by a colon (no backslash). If the file is on a network path (UNC), the variable will be set to the share name, e.g. \\Workstation01</param>
        public static void SplitPath(string InputVar, out string OutFileName, out string OutDir, out string OutExtension, out string OutNameNoExt, out string OutDrive)
        {
            try { InputVar = Path.GetFullPath(InputVar); }
            catch (Exception) { error = 1; }

            OutFileName = Path.GetFileName(InputVar);
            OutDir = Path.GetDirectoryName(InputVar);
            OutExtension = Path.GetExtension(InputVar);
            OutNameNoExt = Path.GetFileNameWithoutExtension(InputVar);
            OutDrive = Path.GetPathRoot(InputVar);
        }

        /// <summary>
        /// Downloads a file from the Internet.
        /// </summary>
        /// <param name="URL">URL of the file to download. For example, http://www.example.com might retrieve the welcome page for that organization.</param>
        /// <param name="Filename">Specify the name of the file to be created locally, which is assumed to be in %A_WorkingDir% if an absolute path isn't specified. Any existing file will be overwritten by the new file.</param>
        public static void URLDownloadToFile(string URL, string Filename)
        {
            int z = URL.IndexOf('*');
            if (z != -1) // i.e. *0 http://...
            {
                for (; z < URL.Length; z++)
                    if (char.IsWhiteSpace(URL, z))
                        break;
                URL = URL.Substring(z);
            }

            try { (new WebClient()).DownloadFile(URL, Filename); }
            catch (Exception) { error = 1; }
        }
    }
}