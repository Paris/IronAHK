using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using ThreadState = System.Threading.ThreadState;
using Timer = System.Timers.Timer;

namespace IronAHK.Rusty
{
    partial class Core
    {
        /// <summary>
        /// Prevents the current thread from being interrupted by other threads.
        /// </summary>
        /// <param name="mode">
        /// <list type="bullet">
        /// <item><term>On</term>: <description>give the current thread the highest priority.</description></item>
        /// <item><term>Off</term>: <description>resets the current thread priority to normal.</description></item>
        /// </list>
        /// </param>
        public static void Critical(string mode)
        {
            bool on = OnOff(mode) ?? true;
            System.Threading.Thread.CurrentThread.Priority = on ? ThreadPriority.Highest : ThreadPriority.Normal;
        }

        /// <summary>
        /// Exits the current thread or the entire program if non-persistent.
        /// </summary>
        /// <param name="exitCode">An integer that is returned to the caller.</param>
        public static void Exit(int exitCode)
        {
            if (ApplicationExit != null)
                ApplicationExit(null, null);

            Environment.ExitCode = exitCode;
            Application.ExitThread();
        }

        /// <summary>
        /// Terminates the program unconditionally.
        /// </summary>
        /// <param name="exitCode">An integer that is returned to the caller.</param>
        public static void ExitApp(int exitCode)
        {
            if (ApplicationExit != null)
                ApplicationExit(null, null);

            Environment.Exit(exitCode);
        }

        /// <summary>
        /// Checks if a function is defined.
        /// </summary>
        /// <param name="name">The name of a function.</param>
        /// <returns><c>true</c> if the specified function exists in the current scope, <c>false</c> otherwise.</returns>
        public static bool IsFunc(string name)
        {
            return FindLocalMethod(name) != null;
        }

        /// <summary>
        /// Checks if a label is defined.
        /// </summary>
        /// <param name="name">The name of a label.</param>
        /// <returns><c>true</c> if the specified label exists in the current scope, <c>false</c> otherwise.</returns>
        public static bool IsLabel(string name)
        {
            string method = LabelMethodName(name);
            return FindLocalMethod(method) != null;
        }

        /// <summary>
        /// Specifies a label to run automatically when the program exits.
        /// </summary>
        /// <param name="label">The name of a label. Leave blank to remove an existing label, if any.</param>
        public static void OnExit(string label)
        {
            ErrorLevel = 0;

            if (onExit != null)
            {
                AppDomain.CurrentDomain.ProcessExit -= onExit;
                onExit = null;
            }

            if (string.IsNullOrEmpty(label))
                return;

            var method = FindLocalRoutine(label);

            if (method == null)
            {
                ErrorLevel = 1;
                return;
            }

            var handler = new EventHandler(delegate
                                               {
                try { method.Invoke(null, new object[] { new object[] { } }); }
                catch (Exception) { }
            });

            AppDomain.CurrentDomain.ProcessExit += handler;
            onExit = handler;
        }

        /// <summary>
        /// Specifies a function to call automatically when the program receives the specified message.
        /// </summary>
        /// <param name="number">The number of the message to monitor.</param>
        /// <param name="function">The name of a function to call whenever the specified message is received.</param>
        /// <param name="maxThreads">The maximum number of concurrent threads to launch per message number.</param>
        public static void OnMessage(string number, string function, string maxThreads)
        {
            // TODO: onmessage
        }

        /// <summary>
        /// Sends a string to the debugger (if any) for display.
        /// </summary>
        /// <param name="text">The text to send to the debugger for display.</param>
        public static void OutputDebug(string text)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                WindowsAPI.OutputDebugString(text);
        }

        /// <summary>
        /// Pauses the current thread.
        /// </summary>
        /// <param name="mode">
        /// <list type="bullet">
        /// <item><term>Toggle</term> (default): <description>pauses the current thread unless the thread beneath it is paused, in which case the underlying thread is unpaused.</description></item>
        /// <item><term>On</term>: <description>pauses the current thread.</description></item>
        /// <item><term>Off</term>: <description>if the thread beneath the current thread is paused, it will be in an unpaused state when resumed.</description></item>
        /// </list>
        /// </param>
        /// <param name="parentThread">
        /// <list type="bullet">
        /// <item><term>0</term>: <description>pause the current thread.</description></item>
        /// <item><term>1</term>: <description>marks the thread beneath the current thread as paused so that when it resumes, it will finish the command it was running (if any) and then enter a paused state. If there is no thread beneath the current thread, the program itself is paused, which prevents timers from running.</description></item>
        /// </list>
        /// </param>
        public static void Pause(string mode, bool parentThread)
        {
            var thread = System.Threading.Thread.CurrentThread;
            var state = OnOff(mode);

            if (state == null && mode.Equals(Keyword_Toggle, StringComparison.OrdinalIgnoreCase))
                state = !(thread.ThreadState == ThreadState.Suspended || thread.ThreadState == ThreadState.SuspendRequested);

#pragma warning disable 612,618

            if (state == true)
                thread.Suspend();
            else if (state == false)
                thread.Resume();

#pragma warning restore 612,618

            // UNDONE: correct handling of pause on underlying thread

            if (parentThread)
                thread.Join();
        }

