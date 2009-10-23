using System;
using System.Runtime.InteropServices;

namespace IronAHK.Rusty.Windows
{
    class Sound
    {
        [DllImport("winmm.dll")]
        public static extern uint waveOutGetVolume(IntPtr hwo, out uint dwVolume);

        [DllImport("winmm.dll")]
        public static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);

        [DllImport("kernel32.dll")]
        public static extern bool Beep(uint dwFreq, uint dwDuration);
    }
}
