using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using IronAHK.Rusty.Common;

namespace IronAHK.Rusty
{
    partial class Core
    {
        // TODO: organise Disk.cs

        /// <summary>
        /// Ejects/retracts the tray in a CD or DVD drive, or sets a drive's volume label.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="drive">The drive letter.</param>
        /// <param name="value"></param>
        public static void Drive(string command, string drive = null, string value = null)
        {
            var cmd = command.ToLowerInvariant();

            #region cmd Eject

            if(cmd == Keyword_Eject && string.IsNullOrEmpty(value)) {
                if(!string.IsNullOrEmpty(drive)) {
                    var lowDriv = Common.Drive.DriveProvider.CreateDrive(new DriveInfo(drive));
                    lowDriv.Eject();
                }
            }

            #endregion

            #region cmd Retract / Close

            if(cmd == Keyword_Eject && !string.IsNullOrEmpty(value) && value == "1") {
                if(!string.IsNullOrEmpty(drive)) {
                    var lowDriv = Common.Drive.DriveProvider.CreateDrive(new DriveInfo(drive));
                    lowDriv.Retract();
                }
            }

            #endregion

            #region cmd Label

            if(cmd == Keyword_Label) {
                if(!string.IsNullOrEmpty(drive)) {
                    try {
                        var drv = new DriveInfo(drive);
                        drv.VolumeLabel = string.IsNullOrEmpty(value) ? "" : value;
                    } catch(Exception) {
                        ErrorLevel = 1;
                        return;
                    }
                }

            }

            #endregion

            #region cmd Lock

            if(cmd == Keyword_Lock) {
                if(!string.IsNullOrEmpty(drive)) {

                }
            }

            #endregion

            #region cmd UnLock

            if(cmd == Keyword_Unlock) {
                if(!string.IsNullOrEmpty(drive)) {

                }
            }

            #endregion
        }

        /// <summary>
        /// Retrieves various types of information about the computer's drive(s).
        /// </summary>
        /// <param name="result">The name of the variable in which to store the result.</param>
        /// <param name="command"></param>
        /// <param name="value"></param>
        public static void DriveGet(out string result, string command, string value = null)
        {
            result = null;
            var cmd = command.ToLowerInvariant();

            #region cmd List

            if(cmd == Keyword_List){

                string matchingDevices = "";
                DriveType? type = null;

                if(!string.IsNullOrEmpty(value))
                    type = Mapper.MappingService.Instance.DriveType.LookUpCLRType(value);
                var drives = DriveInfo.GetDrives();
                
                for(int i=0; i < drives.Length; i++) {

                        if(type.HasValue) {
                            if(i == 0) continue; // prefromace hack: skip A:\\

                            try {
                                if(drives[i].DriveType == type.Value)
                                    matchingDevices += drives[i].Name.Substring(0, 1);
                            } catch {
                                // ignore
                            }
                        } else {
                            matchingDevices += drives[i].Name.Substring(0, 1);
                        }
                }
                result = matchingDevices;
            }

            #endregion

            #region cmd Capacity

            if(cmd == Keyword_Capacity) {
                if(!string.IsNullOrEmpty(value)){
                    try {
                        var drv = new DriveInfo(value);
                        result = drv.TotalSize.ToString();
                    } catch(ArgumentException) {
                        // value was not a valid label!
                        ErrorLevel = 1;
                        return;
                    }
                }
            }

            #endregion

            #region cmd Filesystem

            if(cmd == Keyword_FileSystem) {
                if(!string.IsNullOrEmpty(value)) {
                    try {
                        var drv = new DriveInfo(value);
                        result = drv.DriveFormat;
                    } catch(ArgumentException) {
                        // value was not a valid label!
                        ErrorLevel = 1;
                        return;
                    }
                }
            }

            #endregion

            #region cmd Type

            if(cmd == Keyword_Type) {
                if(!string.IsNullOrEmpty(value)) {
                    try {
                        var drv = new DriveInfo(value);
                        result = Mapper.MappingService.Instance.DriveType.LookUpIAType(drv.DriveType);
                    } catch {
                        // value was not a valid label!
                        ErrorLevel = 1;
                        return;
                    }
                }
            }

            #endregion

            #region cmd Label 

            if(cmd == Keyword_Label) {
                if(!string.IsNullOrEmpty(value)) {
                    try {
                        var drv = new DriveInfo(value);
                        result = drv.VolumeLabel;
                    } catch(ArgumentException) {
                        // value was not a valid label!
                        ErrorLevel = 1;
                        return;
                    }
                }
            }


            #endregion

            #region cmd Serial TODO!!

            if(cmd == Keyword_Serial) {

            }

            #endregion

            #region cmd Status

            if(cmd == Keyword_Status) {
                if(!string.IsNullOrEmpty(value)) {
                    try {
                        var drv = new DriveInfo(value);
                        var dummy = drv.DriveFormat; // provocate DriveNotFoundException on invalid paths
                        result = drv.IsReady ? "Ready" : "NotReady";
                    } catch(DriveNotFoundException) {
                        result = "Invalid";
                    } catch(ArgumentException) {
                        result = "Invalid";
                    }catch{
                        result = "Unknown";
                    }
                }
            }

            #endregion

            #region cmd StatusCD TODO!!

            if(cmd == Keyword_StatusCD) {

            }

            #endregion

        }

