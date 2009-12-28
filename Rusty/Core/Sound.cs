using System;
using System.Media;

namespace IronAHK.Rusty
{
    partial class Core
    {
        /// <summary>
        /// Emits a tone from the PC speaker.
        /// </summary>
        /// <param name="Frequency">The frequency of the sound. It should be a number between 37 and 32767. If omitted, the frequency will be 523.</param>
        /// <param name="Duration">The duration of the sound, in milliseconds . If omitted, the duration will be 150.</param>
        public static void SoundBeep(int Frequency, int Duration)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                Win32.Beep((uint)Frequency, (uint)Duration);
            else SystemSounds.Beep.Play();
        }

        /// <summary>
        /// Retrieves various settings from a sound device (master mute, master volume, etc.)
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the retrieved setting, which is either a floating point number between 0 and 100 (inclusive) or the word ON or OFF (used only for ControlTypes ONOFF, MUTE, MONO, LOUDNESS, STEREOENH, and BASSBOOST). The variable will be made blank if there was a problem retrieving the setting. The format of the floating point number, such as its decimal places, is determined by SetFormat.</param>
        /// <param name="ComponentType">
        /// <para>If omitted or blank, it defaults to the word MASTER. Otherwise, it can be one of the following words: MASTER (synonymous with SPEAKERS), DIGITAL, LINE, MICROPHONE, SYNTH, CD, TELEPHONE, PCSPEAKER, WAVE, AUX, ANALOG, HEADPHONES, or N/A. If the sound device lacks the specified ComponentType, ErrorLevel will indicate the problem.</para>
        /// <para>The component labled Auxiliary in some mixers might be accessible as ANALOG rather than AUX.</para>
        /// <para>If a device has more than one instance of ComponentType (two of type LINE, for example), usually the first contains the playback settings and the second contains the recording  To access an instance other than the first, append a colon and a number to this parameter. For example: Analog:2 is the second instance of the analog component.</para>
        /// </param>
        /// <param name="ControlType">If omitted or blank, it defaults to VOLUME. Otherwise, it can be one of the following words: VOLUME (or VOL), ONOFF, MUTE, MONO, LOUDNESS, STEREOENH, BASSBOOST, PAN, QSOUNDPAN, BASS, TREBLE, EQUALIZER, or the number of a valid control type (see soundcard analysis script). If the specified ComponentType lacks the specified ControlType, ErrorLevel will indicate the problem.</param>
        /// <param name="DeviceNumber">If this parameter is omitted, it defaults to 1 (the first sound device), which is usually the system's default device for recording and playback. Specify a number higher than 1 to operate upon a different sound device. This parameter can be an expression.</param>
        public static void SoundGet(out string OutputVar, string ComponentType, string ControlType, string DeviceNumber)
        {
            OutputVar = null;
        }

        /// <summary>
        /// Retrieves the wave output volume for a sound device.
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the retrieved volume level, which is a floating point number between 0 and 100 inclusive. The variable will be made blank if there was a problem retrieving the volume. The format of the floating point number, such as its decimal places, is determined by SetFormat.</param>
        /// <param name="DeviceNumber">If this parameter is omitted, it defaults to 1 (the first sound device), which is usually the system's default device for recording and playback. Specify a number higher than 1 to operate upon a different sound device.</param>
        public static void SoundGetWaveVolume(out int OutputVar, int DeviceNumber)
        {
            OutputVar = default(int);

            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                return;

            uint vol = 0;
            Win32.waveOutGetVolume(new IntPtr(DeviceNumber), out vol);
            OutputVar = (int)vol;
        }

