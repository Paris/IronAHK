using System;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Core
    {
        // TODO: organise Flow.cs

        /// <summary>
        /// Prevents the current thread from being interrupted by other threads.
        /// </summary>
        /// <param name="Mode">
        /// <list type="bullet">
        /// <item><term>On</term>: <description>give the current thread the highest priority.</description></item>
        /// <item><term>Off</term>: <description>resets the current thread priority to normal.</description></item>
        /// </list>
        /// </param>
        public static void Critical(string Mode)
        {
            bool on = OnOff(Mode) ?? true;
            System.Threading.Thread.CurrentThread.Priority = on ? ThreadPriority.Highest : ThreadPriority.Normal;
        }

        /// <summary>
        /// Exits the current thread.
        /// </summary>
        /// <param name="ExitCode">An integer that is returned to its caller when the program exits.</param>
        public static void Exit(int ExitCode)
        {
            Environment.ExitCode = ExitCode;
            Application.ExitThread();
        }

        /// <summary>
        /// Terminates the script unconditionally.
        /// </summary>
        /// <param name="ExitCode">An integer that is returned to its caller when the program exits.</param>
        public static void ExitApp(int ExitCode)
        {
            Environment.ExitCode = ExitCode;
            Application.Exit();
        }

        /// <summary>
        /// Checks if a function is defined.
        /// </summary>
        /// <param name="FunctionName">The name of a function.</param>
        /// <returns><c>true</c> if the specified function exists in the current scope, <c>false</c> otherwise.</returns>
        public static bool IsFunc(string FunctionName)
        {
            return FindLocalMethod(FunctionName) != null;
        }

        /// <summary>
        /// Checks if a label is defined.
        /// </summary>
        /// <param name="LabelName">The name of a label.</param>
        /// <returns><c>true</c> if the specified label exists in the current scope, <c>false</c> otherwise.</returns>
        public static bool IsLabel(string LabelName)
        {
            string method = LabelMethodName(LabelName);
            return FindLocalMethod(method) != null;
        }

        /// <summary>
        /// Specifies a label to run automatically when the application exits.
        /// </summary>
        /// <param name="Label">The name of a label. Leave blank to remove an existing label, if any.</param>
        public static void OnExit(string Label)
        {
            error = 0;

            if (onExit != null)
            {
                AppDomain.CurrentDomain.ProcessExit -= onExit;
                onExit = null;
            }

            if (string.IsNullOrEmpty(Label))
                return;

            string name = LabelMethodName(Label);
            var method = FindLocalMethod(name);

            if (method == null)
            {
                error = 1;
                return;
            }

            var handler = new EventHandler(delegate(object sender, EventArgs e)
            {
                method.Invoke(null, new object[] { new object[] { } });
            });

            AppDomain.CurrentDomain.ProcessExit += handler;
            onExit = handler;
        }

        /// <summary>
        /// Specifies a function to call automatically when the script receives the specified message.
        /// </summary>
        /// <param name="MsgNumber">The number of the message to monitor or query, which should be between 0 and 4294967295 (0xFFFFFFFF). If you do not wish to monitor a system message (that is, one below 0x400), it is best to choose a number greater than 4096 (0x1000) to the extent you have a choice. This reduces the chance of interfering with messages used internally by current and future versions of AutoHotkey.</param>
        /// <param name="FunctionName">A function's name, which must be enclosed in quotes if it is a literal string. This function will be called automatically when the script receives MsgNumber. Omit this parameter and the next one to retrieve the name of the function currently monitoring MsgNumber (blank if none). Specify an empty string ("") or an empty variable to turn off the monitoring of MsgNumber.</param>
        /// <param name="MaxThreads">This integer is normally omitted, in which case the monitor function is limited to one thread at a time. This is usually best because otherwise, the script would process messages out of chronological order whenever the monitor function interrupts itself. Therefore, as an alternative to MaxThreads, consider using Critical as described below.</param>
        public static void OnMessage(string MsgNumber, string FunctionName, string MaxThreads)
        {

        }

        /// <summary>
        /// Sends a string to the debugger (if any) for display.
        /// </summary>
        /// <param name="Text">The text to send to the debugger for display. This text may include linefeed characters (`n) to start new lines. In addition, a single long line can be broken up into several shorter ones by means of a continuation section.</param>
        public static void OutputDebug(string Text)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                Windows.OutputDebugString(Text);
        }

        /// <summary>
        /// Pauses the programs's current thread.
        /// </summary>
        /// <param name="Mode">
        /// <para>If blank or omitted, it defaults to <c>Toggle</c>. Otherwise, specify one of the following words:</para>
        /// <list type="bullet">
        /// <item><term>Toggle</term>: <description>pauses the current thread unless the thread beneath it is paused, in which case the underlying thread is unpaused.</description></item>
        /// <item><term>On</term>: <description>pauses the current thread.</description></item>
        /// <item><term>Off</term>: <description>if the thread beneath the current thread is paused, it will be in an unpaused state when resumed. Otherwise, the command has no effect.</description></item>
        /// </list>
        /// </param>
        /// <param name="OperateOnUnderlyingThread">
        /// <list type="bullet">
        /// <item><term>0</term>: <description>pause the current thread.</description></item>
        /// <item><term>1</term>: <description>marks the thread beneath the current thread as paused so that when it resumes, it will finish the command it was running (if any) and then enter a paused state. If there is no thread beneath the current thread, the program itself is paused, which prevents timers from running.</description></item>
        /// </list>
        /// </param>
        public static void Pause(string Mode, bool OperateOnUnderlyingThread)
        {
            throw new NotImplementedException();
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
        /// <param name="Label">Name of the label to call.</param>
        /// <param name="Mode">
        /// <list type="bullet">
        /// <item><term>On</term>: <description>enables a previously disabled timer or creates a new one at 250ms intervals.</description></item>
        /// <item><term>Off</term>: <description>disables an existing timer.</description></item>
        /// <item><term>Period</term>: <description>creates a new timer at the specified interval in milliseconds. If this value is negative the timer will only run once.</description></item>
        /// </list>
        /// </param>
        /// <param name="Priority">A value between 0 and 4 inclusive to indicate the priority of the timer's thread.</param>
        public static void SetTimer(string Label, string Mode, int Priority)
        {
            switch (Mode.ToLowerInvariant())
            {
                case Keyword_On:
                    if (timers.ContainsKey(Label))
                    {
                        timers[Label].Start();
                        return;
                    }
                    else
                        Mode = "250";
                    break;

                case Keyword_Off:
                    if (timers.ContainsKey(Label))
                        timers[Label].Stop();
                    else
                        error = 1;
                    return;
            }

            int interval = 250;

            if (!string.IsNullOrEmpty(Mode) && !int.TryParse(Mode, out interval))
            {
                error = 2;
                return;
            }

            var timer = new System.Timers.Timer();

            bool once = interval < 0;

            if (once)
                interval = -interval;

            if (timers.ContainsKey(Label))
                timers[Label].Interval = interval;
            else
                timers.Add(Label, timer);

            if (once)
                timers.Remove(Label);

            timer.Interval = interval;

            var priority = System.Threading.ThreadPriority.Normal;

            if (Priority > -1 && Priority < 5)
                priority = (System.Threading.ThreadPriority)Priority;

            var method = FindLocalMethod(Label);

            timer.Elapsed += new ElapsedEventHandler(delegate(object s, ElapsedEventArgs e)
            {
                System.Threading.Thread.CurrentThread.Priority = priority;

                try { method.Invoke(null, new object[] { new object[] { } }); }
                catch (Exception)
                {
                    timer.Stop();
                    timers.Remove(Label);
                    timer.Dispose();
                }

                if (once)
                {
                    timer.Stop();
                    timer.Dispose();
                }
            });

            timer.Start();
        }

        /// <summary>
        /// Waits the specified amount of time before continuing.
        /// </summary>
        /// <param name="Delay">The amount of time to pause in milliseconds.</param>
        public static void Sleep(int Delay)
        {
            const int step = 10;
            Delay /= step;

            for (int i = 0; i < Delay; i++)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(step);
            }
        }

        /// <summary>
        /// Disables or enables all or selected hotkeys.
        /// </summary>
        /// <param name="Mode">
        /// <list type="bullet">
        /// <item><term>On</term>: <description>suspends all hotkeys.</description></item>
        /// <item><term>Off</term>: <description>re-enables all hotkeys.</description></item>
        /// <item><term>Toggle</term> (default): <description>changes to the opposite of its previous state.</description></item>
        /// <item><term>Permit</term>: <description>marks the current subroutine as being exempt from suspension.</description></item>
        /// </list>
        /// </param>
        public static void Suspend(string Mode)
        {

        }

        /// <summary>
        /// Sets the priority or interruptibility of threads. It can also temporarily disable all timers.
        /// </summary>
        /// <param name="Setting"></param>
        /// <param name="P2"></param>
        /// <param name="P3"></param>
        public static void Thread(string Setting, string P2, string P3)
        {

        }
    }
}