        /// <summary>
        /// Retrieves the free disk space of a drive, in megabytes.
        /// </summary>
        /// <param name="result">The variable in which to store the result.</param>
        /// <param name="path">Path of drive to receive information from.</param>
        public static void DriveSpaceFree(out double result, string path)
        {
            result = Math.Floor((double)(new DriveInfo(path)).TotalFreeSpace / 1024 / 1024);
        }
        
        /// <summary>
        /// Writes text to the end of a file, creating it first if necessary.
        /// </summary>
        /// <param name="text">The text to append to <paramref name="file"/>.</param>
        /// <param name="file">The name of the file to be appended.
        /// <list type="bullet">
        /// <item><term>Binary mode</term>: <description>to append in binary mode rather than text mode, prepend an asterisk.</description></item>
        /// <item><term>Standard output (stdout)</term>: <description>specifying an asterisk (*) causes <paramref name="text"/> to be written to the console.</description></item>
        /// </list>
        /// </param>
        public static void FileAppend(string text, string file)
        {
            try
            {
                if (file == "*")
                    Console.Write(text);
                else
                {
                    if (file.Length > 0 && file[0] == '*')
                    {
                        file = file.Substring(1);
                        var writer = new BinaryWriter(File.Open(file, FileMode.OpenOrCreate));
                        writer.Write(text);
                    }
                    else
                        File.AppendAllText(file, text);
                }

                ErrorLevel = 0;
            }
            catch (IOException)
            {
                ErrorLevel = 1;
            }
        }

        /// <summary>
        /// Copies one or more files.
        /// </summary>
        /// <param name="source">The name of a single file or folder, or a wildcard pattern.</param>
        /// <param name="destination">The name or pattern of the destination.</param>
        /// <param name="flag">
        /// <list type="bullet">
        /// <item><term>0</term>: <description>(default) do not overwrite existing files</description></item>
        /// <item><term>1</term>: <description>overwrite existing files</description></item>
        /// </list>
        /// </param>
        public static void FileCopy(string source, string destination, int flag = 0)
        {
            try
            {
                File.Copy(source, destination, flag != 0);
                ErrorLevel = 0;
            }
            catch (IOException)
            {
                ErrorLevel = 1;
            }
        }

        private static void CopyDirectory(string source, string destination, bool overwrite)
        {
            try
            {
                Directory.CreateDirectory(destination);
            }
            catch (IOException)
            {
                if (!overwrite)
                    throw;
            }
            
            foreach (string filepath in Directory.GetFiles(source))
            {
                string basename = Path.GetFileName(filepath);
                string destfile = Path.Combine(destination, basename);
                File.Copy(filepath, destfile, overwrite);
            }
            
            foreach (string dirpath in Directory.GetDirectories(source))
            {
                string basename = Path.GetFileName(dirpath);
                string destdir = Path.Combine(destination, basename);
                CopyDirectory(dirpath, destdir, overwrite);
            }
        }
        
