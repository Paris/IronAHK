using System;
using System.Diagnostics;
using System.Timers;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Core
    {
        /// <summary>
        /// Exits (terminates) a loop. Valid inside any kind of loop.
        /// </summary>
        [Obsolete, Conditional("FLOW")]
        public static void Break() { }

        /// <summary>
        /// Skips the rest of the current loop iteration and begins a new one. Valid inside any kind of loop.
        /// </summary>
        [Obsolete, Conditional("FLOW")]
        public static void Continue() { }

        /// <summary>
        /// Specifies the command(s) to perform if an IF-statement evaluates to FALSE. When more than one command is present, enclose them in a block (braces). 
        /// </summary>
        [Obsolete, Conditional("FLOW")]
        public static void Else() { }

        /// <summary>
        /// Exits the current thread or (if the script is not persistent contains no hotkeys) the entire script.
        /// </summary>
        /// <param name="ExitCode">An integer (i.e. negative, positive, zero, or an expression) that is returned to its caller when the script exits. This code is accessible to any program that spawned the script, such as another script (via RunWait) or a batch (.bat) file. If omitted, ExitCode defaults to zero. Zero is traditionally used to indicate success. Note: Windows 95 may be limited in how large ExitCode can be.</param>
        public static void Exit(int ExitCode)
        {
            Environment.ExitCode = ExitCode;
            Application.ExitThread();
        }

        /// <summary>
        /// Terminates the script unconditionally.
        /// </summary>
        /// <param name="ExitCode">An integer (i.e. negative, positive, or zero) that is returned to its caller when the script exits. This code is accessible to any program that spawned the script, such as another script (via RunWait) or a batch (.bat) file. If omitted, ExitCode defaults to zero. Zero is traditionally used to indicate success. Note: Windows 95 may be limited in how large ExitCode can be.</param>
        public static void ExitApp(int ExitCode)
        {
            Environment.ExitCode = ExitCode;
            Application.Exit();
        }

        /// <summary>
        /// Jumps to the specified label and continues execution until Return is encountered.
        /// </summary>
        /// <param name="Label"></param>
        [Obsolete, Conditional("FLOW")]
        public static void Gosub(string Label)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Jumps to the specified label and continues execution.
        /// </summary>
        /// <param name="Label"></param>
        [Obsolete, Conditional("FLOW")]
        public static void Goto(string Label)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Specifies a subroutine to run automatically when the script exits.
        /// </summary>
        /// <param name="Label">If omitted, the script is returned to its normal exit behavior. Otherwise, specify the name of the label whose contents will be executed (as a new thread) when the script exits by any means.</param>
        public static void OnExit(string Label)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Pauses the script's current thread.
        /// </summary>
        /// <param name="Mode">
        /// <para>If blank or omitted, it defaults to Toggle. Otherwise, specify one of the following words:</para>
        /// <para>Toggle: Pauses the current thread unless the thread beneath it is paused, in which case the underlying thread is unpaused.</para>
        /// <para>On: Pauses the current thread.</para>
        /// <para>Off: If the thread beneath the current thread is paused, it will be in an unpaused state when resumed. Otherwise, the command has no effect.</para>
        /// </param>
        /// <param name="OperateOnUnderlyingThread">
        /// <para>This parameter is ignored for "Pause Off". For the others, it is ignored unless Pause is being turned on (including via Toggle).</para>
        /// <para>Specify one of the following numbers:</para>
        /// <para>0 (or omitted): The command pauses the current thread; that is, the one now running the Pause command.</para>
        /// <para>1: The command marks the thread beneath the current thread as paused so that when it resumes, it will finish the command it was running (if any) and then enter a paused state. If there is no thread beneath the current thread, the script itself is paused, which prevents timers from running (this effect is the same as having used the menu item "Pause Script" while the script has no threads).</para>
        /// </param>
        public static void Pause(string Mode, bool OperateOnUnderlyingThread)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Replaces the currently running instance of the script with a new one.
        /// </summary>
        public static void Reload()
        {
            Application.Restart();
        }

        /// <summary>
        /// Determines how fast a script will run (affects CPU utilization).
        /// </summary>
        /// <param name="LineCount">
        /// <para>(The 20ms is just an example.) If the value ends in ms, it indicates how often the script should sleep (each sleep is 10 ms long). In the following example, the script will sleep for 10ms every time it has run for 20ms: <example>SetBatchLines, 20ms</example></para>
        /// <para>The number of script lines to execute prior to sleeping for 10ms. The value can be as high as 9223372036854775807. Also, this mode is mutually exclusive of the 20ms mode in the previous paragraph; that is, only one of them can be in effect at a time.</para>
        /// </param>
        [Obsolete, Conditional("LEGACY")]
        public static void SetBatchLines(string LineCount)
        {

        }

        /// <summary>
        /// Calls a function automatically at every specified interval.
        /// </summary>
        /// <param name="Label">Name of the function to call.</param>
        /// <param name="Mode">
        /// <list type="bullet">
        /// <item><description>On: enables a previously disabled timer or creates a new one at 250ms intervals.</description></item>
        /// <item><description>Off: disables an existing timer.</description></item>
        /// <item><description>Period: creates a new timer at the specified interval in milliseconds.
        /// If this value is negative the timer will only run once.</description></item>
        /// </list>
        /// </param>
        /// <param name="Priority">A value between 0 and 4 inclusive to indicate the priority of the timer's thread.</param>
        public static void SetTimer(GenericFunction Label, string Mode, int Priority)
        {
            string name = Label.Method.Name;

            switch (Mode.ToLowerInvariant())
            {
                case Keyword_On:
                    if (timers.ContainsKey(name))
                    {
                        timers[name].Start();
                        return;
                    }
                    else
                        Mode = "250";
                    break;

                case Keyword_Off:
                    if (timers.ContainsKey(name))
                        timers[name].Stop();
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

            if (timers.ContainsKey(name))
                timers[name].Interval = interval;
            else
                timers.Add(name, timer);

            if (once)
                timers.Remove(name);

            timer.Interval = interval;

            var priority = System.Threading.ThreadPriority.Normal;

            if (Priority > -1 && Priority < 5)
                priority = (System.Threading.ThreadPriority)Priority;

            timer.Elapsed += new ElapsedEventHandler(delegate(object s, ElapsedEventArgs e)
            {
                System.Threading.Thread.CurrentThread.Priority = priority;
                Label(new object[] { });

                if (once)
                {
                    timer.Stop();
                    timer.Dispose();
                }
            });
        }

        /// <summary>
        /// Waits the specified amount of time before continuing.
        /// </summary>
        /// <param name="Delay">The amount of time to pause (in milliseconds) between 0 and 2147483647 (24 days).</param>
        public static void Sleep(int Delay)
        {
            for (int i = 0; i < (int)(Delay / 10); i++)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(10);
            }
        }

        /// <summary>
        /// Disables or enables all or selected hotkeys.
        /// </summary>
        /// <param name="Mode">
        /// <list type="">
        /// <item>On: Suspends all hotkeys except those explained the Remarks section.</item>
        /// <item>Off: Re-enables all hotkeys.</item>
        /// <item>Toggle (default): Changes to the opposite of its previous state (On or Off).</item>
        /// <item>Permit: Does nothing except mark the current subroutine as being exempt from suspension.</item>
        /// </list>
        /// </param>
        public static void Suspend(string Mode)
        {

        }
    }
}