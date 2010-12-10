using System;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Core
    {
        /// <summary>
        /// Waits until the clipboard contains data.
        /// </summary>
        /// <param name="timeout">If omitted, the command will wait indefinitely.
        /// Otherwise, it will wait no longer than this many seconds</param>
        /// <param name="type"><c>false</c> to wait specifically for text or files to appear, otherwise wait for data of any kind.</param>
        public static void ClipWait(double timeout, bool type)
        {
            int frequency = 100, time = (int)(timeout * 1000);

            for (int i = 0; i < time; i += frequency)
            {
                if ((!type && Clipboard.ContainsText()) || Clipboard.ContainsData(DataFormats.WaveAudio))
                {
                    ErrorLevel = 0;
                    return;
                }
                System.Threading.Thread.Sleep(frequency);
            }

            ErrorLevel = 1;
        }

        /// <summary>
        /// Retrieves an environment variable.
        /// </summary>
        /// <param name="output">The variable to store the result.</param>
        /// <param name="name">The name of the environment variable to retrieve.</param>
        public static void EnvGet(out string output, string name)
        {
            output = Environment.GetEnvironmentVariable(name);
        }

        /// <summary>
        /// Writes a value to a variable contained in the environment.
        /// </summary>
        /// <param name="name">Name of the environment variable to use, e.g. <c>PATH</c>.</param>
        /// <param name="value">Value to set the environment variable to.</param>
        public static void EnvSet(string name, string value)
        {
            Environment.SetEnvironmentVariable(name, value);
        }

        /// <summary>
        /// Notifies the operating system and all running applications that environment variable have changed.
        /// </summary>
        public static void EnvUpdate()
        {
            ErrorLevel = 0;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                try { WindowsAPI.SendMessage(new IntPtr(WindowsAPI.HWND_BROADCAST), WindowsAPI.WM_SETTINGCHANGE, IntPtr.Zero, IntPtr.Zero); }
                catch (Exception) { ErrorLevel = 1; }
            }
        }

        /// <summary>
        /// Retrieves screen resolution, multi-monitor info, dimensions of system objects, and other system properties.
        /// </summary>
        /// <param name="output">The variable to store the result.</param>
        /// <param name="command"></param>
        /// <param name="param"></param>
        public static void SysGet(out string output, string command, string param)
        {
            // TODO: sysget command

            output = null;
        }
    }
}