        /// <summary>
        /// Copies a folder along with all its sub-folders and files.
        /// </summary>
        /// <param name="source">Path of the source directory.</param>
        /// <param name="destination">Path of the destination directory.</param>
        /// <param name="flag">
        /// <list type="bullet">
        /// <item><term>0</term>: <description>(default) do not overwrite existing files</description></item>
        /// <item><term>1</term>: <description>overwrite existing files</description></item>
        /// </list>
        /// </param>
        public static void FileCopyDir(string source, string destination, int flag = 0)
        {
            var overwrite = (flag & 1) == 1;

            try
            {
                destination = Path.GetFullPath(destination);

                CopyDirectory(source, destination, overwrite);
            }
            catch (IOException)
            {
                ErrorLevel = 1;
            }
        }

        /// <summary>
        /// Creates a directory.
        /// </summary>
        /// <param name="path">Path of the directory to create.</param>
        public static void FileCreateDir(string path)
        {
            try
            {
                Directory.CreateDirectory(path);
                ErrorLevel = Directory.Exists(path) ? 0 : 1;
            }
            catch (IOException)
            {
                ErrorLevel = 1;
            }
        }

        /// <summary>
        /// Creates a shortcut to a file.
        /// </summary>
        /// <param name="target">Path to the shortcut file.</param>
        /// <param name="link">The file referenced by the shortcut.</param>
        /// <param name="workingDir">The working directory.</param>
        /// <param name="args">Arguments to start <paramref name="link"/> with.</param>
        /// <param name="description">A summary of the shortcut.</param>
        /// <param name="icon"></param>
        /// <param name="shortcutKey">A hotkey activator.</param>
        /// <param name="iconNumber"></param>
        /// <param name="runState"></param>
        public static void FileCreateShortcut(string target, string link, string workingDir = null, string args = null, string description = null, string icon = null, string shortcutKey = null, int iconNumber = 0, int runState = 1)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes one or more files.
        /// </summary>
        /// <param name="pattern">The name of a file or a wildcard pattern.</param>
        public static void FileDelete(string pattern)
        {
            try
            {
                foreach (var file in Glob(pattern))
                    File.Delete(file);
                ErrorLevel = 0;
            }
            catch (IOException)
            {
                ErrorLevel = 1;
            }
        }

