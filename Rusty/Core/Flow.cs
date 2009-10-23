using System;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Core
    {
#if FLOW
        [Obsolete()]
        public static void Break() { }
#endif

#if FLOW
        [Obsolete()]
        public static void Continue() { }
#endif

#if FLOW
        [Obsolete()]
        public static void Else() { }
#endif

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

#if FLOW
        [Obsolete()]
        public static void Gosub(string Label) { }
#endif

#if FLOW
        [Obsolete()]
        public static void Goto(string Label) { }
#endif

#if DEPRECATED
        /// <summary>
        /// Specifies a subroutine to run automatically when the script exits.
        /// </summary>
        /// <param name="Label">If omitted, the script is returned to its normal exit behavior. Otherwise, specify the name of the label whose contents will be executed (as a new thread) when the script exits by any means.</param>
        [Obsolete()]
        public static void OnExit(string Label) { }
#endif

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

        }

        /// <summary>
        /// Determines how fast a script will run (affects CPU utilization).
        /// </summary>
        /// <param name="LineCount">
        /// <para>(The 20ms is just an example.) If the value ends in ms, it indicates how often the script should sleep (each sleep is 10 ms long). In the following example, the script will sleep for 10ms every time it has run for 20ms: <example>SetBatchLines, 20ms</example></para>
        /// <para>The number of script lines to execute prior to sleeping for 10ms. The value can be as high as 9223372036854775807. Also, this mode is mutually exclusive of the 20ms mode in the previous paragraph; that is, only one of them can be in effect at a time.</para>
        /// </param>
        public static void SetBatchLines(string LineCount)
        {
            int pos = LineCount.ToLower().IndexOf("ms");
            bool line = pos == -1;
            int n = int.Parse(line ? LineCount : LineCount.Substring(0, pos));
            Settings.BatchLines = line ? n : -n;
        }

        /// <summary>
        /// Causes a subroutine to be launched automatically and repeatedly at a specified time interval.
        /// </summary>
        /// <param name="Label">The name of the label or hotkey label to which to jump, which causes the commands beneath Label to be executed until a Return or Exit is encountered. As with the parameters of almost all other commands, Label can be a variable reference such as %MyLabel%, in which case the name stored in the variable is used as the target.</param>
        /// <param name="Mode">
        /// <para>On: Re-enables a previously disabled timer at its former period. If the timer doesn't exist, it is created (with a default period of 250). If the timer exists but was previously set to run-only-once mode, it will again run only once.</para>
        /// <para>Off: Disables an existing timer.</para>
        /// <para>Period: Creates or updates a timer using this parameter as the number of milliseconds that must pass since the last time the Label subroutine was started. When this amount of time has passed, Label will be run again (unless it is still running from the last time). The timer will be automatically enabled. To prevent this, call the command a second time immediately afterward, specifying OFF for this parameter.</para>
        /// <para>If this parameter is blank and:</para>
        /// <list type="">
        /// <item>1) the timer does not exist: it will be created with a period of 250.</item>
        /// <item>2) the timer already exists: it will be enabled and reset at its former period unless a Priority is specified.</item>
        /// </list>
        /// <para>Run only once: Specify a negative Period to indicate that the timer should run only once. For example, specifying -100 would run the timer 100 ms from now then disable the timer as though SetTimer, Label, Off had been used.</para>
        /// </param>
        /// <param name="Priority">
        /// <para>This optional parameter is an integer between -2147483648 and 2147483647 (or an expression) to indicate this timer's thread priority. If omitted, 0 will be used. See Threads for details.</para>
        /// <para>To change the priority of an existing timer without affecting it in any other way, leave the parameter before this one blank.</para>
        /// </param>
        public static void SetTimer(PseudoLabel Label, string Mode, string Priority)
        {

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