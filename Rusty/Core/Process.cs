using System;
using System.Diagnostics;
using System.Security;

namespace IronAHK.Rusty
{
    partial class Core
    {
        // TODO: organise Process.cs

        /// <summary>
        /// Performs one of the following operations on a process: checks if it exists; changes its priority; closes it; waits for it to close.
        /// </summary>
        /// <param name="command">
        /// <para>One of the following words:</para>
        /// <para>Exist: Sets ErrorLevel to the Process ID (PID) if a matching process exists, or 0 otherwise. If the PID-or-Name parameter is blank, the script's own PID is retrieved. An alternate, single-line method to retrieve the script's PID is PID := DllCall("GetCurrentProcessId")</para>
        /// <para>Close: If a matching process is successfully terminated, ErrorLevel is set to its former Process ID (PID). Otherwise (there was no matching process or there was a problem terminating it), it is set to 0. Since the process will be abruptly terminated -- possibly interrupting its work at a critical point or resulting in the loss of unsaved data in its windows (if it has any) -- this method should be used only if a process cannot be closed by using WinClose on one of its windows.</para>
        /// <para>List: Although List is not yet supported, the examples section demonstrates how to retrieve a list of processes via DllCall.</para>
        /// <para>Priority: Changes the priority (as seen in Windows Task Manager) of the first matching process to Param3 and sets ErrorLevel to its Process ID (PID). If the PID-or-Name parameter is blank, the script's own priority will be changed. If there is no matching process or there was a problem changing its priority, ErrorLevel is set to 0.</para>
        /// <para>Param3 should be one of the following letters or words: L (or Low), B (or BelowNormal), N (or Normal), A (or AboveNormal), H (or High), R (or Realtime). Since BelowNormal and AboveNormal are not supported on Windows 95/98/Me/NT4, normal will be automatically substituted for them on those operating systems. Note: Any process not designed to run at Realtime priority might reduce system stability if set to that level.</para>
        /// <para>Wait: Waits up to Param3 seconds (can contain a decimal point) for a matching process to exist. If Param3 is omitted, the command will wait indefinitely. If a matching process is discovered, ErrorLevel is set to its Process ID (PID). If the command times out, ErrorLevel is set to 0.</para>
        /// <para>WaitClose: Waits up to Param3 seconds (can contain a decimal point) for ALL matching processes to close. If Param3 is omitted, the command will wait indefinitely. If all matching processes are closed, ErrorLevel is set to 0. If the command times out, ErrorLevel is set to the Process ID (PID) of the first matching process that still exists.</para>
        /// </param>
        /// <param name="name">
        /// <para>This parameter can be either a number (the PID) or a process name as described below. It can also be left blank to change the priority of the script itself.</para>
        /// <para>PID: The Process ID, which is a number that uniquely identifies one specific process (this number is valid only during the lifetime of that process). The PID of a newly launched process can be determined via the Run command. Similarly, the PID of a window can be determined with WinGet. The Process command itself can also be used to discover a PID.</para>
        /// <para>Name: The name of a process is usually the same as its executable (without path), e.g. notepad.exe or winword.exe. Since a name might match multiple running processes, only the first process will be operated upon. The name is not case sensitive.</para>
        /// </param>
        /// <param name="arg">See Cmd above for details.</param>
        public static void Process(string command, string name, string arg)
        {
            var prc = string.IsNullOrEmpty(name) ? System.Diagnostics.Process.GetCurrentProcess() : FindProcess(name);
            var none = prc == null;
            const int scale = 1000;
            command = command.ToLowerInvariant();
            ErrorLevel = none ? 0 : prc.Id;

            if (none && command != Keyword_WaitClose)
                return;

            switch (command)
            {
                case Keyword_Exist:
                    break;

                case Keyword_Close:
                    try { prc.Kill(); }
                    catch (System.ComponentModel.Win32Exception) { }
                    break;

                case Keyword_Priority:
                    arg = string.IsNullOrEmpty(arg) ? string.Empty : arg.ToLowerInvariant();

                    if (arg.Length == 1)
                    {
                        foreach (var mode in new[] { Keyword_Low, Keyword_BelowNormal, Keyword_Normal, Keyword_AboveNormal, Keyword_High, Keyword_Realtime })
                            if (mode[0] == arg[0])
                                arg = mode;
                    }

                    switch (arg.ToLowerInvariant())
                    {
                        case Keyword_Low: prc.PriorityClass = ProcessPriorityClass.Idle; break;
                        case Keyword_BelowNormal: prc.PriorityClass = ProcessPriorityClass.BelowNormal; break;
                        case Keyword_Normal: prc.PriorityClass = ProcessPriorityClass.Normal; break;
                        case Keyword_AboveNormal: prc.PriorityClass = ProcessPriorityClass.AboveNormal; break;
                        case Keyword_High: prc.PriorityClass = ProcessPriorityClass.High; break;
                        case Keyword_Realtime: prc.PriorityClass = ProcessPriorityClass.RealTime; break;
                    }
                    break;

                case Keyword_Wait:
                    {
                        int t = -1;
                        double d;

                        if (!string.IsNullOrEmpty(arg) && double.TryParse(arg, out d))
                            t = (int)(d * scale);

                        var start = Environment.TickCount;

                        while (0 == (ErrorLevel = FindProcess(name).Id))
                        {
                            System.Threading.Thread.Sleep(LoopFrequency);

                            if (t != -1 && Environment.TickCount - start > t)
                                break;
                        }
                    }
                    break;

                case Keyword_WaitClose:
                    if (string.IsNullOrEmpty(arg))
                        prc.WaitForExit();
                    else
                    {
                        double d;

                        if (double.TryParse(arg, out d))
                            prc.WaitForExit((int)(d * scale));
                        else
                            prc.WaitForExit();
                    }
                    break;
            }
        }

