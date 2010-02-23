using System;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Core
    {
        /// <summary>
        /// Waits until the clipboard contains data.
        /// </summary>
        /// <param name="SecondsToWait">If omitted, the command will wait indefinitely. Otherwise, it will wait no longer than this many seconds</param>
        /// <param name="AnyType"><c>false</c> to wait specifically for text or files to appear, otherwise wait for data of any kind.</param>
        public static void ClipWait(double SecondsToWait, bool AnyType)
        {
            int frequency = 100, time = (int)(SecondsToWait * 1000);

            for (int i = 0; i < time; i += frequency)
            {
                if ((!AnyType && Clipboard.ContainsText()) || Clipboard.ContainsData(DataFormats.WaveAudio))
                {
                    error = 0;
                    return;
                }
                System.Threading.Thread.Sleep(frequency);
            }

            error = 1;
        }

        /// <summary>
        /// Retrieves an environment variable.
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the string.</param>
        /// <param name="EnvVarName">The name of the environment variable to retrieve.</param>
        public static void EnvGet(out string OutputVar, string EnvVarName)
        {
            OutputVar = Environment.GetEnvironmentVariable(EnvVarName);
        }

        /// <summary>
        /// Writes a value to a variable contained in the environment.
        /// </summary>
        /// <param name="EnvVar">Name of the environment variable to use, e.g. <c>PATH</c>.</param>
        /// <param name="Value">Value to set the environment variable to.</param>
        public static void EnvSet(string EnvVar, string Value)
        {
            Environment.SetEnvironmentVariable(EnvVar, Value);
        }

        /// <summary>
        /// Notifies the operating system and all running applications that environment variable have changed.
        /// </summary>
        public static void EnvUpdate()
        {
            error = 0;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                try { Windows.SendMessage(new IntPtr(Windows.HWND_BROADCAST), Windows.WM_SETTINGCHANGE, IntPtr.Zero, IntPtr.Zero); }
                catch (Exception) { error = 1; }
            }
        }

        /// <summary>
        /// Retrieves screen resolution, multi-monitor info, dimensions of system objects, and other system properties.
        /// </summary>
        /// <param name="OutputVar">The variable in which to store the result.</param>
        /// <param name="Command"></param>
        /// <param name="Param"></param>
        public static void SysGet(out string OutputVar, string Command, string Param)
        {
            OutputVar = null;
        }
    }
}