        /// <summary>
        /// Plays a sound, video, or other supported file type.
        /// </summary>
        /// <param name="Filename">
        /// <para>The name of the file to be played, which is assumed to be in %A_WorkingDir% if an absolute path isn't specified.</para>
        /// <para>To produce standard system sounds, specify an asterisk followed by a number as shown below. Note: thewait parameter has no effect in this mode.</para>
        /// <list type="">
        /// <item>*-1: Simple beep. If the sound card is not available, the sound is generated using the speaker.</item>
        /// <item>*16: Hand (stop/error)</item>
        /// <item>*32: Question</item>
        /// <item>*48: Exclamation</item>
        /// <item>*64: Asterisk (info)</item>
        /// </list>
        /// </param>
        /// <param name="wait">
        /// <para>If omitted, the script's current thread will move on to the next commmand(s) while the file is playing. To avoid this, specify 1 or the word WAIT, which causes the current thread to wait until the file is finished playing before continuing. Even while waiting, new threads can be launched via hotkey, custom menu item, or timer.</para>
        /// <para>Known limitation: If the WAIT parameter is omitted, the OS might consider the playing file to be "in use" until the script closes or until another file is played (even a nonexistent file).</para>
        /// </param>
        public static void SoundPlay(string Filename, string wait)
        {

        }

        /// <summary>
        /// Changes various settings of a sound device (master mute, master volume, etc.)
        /// </summary>
        /// <param name="NewSetting">
        /// <para>Percentage number between -100 and 100 inclusive (it can be a floating point number or expression). If the number begins with a plus or minus sign, the current setting will be adjusted up or down by the indicated amount. Otherwise, the setting will be set explicitly to the level indicated by NewSetting.</para>
        /// <para>For ControlTypes with only two possible settings -- namely ONOFF, MUTE, MONO, LOUDNESS, STEREOENH, and BASSBOOST -- any positive number will turn on the setting and a zero will turn it off. However, if the number begins with a plus or minus sign, the setting will be toggled (set to the opposite of its current state).</para>
        /// </param>
        /// <param name="ComponentType">
        /// <para>If omitted or blank, it defaults to the word MASTER. Otherwise, it can be one of the following words: MASTER (synonymous with SPEAKERS), DIGITAL, LINE, MICROPHONE, SYNTH, CD, TELEPHONE, PCSPEAKER, WAVE, AUX, ANALOG, HEADPHONES, or N/A. If the sound device lacks the specified ComponentType, ErrorLevel will indicate the problem.</para>
        /// <para>The component labeled Auxiliary in some mixers might be accessible as ANALOG rather than AUX.</para>
        /// <para>If a device has more than one instance of ComponentType (two of type LINE, for example), usually the first contains the playback settings and the second contains the recording  To access an instance other than the first, append a colon and a number to this parameter. For example: Analog:2 is the second instance of the analog component.</para>
        /// </param>
        /// <param name="ControlType">If omitted or blank, it defaults to VOLUME. Otherwise, it can be one of the following words: VOLUME (or VOL), ONOFF, MUTE, MONO, LOUDNESS, STEREOENH, BASSBOOST, PAN, QSOUNDPAN, BASS, TREBLE, EQUALIZER, or the number of a valid control type (see soundcard analysis script). If the specified ComponentType lacks the specified ControlType, ErrorLevel will indicate the problem.</param>
        /// <param name="DeviceNumber">If this parameter is omitted, it defaults to 1 (the first sound device), which is usually the system's default device for recording and playback. Specify a number higher than 1 to operate upon a different sound device. This parameter can be an expression.</param>
        public static void SoundSet(string NewSetting, string ComponentType, string ControlType, string DeviceNumber)
        {

        }

        /// <summary>
        /// Changes the wave output volume for a sound device.
        /// </summary>
        /// <param name="Percent">Percentage number between -100 and 100 inclusive (it can be a floating point number or an expression). If the number begins with a plus or minus sign, the current volume level will be adjusted up or down by the indicated amount. Otherwise, the volume will be set explicitly to the level indicated by Percent.</param>
        /// <param name="DeviceNumber">If this parameter is omitted, it defaults to 1 (the first sound device), which is usually the system's default device for recording and playback. Specify a number higher than 1 to operate upon a different sound device.</param>
        public static void SoundSetWaveVolume(string Percent, int DeviceNumber)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                return;

            IntPtr dev = new IntPtr(DeviceNumber);
            uint vol;

            char p = Percent[0];
            if (p == '+' || p == '-')
            {
                Win32.waveOutGetVolume(dev, out vol);
                vol = (uint)(vol * double.Parse(Percent.Substring(1)) / 100);
            }
            else vol = (uint)(0xfffff * (double.Parse(Percent) / 100));

            Win32.waveOutSetVolume(dev, vol);
        }
    }
}