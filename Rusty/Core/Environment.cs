using System;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Core
    {
        /// <summary>
        /// Waits until the clipboard contains data.
        /// </summary>
        /// <param name="SecondsToWait">If omitted, the command will wait indefinitely. Otherwise, it will wait no longer than this many seconds (can contain a decimal point or be an expression). Specifying 0 is the same as specifying 0.5.</param>
        /// <param name="AnyType">If this parameter is omitted, the command is more selective, waiting specifically for text or files to appear ("text" includes anything that would produce text when you paste into Notepad). If this parameter is 1 (can be an expression), the command waits for data of any kind to appear on the clipboard.</param>
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
        /// Write a line of text to the console (stdout).
        /// </summary>
        /// <param name="Text">The text to write. A linefeed is automatically included at the end of the text.</param>
        public static void ConsoleAppend(string Text)
        {
            Console.WriteLine(Text);
        }

        /// <summary>
        /// Read a line of user input from the console (stdin).
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the retrieved line of text.</param>
        public static void ConsoleRead(out string OutputVar)
        {
            OutputVar = Console.ReadLine();
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
        /// <param name="EnvVar">Name of the environment variable to use, e.g. "COMSPEC" or "PATH".</param>
        /// <param name="Value">Value to set the environment variable to.</param>
        public static void EnvSet(ref string EnvVar, string Value)
        {
            Environment.SetEnvironmentVariable(EnvVar, Value);
        }

        /// <summary>
        /// Notifies the OS and all running applications that environment variable(s) have changed.
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
    }
}