        /// <summary>
        /// Runs an external program. Unlike Run, RunWait will wait until the program finishes before continuing.
        /// </summary>
        /// <param name="Target">
        /// <para>A document, URL, executable file (.exe, .com, .bat, etc.), shortcut (.lnk), or system verb to launch (see remarks). If Target is a local file and no path was specified with it, A_WorkingDir will be searched first. If no matching file is found there, the system will search for and launch the file if it is integrated ("known"), e.g. by being contained in one of the PATH folders.</para>
        /// <para>To pass parameters, add them immediately after the program or document name. If a parameter contains spaces, it is safest to enclose it in double quotes (even though it may work without them in some cases).</para>
        /// </param>
        /// <param name="WorkingDir">The working directory for the launched item. Do not enclose the name in double quotes even if it contains spaces. If omitted, the script's own working directory (A_WorkingDir) will be used.</param>
        /// <param name="ShowMode">
        /// <para>If omitted, Target will be launched normally. Alternatively, it can contain one or more of these words: </para>
        /// <para>Max: launch maximized</para>
        /// <para>Min: launch minimized</para>
        /// <para>Hide: launch hidden (cannot be used in combination with either of the above)</para>
        /// <para>Note: Some applications (e.g. Calc.exe) do not obey the requested startup state and thus Max/Min/Hide will have no effect.</para>
        /// <para>UseErrorLevel: UseErrorLevel can be specified alone or in addition to one of the above words (by separating it from the other word with a space). If the launch fails, this option skips the warning dialog, sets ErrorLevel to the word ERROR, and allows the current thread to continue. If the launch succeeds, RunWait sets ErrorLevel to the program's exit code, and Run sets it to 0.</para>
        /// <para>When UseErrorLevel is specified, the variable A_LastError is set to the result of the operating system's GetLastError() function. A_LastError is a number between 0 and 4294967295 (always formatted as decimal, not hexadecimal). Zero (0) means success, but any other number means the launch failed. Each number corresponds to a specific error condition (to get a list, search www.microsoft.com for "system error codes"). Like ErrorLevel, A_LastError is a per-thread setting; that is, interruptions by other threads cannot change it. However, A_LastError is also set by DllCall.</para>
        /// </param>
        /// <param name="OutputVarPID">
        /// <para>The name of the variable in which to store the newly launched program's unique Process ID (PID). The variable will be made blank if the PID could not be determined, which usually happens if a system verb, document, or shortcut is launched rather than a direct executable file. RunWait also supports this parameter, though its OuputVarPID must be checked in another thread (otherwise, the PID will be invalid because the process will have terminated by the time the line following RunWait executes).</para>
        /// <para>After the Run command retrieves a PID, any windows to be created by the process might not exist yet. To wait for at least one window to be created, use WinWait ahk_pid %OutputVarPID%</para>
        /// </param>
        /// <param name="Wait"></param>
        public static void Run(string Target, string WorkingDir, string ShowMode, out int OutputVarPID, bool Wait)
        {
            var prc = new Process();
            prc.StartInfo.UseShellExecute = true;
            prc.StartInfo.FileName = Target;
            if (WorkingDir != null)
                prc.StartInfo.WorkingDirectory = WorkingDir;

            bool UseErrorLevel = false;

            if (!string.IsNullOrEmpty(runUser))
                prc.StartInfo.UserName = runUser;

            if (!string.IsNullOrEmpty(runDomain))
                prc.StartInfo.Domain = runDomain;

            if (runPassword != null || runPassword.Length != 0)
                prc.StartInfo.Password = runPassword;

            switch ((ShowMode).Trim().Substring(2, 1).ToLowerInvariant().ToCharArray()[0])
            {
                case 'x': prc.StartInfo.WindowStyle = ProcessWindowStyle.Maximized; break;
                case 'n': prc.StartInfo.WindowStyle = ProcessWindowStyle.Minimized; break;
                case 'd': prc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden; break;
                case 'e': UseErrorLevel = true; break;
            }

            ErrorLevel = 0;
            try
            {
                prc.Start();
                if (Wait)
                    prc.WaitForExit();
            }
            catch (Exception)
            {
                if (UseErrorLevel)
                    ErrorLevel = 2;
                else if (Wait)
                    ErrorLevel = prc.ExitCode;
            }

            OutputVarPID = prc.Id;
        }