        /// <summary>
        /// Returns the attributes of a file if it exists.
        /// </summary>
        /// <param name="pattern">The name of a file or wildcard pattern.</param>
        /// <returns>A blank string if no files or folders are found, otheriwse the attributes of the first match.</returns>
        public static string FileExist(string pattern)
        {
            try
            {
                foreach (var file in Glob(pattern))
                    return FromFileAttribs(File.GetAttributes(file));

                return string.Empty;
            }
            catch (IOException)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Retrieves information about a shortcut file.
        /// </summary>
        /// <param name="link"></param>
        /// <param name="target"></param>
        /// <param name="workingDir"></param>
        /// <param name="args"></param>
        /// <param name="description"></param>
        /// <param name="icon"></param>
        /// <param name="iconNumber"></param>
        /// <param name="runState"></param>
        public static void FileGetShortcut(string link, out string target, out string workingDir, out string args, out string description, out string icon, out string iconNumber, out string runState)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves the size of a file.
        /// </summary>
        /// <param name="result">The name of the variable in which to store the retrieved size.</param>
        /// <param name="file">The name of the target file.</param>
        /// <param name="units">
        /// <para>If present, this parameter causes the result to be returned in units other than bytes:</para>
        /// <list type="bullet">
        /// <item><term>K</term>: <description>kilobytes</description></item>
        /// <item><term>M</term>: <description>megabytes</description></item>
        /// <item><term>G</term>: <description>gigabytes</description></item>
        /// </list>
        /// </param>
        public static void FileGetSize(out long result, string file, string units = null)
        {
            try
            {
                long size = (new FileInfo(file)).Length;
                const int scale = 1024;

                if (!string.IsNullOrEmpty(units))
                {
                    switch (units[0])
                    {
                        case 'k':
                        case 'K':
                            size /= scale;
                            break;

                        case 'm':
                        case 'M':
                            size /= scale * scale;
                            break;

                        case 'g':
                        case 'G':
                            size /= scale * scale * scale;
                            break;
                    }
                }

                result = size;
            }
            catch (Exception)
            {
                result = 0;
                ErrorLevel = 1;
            }
        }

        /// <summary>
        /// Retrieves the datetime stamp of a file or folder.
        /// </summary>
        /// <param name="result">The name of the variable in which to store the retrieved date-time in format YYYYMMDDHH24MISS in local time.</param>
        /// <param name="file">The name of the target file or folder.</param>
        /// <param name="time">
        /// <para>Which timestamp to retrieve:</para>
        /// <list type="bullet">
        /// <item><term>M</term>: <description>(default) modification time</description></item>
        /// <item><term>C</term>: <description>reation time</description></item>
        /// <item><term>A</term>: <description>last access time</description></item>
        /// </list>
        /// </param>
        public static void FileGetTime(out string result, string file, string time = "M")
        {
            if (!File.Exists(file))
            {
                result = string.Empty;
                ErrorLevel = 1;
                return;
            }

            var info = new FileInfo(file);
            var date = new DateTime();

            switch (time[0])
            {
                case 'm':
                case 'M':
                    date = info.LastWriteTime;
                    break;

                case 'c':
                case 'C':
                    date = info.CreationTime;
                    break;

                case 'a':
                case 'A':
                    date = info.LastAccessTime;
                    break;
            }

            result = FromTime(date).ToString();
        }

        /// <summary>
        /// Retrieves the version information of a file.
        /// </summary>
        /// <param name="result">The name of the variable in which to store the version number or string.</param>
        /// <param name="file">The name of the target file.</param>
        public static void FileGetVersion(out string result, string file)
        {
            result = string.Empty;
            try
            {
                var info = FileVersionInfo.GetVersionInfo(file);
                result = info.FileVersion;
                ErrorLevel = 0;
            }
            catch (Exception)
            {
                ErrorLevel = 1;
            }
        }

        /// <summary>
        /// Moves or renames one or more files.
        /// </summary>
        /// <param name="source">The name of a single file or a wildcard pattern.</param>
        /// <param name="destination">The name or pattern of the destination.</param>
        /// <param name="flag">
        /// <list type="bullet">
        /// <item><term>0</term>: <description>(default) do not overwrite existing files</description></item>
        /// <item><term>1</term>: <description>overwrite existing files</description></item>
        /// </list>
        /// </param>
        public static void FileMove(string source, string destination, int flag = 0)
        {
            try
            {
                if (source == destination)
                {
                    ErrorLevel = 1;
                    return;
                }
                if (File.Exists(destination))
                {
                    if (flag == 0)
                    {
                        ErrorLevel = 1;
                        return;
                    }
                    else
                    {
                        File.Delete(destination);
                    }
                }
                if (File.Exists(source))
                {
                        File.Move(source, destination);
                }
            }
            catch (Exception) { ErrorLevel = 2; }
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
            ErrorLevel = 0;

            switch (Flag)
            {
                case "0":
                    if (Directory.Exists(Dest))
                        return;
                    break;

                default:
                    ErrorLevel = 1;
                    return;
            }

            Directory.Move(Source, Dest);
        }

        /// <summary>
        /// Read the contents of a file.
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the retrieved content.</param>
        /// <param name="Filename">
        /// <para>The file path, optionally preceded by one or more of the following options:</para>
        /// <list type="bullet">
        /// <item><term>*c</term>: <description>treat the source as binary rather than text, <paramref name="OutputVar"/> will be a byte array.</description></item>
        /// <item><term>*m<c>n</c></term>: <description>stop reading at <c>n</c> bytes.</description></item>
        /// <item><term>*t</term>: <description>replace all occurrences of <c>`r`n</c> with <c>`n</c>. This option is ignored in binary mode.</description></item>
        /// </list>
        /// </param>
        public static void FileRead(out object OutputVar, string Filename)
        {
            #region Variables

            OutputVar = null;
            ErrorLevel = 0;

            if (string.IsNullOrEmpty(Filename))
            {
                ErrorLevel = 1;
                return;
            }

            #endregion

            #region Options

            bool binary = false, nocrlf = false;
            int i, max = -1;

            while ((i = Filename.IndexOf('*')) != -1)
            {
                int n = i + 1;

                if (n == Filename.Length)
                {
                    Filename = i == 0 ? string.Empty : Filename.Substring(0, i);
                    break;
                }

                char mode = Filename[n++];

                switch (mode)
                {
                    case 'c':
                    case 'C':
                        binary = true;
                        break;

                    case 't':
                    case 'T':
                        nocrlf = true;
                        break;

                    case 'm':
                    case 'M':
                        int s = n;
                        while (n < Filename.Length && char.IsDigit(Filename, n))
                            n++;
                        if (s < n)
                            max = int.Parse(Filename.Substring(s, n - s));
                        break;
                }

                if (n == Filename.Length)
                    Filename = Filename.Substring(0, n);
                else if (i == 0)
                    Filename = Filename.Substring(n);
                else
                    Filename = Filename.Substring(0, i) + Filename.Substring(n);
            }

            if (max == 0)
                return;

            if (Filename.Length == 0)
            {
                ErrorLevel = 1;
                return;
            }

            #endregion

            #region Read

            if (binary)
            {
                try
                {
                    if (max == -1)
                        OutputVar = File.ReadAllBytes(Filename);
                    else
                        OutputVar = new BinaryReader(File.OpenRead(Filename)).ReadBytes(max);
                }
                catch (Exception)
                {
                    ErrorLevel = 1;
                    return;
                }
            }
            else
            {
                string text;

                try
                {
                    text = File.ReadAllText(Filename);
                }
                catch (Exception)
                {
                    ErrorLevel = 1;
                    return;
                }

                if (max != -1)
                    text = text.Substring(0, max);

                if (nocrlf)
                    text = text.Replace("\r\n", "\n");

                OutputVar = text;
            }

            #endregion
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
                var sr = new StreamReader(Filename);
                string line = string.Empty;
                for (int i = 0; i < LineNum; i++)
                    line = sr.ReadLine();
                sr.Close();
                OutputVar = line;
                ErrorLevel = 0;
            }
            catch (Exception) { ErrorLevel = 1; }
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
                WindowsAPI.SHEmptyRecycleBin(IntPtr.Zero, Root, WindowsAPI.SHERB_NOCONFIRMATION | WindowsAPI.SHERB_NOPROGRESSUI | WindowsAPI.SHERB_NOSOUND);
                ErrorLevel = 0;
            }
            catch (Exception) { ErrorLevel = 1; }
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
                ErrorLevel = 0;
            }
            catch (Exception) { ErrorLevel = 1; }
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
                int error = 0;

