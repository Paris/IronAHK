using System;
using System.Diagnostics;
using System.Security;

namespace IronAHK.Rusty
{
    partial class Core
    {
        /// <summary>
        /// Performs one of the following operations on a process: checks if it exists; changes its priority; closes it; waits for it to close.
        /// </summary>
        /// <param name="command">
        /// <list type="bullet">
        /// <item><term>Exist</term>: <description>set <see cref="ErrorLevel"/> to the process ID (PID) of the matching process;</description></item>
        /// <item><term>Close</term>: <description>terminate the process and set <see cref="ErrorLevel"/> to its PID;</description></item>
        /// <item><term>Priority</term>: <description>change the priority of the process to:
        /// <c>Low</c>, <c>BelowNormal</c>, <c>Normal</c>, <c>AboveNormal</c>, <c>High</c> or <c>RealTime</c> specified in <paramref name="arg"/>;</description></item>
        /// <item><term>Wait</term>: <description>wait <paramref name="arg"/> seconds for the process to exist;</description></item>
        /// <item><term>WaitClose</term>: <description>wait <paramref name="arg"/> seconds for the process to close.</description></item>
        /// </list>
        /// </param>
        /// <param name="name">A process name or PID. Leave blank to use the current running process.</param>
        /// <param name="arg">See <paramref name="command"/>.</param>
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
        /// Runs an external program.
        /// </summary>
        /// <param name="target">A document, URL, executable file, shortcut or system verb to launch.</param>
        /// <param name="workingDir">The working directory of the new process.</param>
        /// <param name="showMode">Optionally one of the following: <c>min</c> (minimised), <c>max</c> (maximised) or <c>hide</c> (hidden).</param>
        /// <param name="pid">The variable to store the newly created process ID.</param>
        /// <param name="wait"><c>true</c> to wait for the process to close before continuing, <c>false</c> otherwise.</param>
        public static void Run(string target, string workingDir, string showMode, out int pid, bool wait = false)
        {
            var prc = new Process { 
                StartInfo = new ProcessStartInfo 
                { 
                    UseShellExecute = true,
                    FileName = target,
                    WorkingDirectory = string.IsNullOrEmpty(workingDir) ? null : workingDir,
                    UserName = string.IsNullOrEmpty(runUser) ? null : runUser,
                    Domain = string.IsNullOrEmpty(runDomain) ? null : runDomain,
                    Password = (runPassword == null || runPassword.Length == 0) ? null : runPassword,
 
                } 
            };

            if(prc.StartInfo.UserName != null || prc.StartInfo.Domain != null) {
                prc.StartInfo.UseShellExecute = false;
            }

            var error = false;
            switch (showMode.ToLowerInvariant())
            {
                case Keyword_Max: prc.StartInfo.WindowStyle = ProcessWindowStyle.Maximized; break;
                case Keyword_Min: prc.StartInfo.WindowStyle = ProcessWindowStyle.Minimized; break;
                case Keyword_Hide: prc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden; break;
                case Keyword_UseErrorLevel: error = true; break;
            }

            ErrorLevel = 0;
            pid = 0;
            try
            {
                prc.Start();

                if (wait)
                    prc.WaitForExit();

                pid = prc.Id;
            }
            catch (Exception)
            {
                if (error)
                    ErrorLevel = 2;
                else if (wait)
                    ErrorLevel = prc.ExitCode;
            }
        }

        /// <summary>
        /// Specifies a set of user credentials to use for all subsequent uses of <see cref="Run"/>.
        /// </summary>
        /// <param name="user">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="domain">The user domain.</param>
        /// <remarks>Leave all parameters blank to use no credentials.</remarks>
        public static void RunAs(string user, string password, string domain)
        {
            runUser = user;
            runDomain = domain;

            if (string.IsNullOrEmpty(password))
                runPassword = null;
            else
            {
                runPassword = new SecureString();
                foreach (var sym in password)
                    runPassword.AppendChar(sym);
                runPassword.MakeReadOnly();
            }
        }
        
        /// <summary>
        /// See <see cref="Run"/>.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="workingDir"></param>
        /// <param name="showMode"></param>
        /// <param name="pid"></param>
        [Obsolete]
        public static void RunWait(string target, string workingDir, string showMode, out int pid)
        {
            Run(target, workingDir, showMode, out pid, true);
        }

        /// <summary>
        /// Shuts down, restarts, or logs off the system.
        /// </summary>
        /// <param name="code">A combination of the following codes:
        /// <list type="bullet">
        /// <item><term>0</term>: <description>logoff</description></item>
        /// <item><term>1</term>: <description>shutdown</description></item>
        /// <item><term>2</term>: <description>reboot</description></item>
        /// <item><term>4</term>: <description>force</description></item>
        /// <item><term>8</term>: <description>power down</description></item>
        /// </list>
        /// </param>
        public static void Shutdown(int code)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                return;
            WindowsAPI.ExitWindowsEx((uint)code, 0);
        }
    }
}
