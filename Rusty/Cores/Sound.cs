using System;
using System.Media;

namespace IronAHK.Rusty
{
    partial class Core
    {
        /// <summary>
        /// Emits a tone from the PC speaker.
        /// </summary>
        /// <param name="frequency">The frequency of the sound which should be between 37 and 32767.
        /// If omitted, the frequency will be 523.</param>
        /// <param name="duration">The duration of the sound in ms. If omitted, the duration will be 150.</param>
        public static void SoundBeep(int frequency, int duration)
        {
            if (frequency == 0)
                frequency = 523;

            if (duration == 0)
                duration = 150;

            Console.Beep(frequency, duration);
        }

        /// <summary>
        /// Retrieves various settings from a sound device.
        /// </summary>
        /// <param name="output"></param>
        /// <param name="component"></param>
        /// <param name="control"></param>
        /// <param name="device"></param>
        public static void SoundGet(out string output, string component, string control, string device)
        {
            // TODO: SoundGet

            output = null;
        }

        /// <summary>
        /// Retrieves the wave output volume for a sound device.
        /// </summary>
        /// <param name="output">The variable to store the result.</param>
        /// <param name="device">If this parameter is omitted, it defaults to 1 (the first sound device),
        /// which is usually the system's default device for recording and playback.
        /// Specify a higher value to operate upon a different sound device.</param>
        public static void SoundGetWaveVolume(out int output, int device)
        {
            output = 0;

            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                return;

            uint vol = 0;
            WindowsAPI.waveOutGetVolume(new IntPtr(device), out vol);
            output = (int)vol;

            // UNDONE: cross platform SoundGetWaveVolume
        }

        /// <summary>
        /// Plays a sound, video, or other supported file type.
        /// </summary>
        /// <param name="filename">
        /// <para>The name of the file to be played.</para>
        /// <para>To produce standard system sounds, specify an asterisk followed by a number as shown below.</para>
        /// <list type="bullet">
        /// <item><term>*-1</term>: <description>simple beep</description></item>
        /// <item><term>*16</term>: <description>hand (stop/error)</description></item>
        /// <item><term>*32</term>: <description>question</description></item>
        /// <item><term>*48</term>: <description>exclamation</description></item>
        /// <item><term>*64</term>: <description>asterisk (info)</description></item>
        /// </list>
        /// </param>
        /// <param name="wait"><c>true</c> to block the current thread until the sound has finished playing, false otherwise.</param>
        /// <remarks><see cref="ErrorLevel"/> is set to <c>1</c> if an error occured, <c>0</c> otherwise.</remarks>
        public static void SoundPlay(string filename, bool wait)
        {
            ErrorLevel = 0;

            if (filename.Length > 1 && filename[0] == '*')
            {
                int n;

                if (!int.TryParse(filename.Substring(1), out n))
                {
                    ErrorLevel = 1;
                    return;
                }

                switch (n)
                {
                    case -1: SystemSounds.Beep.Play(); break;
                    case 16: SystemSounds.Hand.Play(); break;
                    case 32: SystemSounds.Question.Play(); break;
                    case 48: SystemSounds.Exclamation.Play(); break;
                    case 64: SystemSounds.Asterisk.Play(); break;
                    default: ErrorLevel = 1; break;
                }

                return;
            }

            try
            {
                var sound = new SoundPlayer(filename);

                if (wait)
                    sound.PlaySync();
                else
                    sound.Play();
            }
            catch (Exception)
            {
                ErrorLevel = 1;
            }
        }

        /// <summary>
        /// Changes various settings of a sound device.
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="component"></param>
        /// <param name="control"></param>
        /// <param name="device"></param>
        public static void SoundSet(string setting, string component, string control, string device)
        {
            // TODO: SoundSet
        }

        /// <summary>
        /// Changes the wave output volume for a sound device.
        /// </summary>
        /// <param name="percent">Percentage number between -100 and 100 inclusive.
        /// If the number begins with a plus or minus sign, the current volume level will be adjusted up or down by the indicated amount.</param>
        /// <param name="device">If this parameter is omitted, it defaults to 1 (the first sound device),
        /// which is usually the system's default device for recording and playback.</param>
        public static void SoundSetWaveVolume(string percent, int device)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                return;

            if (string.IsNullOrEmpty(percent))
                percent = "0";

            var dev = new IntPtr(device);
            uint vol;

            char p = percent[0];
            if (p == '+' || p == '-')
            {
                WindowsAPI.waveOutGetVolume(dev, out vol);
                vol = (uint)(vol * double.Parse(percent.Substring(1)) / 100);
            }
            else
                vol = (uint)(0xfffff * (double.Parse(percent) / 100));

            WindowsAPI.waveOutSetVolume(dev, vol);

            // TODO: cross platform SoundSetWaveVolume
        }
    }
}