        /// <summary>
        /// Replaces the currently running instance of the program with a new one.
        /// </summary>
        public static void Reload()
        {
            Application.Restart();
        }

        /// <summary>
        /// Calls a function automatically at every specified interval.
        /// </summary>
        /// <param name="label">Name of the label to call.</param>
        /// <param name="mode">
        /// <list type="bullet">
        /// <item><term>On</term>: <description>enables a previously disabled timer or creates a new one at 250ms intervals.</description></item>
        /// <item><term>Off</term>: <description>disables an existing timer.</description></item>
        /// <item><term>Period</term>: <description>creates a new timer at the specified interval in milliseconds. If this value is negative the timer will only run once.</description></item>
        /// </list>
        /// </param>
        /// <param name="priority">A value between 0 and 4 inclusive to indicate the priority of the timer's thread.</param>
        public static void SetTimer(string label, string mode, int priority)
        {
            if (timers == null)
                timers = new Dictionary<string, Timer>();

            switch (mode.ToLowerInvariant())
            {
                case Keyword_On:
                    if (timers.ContainsKey(label))
                    {
                        timers[label].Start();
                        return;
                    }
                    else
                        mode = "250";
                    break;

                case Keyword_Off:
                    if (timers.ContainsKey(label))
                        timers[label].Stop();
                    else
                        ErrorLevel = 1;
                    return;
            }

            int interval = 250;

            if (!string.IsNullOrEmpty(mode) && !int.TryParse(mode, out interval))
            {
                ErrorLevel = 2;
                return;
            }

            var timer = new Timer();

            bool once = interval < 0;

            if (once)
                interval = -interval;

            if (timers.ContainsKey(label))
                timers[label].Interval = interval;
            else
                timers.Add(label, timer);

            if (once)
                timers.Remove(label);

            timer.Interval = interval;

            var level = ThreadPriority.Normal;

            if (priority > -1 && priority < 5)
                level = (ThreadPriority)priority;

            var method = FindLocalMethod(label);

            timer.Elapsed += delegate
                                 {
                                     System.Threading.Thread.CurrentThread.Priority = level;

                                     try { method.Invoke(null, new object[] { new object[] { } }); }
                                     catch (Exception)
                                     {
                                         timer.Stop();
                                         timers.Remove(label);
                                         timer.Dispose();
                                     }

                                     if (once)
                                     {
                                         timer.Stop();
                                         timer.Dispose();
                                     }
                                 };

            timer.Start();
        }

        /// <summary>
        /// Waits the specified amount of time before continuing.
        /// </summary>
        /// <param name="Delay">The amount of time to pause in milliseconds.</param>
        public static void Sleep(int Delay)
        {
            System.Threading.Thread.CurrentThread.Join(Delay);

            int stop = Environment.TickCount + Delay;

            while (Environment.TickCount < stop)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(10);
            }
        }

        /// <summary>
        /// Disables or enables all or selected hotkeys.
        /// </summary>
        /// <param name="mode">
        /// <list type="bullet">
        /// <item><term>On</term>: <description>suspends all hotkeys.</description></item>
        /// <item><term>Off</term>: <description>re-enables all hotkeys.</description></item>
        /// <item><term>Toggle</term> (default): <description>changes to the opposite of its previous state.</description></item>
        /// <item><term>Permit</term>: <description>marks the current subroutine as being exempt from suspension.</description></item>
        /// </list>
        /// </param>
        public static void Suspend(string mode)
        {
            var state = OnOff(mode);

            if (state == null && mode.Equals(Keyword_Toggle, StringComparison.OrdinalIgnoreCase))
                suspended = !suspended;
            else
                suspended = (bool)state;

            // UNDONE: permit mode for suspend
        }

        /// <summary>
        /// This method is obsolete, use <see cref="Critical"/>.
        /// </summary>
        [Obsolete]
        public static void Thread()
        {

        }
    }
}