                foreach (var path in ToFiles(FilePattern, OperateOnFolders != 2, OperateOnFolders != 0, Recurse != 0))
                {
                    FileAttributes set = ToFileAttribs(Attributes, File.GetAttributes(path));
                    File.SetAttributes(path, set);
                    if (File.GetAttributes(path) != set)
                        error++;
                }

                ErrorLevel = error;
            }
            catch (Exception) { ErrorLevel = 1; }
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
                int error = 0;

                foreach (var path in ToFiles(FilePattern, OperateOnFolders != 2, OperateOnFolders != 0, Recurse != 0))
                {
                    var set = new DateTime();

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

                ErrorLevel = error;
            }
            catch (Exception) { ErrorLevel = 1; }
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
        /// <param name="path">Name of the variable containing the file name to be analyzed.</param>
        /// <param name="filename">Name of the variable in which to store the file name without its path. The file's extension is included.</param>
        /// <param name="directory">Name of the variable in which to store the directory of the file, including drive letter or share name (if present). The final backslash is not included even if the file is located in a drive's root directory.</param>
        /// <param name="extension">Name of the variable in which to store the file's extension (e.g. TXT, DOC, or EXE). The dot is not included.</param>
        /// <param name="name">Name of the variable in which to store the file name without its path, dot and extension.</param>
        /// <param name="root">Name of the variable in which to store the drive letter or server name of the file. If the file is on a local or mapped drive, the variable will be set to the drive letter followed by a colon (no backslash). If the file is on a network path (UNC), the variable will be set to the share name, e.g. \\Workstation01</param>
        public static void SplitPath(ref string path, out string filename, out string directory, out string extension, out string name, out string root)
        {
            var input = path;

            try
            {
                input = Path.GetFullPath(path);
            }
            catch (ArgumentException)
            {
                ErrorLevel = 1;
                filename = directory = extension = name = root = null;
                return;
            }

            filename = Path.GetFileName(input);
            directory = Path.GetDirectoryName(input);
            extension = Path.GetExtension(input);
            name = Path.GetFileNameWithoutExtension(input);
            root = Path.GetPathRoot(input);
        }
    }
}