        /// <summary>
        /// Specifies a set of user credentials to use for all subsequent uses of <see cref="Run"/> and <see cref="RunWait"/>.
        /// </summary>
        /// <param name="User">The username.</param>
        /// <param name="Password">The password.</param>
        /// <param name="Domain">The user domain.</param>
        /// <remarks>Leave all parameters blank to use no credentials.</remarks>
        public static void RunAs(string User, string Password, string Domain)
        {
            runUser = User;
            runDomain = Domain;

            if (string.IsNullOrEmpty(Password))
                runPassword = null;
            else
            {
                runPassword = new SecureString();
                foreach (var sym in Password)
                    runPassword.AppendChar(sym);
                runPassword.MakeReadOnly();
            }
        }

        /// <summary>
        /// Runs an external program. Unlike Run, RunWait will wait until the program finishes before continuing.
        /// </summary>
        /// <param name="Target">
        /// <para>A document, URL, executable file (.exe, .com, .bat, etc.), shortcut (.lnk), or system verb to launch (see remarks). If Target is a local file and no path was specified with it, A_WorkingDir will be searched first. If no matching file is found there, the system will search for and launch the file if it is integrated ("known"), e.g. by being contained in one of the PATH folders.</para>
        /// <para>To pass parameters, add them immediately after the program or document name. If a parameter contains spaces, it is safest to enclose it in double quotes (even though it may work without them in some cases).</para>
        /// </param>
        /// <param name="WorkingDir">The working directory for the launched item. Do not enclose the name in double quotes even if it contains spaces. If omitted, the script's own working directory (A_WorkingDir) will be used.</param>
        /// <param name="ShowMode">
        /// <para>If omitted, Target will be launched normally. Alternatively, it can contain one or more of these words: </para>
        /// <para>Max: launch maximized</para>
        /// <para>Min: launch minimized</para>
        /// <para>Hide: launch hidden (cannot be used in combination with either of the above)</para>
        /// <para>Note: Some applications (e.g. Calc.exe) do not obey the requested startup state and thus Max/Min/Hide will have no effect.</para>
        /// <para>UseErrorLevel: UseErrorLevel can be specified alone or in addition to one of the above words (by separating it from the other word with a space). If the launch fails, this option skips the warning dialog, sets ErrorLevel to the word ERROR, and allows the current thread to continue. If the launch succeeds, RunWait sets ErrorLevel to the program's exit code, and Run sets it to 0.</para>
        /// <para>When UseErrorLevel is specified, the variable A_LastError is set to the result of the operating system's GetLastError() function. A_LastError is a number between 0 and 4294967295 (always formatted as decimal, not hexadecimal). Zero (0) means success, but any other number means the launch failed. Each number corresponds to a specific error condition (to get a list, search www.microsoft.com for "system error codes"). Like ErrorLevel, A_LastError is a per-thread setting; that is, interruptions by other threads cannot change it. However, A_LastError is also set by DllCall.</para>
        /// </param>
        /// <param name="OutputVarPID">
        /// <para>The name of the variable in which to store the newly launched program's unique Process ID (PID). The variable will be made blank if the PID could not be determined, which usually happens if a system verb, document, or shortcut is launched rather than a direct executable file. RunWait also supports this parameter, though its OuputVarPID must be checked in another thread (otherwise, the PID will be invalid because the process will have terminated by the time the line following RunWait executes).</para>
        /// <para>After the Run command retrieves a PID, any windows to be created by the process might not exist yet. To wait for at least one window to be created, use WinWait ahk_pid %OutputVarPID%</para>
        /// </param>
        public static void RunWait(string Target, string WorkingDir, string ShowMode, out int OutputVarPID)
        {
            Run(Target, WorkingDir, ShowMode, out OutputVarPID, true);
        }

        /// <summary>
        /// Shuts down, restarts, or logs off the system.
        /// </summary>
        /// <param name="Code">A combination of shutdown codes listed below.</param>
        public static void Shutdown(int Code)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                return;
            Windows.ExitWindowsEx((uint)Code, 0);
        }
    }
}
