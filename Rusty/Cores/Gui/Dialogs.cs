using System;
using System.Drawing;
using System.Reflection;
using System.IO;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Core
    {
        delegate void AsyncCallDlgOptions(ComplexDlgOptions options);
        delegate void AsyncComplexDialoge(IComplexDialoge complexDlg);

        // TODO: organise Dialogs.cs

        #region FileSelectFile

        /// <summary>
        /// Displays a standard dialog that allows the user to open or save files.
        /// </summary>
        /// <param name="OutputVar">The user selected files.</param>
        /// <param name="Options">
        /// <list type="bullet">
        /// <item><term>M</term>: <description>allow muliple files to be selected.</description></item>
        /// <item><term>S</term>: <description>show a save as dialog rather than a file open dialog.</description></item>
        /// <item><term>1</term>: <description>only allow existing file or directory paths.</description></item>
        /// <item><term>8</term>: <description>prompt to create files.</description></item>
        /// <item><term>16:</term>: <description>prompt to overwrite files.</description></item>
        /// <item><term>32</term>: <description>follow the target of a shortcut rather than using the shortcut file itself.</description></item>
        /// </list>
        /// </param>
        /// <param name="RootDir">The file path to initially select.</param>
        /// <param name="Prompt">Text displayed in the window to instruct the user what to do.</param>
        /// <param name="Filter">Indicates which types of files are shown by the dialog, e.g. <c>Audio (*.wav; *.mp2; *.mp3)</c>.</param>
        public static void FileSelectFile(out string OutputVar, string Options, string RootDir, string Prompt, string Filter)
        {
            bool save = false, multi = false, check = false, create = false, overwite = false, shortcuts = false;

            Options = Options.ToUpperInvariant();

            if (Options.Contains("M"))
            {
                Options = Options.Replace("M", string.Empty);
                multi = true;
            }

            if (Options.Contains("S"))
            {
                Options = Options.Replace("S", string.Empty);
                save = true;
            }

            int result;

            if (int.TryParse(Options.Trim(), out result))
            {
                if ((result & 1) == 1 || (result & 2) == 2)
                    check = true;

                if ((result & 8) == 8)
                    create = true;

                if ((result & 16) == 16)
                    overwite = true;

                if ((result & 32) == 32)
                    shortcuts = true;
            }

            ErrorLevel = 0;
            OutputVar = null;

            if (save)
            {
                var saveas = new SaveFileDialog { CheckPathExists = check, CreatePrompt = create, OverwritePrompt = overwite, DereferenceLinks = shortcuts, Filter = Filter };
                var selected = dialogOwner == null ? saveas.ShowDialog() : saveas.ShowDialog(dialogOwner);

                if (selected == DialogResult.OK)
                    OutputVar = saveas.FileName;
                else
                    ErrorLevel = 1;
            }
            else
            {
                var open = new OpenFileDialog { Multiselect = multi, CheckFileExists = check, DereferenceLinks = shortcuts, Filter = Filter };
                var selected = dialogOwner == null ? open.ShowDialog() : open.ShowDialog(dialogOwner);

                if (selected == DialogResult.OK)
                    OutputVar = multi ? string.Join("\n", open.FileNames) : open.FileName;
                else
                    ErrorLevel = 1;
            }
        }

        #endregion

        #region FileSelectFolder

        /// <summary>
        /// Displays a standard dialog that allows the user to select a folder.
        /// </summary>
        /// <param name="OutputVar">The user selected folder.</param>
        /// <param name="StartingFolder">An asterisk followed by a path to initially select a folder, can be left blank for none.</param>
        /// <param name="Options">
        /// <list type="bullet">
        /// <item><term>1</term>: <description>show a new folder button in the dialog.</description></item>
        /// </list>
        /// </param>
        /// <param name="Prompt">Text displayed in the window to instruct the user what to do.</param>
        public static void FileSelectFolder(out string OutputVar, string StartingFolder, int Options, string Prompt)
        {
            var select = new FolderBrowserDialog();

            select.ShowNewFolderButton = (Options & 1) == 1;

            if (!string.IsNullOrEmpty(Prompt))
                select.Description = Prompt;

            StartingFolder = StartingFolder.Trim();

            if (StartingFolder.Length > 2 && StartingFolder[0] == '*')
                select.SelectedPath = StartingFolder.Substring(1);
            else if (StartingFolder.Length != 0)
            {
                // TODO: convert CLSID to special folder enumeration for folder select dialog
            }

            ErrorLevel = 0;

            var selected = dialogOwner == null ? select.ShowDialog() : select.ShowDialog(dialogOwner);

            if (selected == DialogResult.OK)
                OutputVar = select.SelectedPath;
            else
            {
                OutputVar = string.Empty;
                ErrorLevel = 1;
            }
        }

        #endregion

        #region InputBox

        /// <summary>
        /// Displays an input box to ask the user to enter a string.
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the text entered by the user.</param>
        /// <param name="Title">The title of the input box. If blank or omitted, it defaults to the name of the script.</param>
        /// <param name="Prompt">The text of the input box, which is usually a message to the user indicating what kind of input is expected.</param>
        /// <param name="Hide">If this parameter is the word HIDE, the user's input will be masked, which is useful for passwords.</param>
        /// <param name="Width">If this parameter is blank or omitted, the starting width of the window will be 375.</param>
        /// <param name="Height">If this parameter is blank or omitted, the starting height of the window will be 189.</param>
        /// <param name="X">The X coordinate of the window (use 0,0 to move it to the upper left corner of the desktop). If either coordinate is blank or omitted, the dialog will be centered in that dimension. Either coordinate can be negative to position the window partially or entirely off the desktop.</param>
        /// <param name="Y">The Y coordinate of the window (see <paramref name="X"/>).</param>
        /// <param name="Font">Not yet implemented (leave blank). In the future it might accept something like verdana:8</param>
        /// <param name="Timeout">Timeout in seconds (can contain a decimal point).  If this value exceeds 2147483 (24.8 days), it will be set to 2147483. After the timeout has elapsed, the InputBox window will be automatically closed and ErrorLevel will be set to 2. OutputVar will still be set to what the user entered.</param>
        /// <param name="Default">A string that will appear in the InputBox's edit field when the dialog first appears. The user can change it by backspacing or other means.</param>
        public static DialogResult InputBox(out string OutputVar, string Title, string Prompt, string Hide, string Width, string Height, string X, string Y, string Font, string Timeout, string Default)
        {
            var input = new InputDialog 
                { 
                    Title = Title, 
                    Prompt = Prompt 
                };

            if (dialogOwner != null)
                input.Owner = dialogOwner;

            int n;

            if (!string.IsNullOrEmpty(Width) && int.TryParse(Width, out n))
                input.Size = new Size(n, input.Size.Height);

            if (!string.IsNullOrEmpty(Height) && int.TryParse(Height, out n))
                input.Size = new Size(input.Size.Width, n);

            if (!string.IsNullOrEmpty(X) && int.TryParse(X, out n))
                input.Location = new Point(n, input.Location.X);

            if (!string.IsNullOrEmpty(Y) && int.TryParse(Y, out n))
                input.Location = new Point(input.Location.Y, n);

            input.Hide = Hide.ToLowerInvariant().Contains(Keyword_Hide);


            var result = input.ShowDialog();

            if (result == DialogResult.OK)
                OutputVar = input.Message;
            else
                OutputVar = null;

            switch (result)
            {
                case DialogResult.OK:
                    ErrorLevel = 0;
                    break;

                default:
                    ErrorLevel = 1;
                    break;
            }

            return result;
        }

        #endregion

        #region MsgBox

        /// <summary>
        /// Show a message box.
        /// </summary>
        /// <param name="Text">The text to show in the prompt.</param>
        public static void MsgBox(string Text)
        {
            var title = Environment.GetEnvironmentVariable("SCRIPT") ?? string.Empty;

            if (!string.IsNullOrEmpty(title) && File.Exists(title))
                title = Path.GetFileName(title);

            if (dialogOwner != null)
                MessageBox.Show(dialogOwner, Text, title);
            else
                MessageBox.Show(Text, title);
        }

        

        /// <summary>
        /// Displays the specified text in a small window containing one or more buttons (such as Yes and No).
        /// </summary>
        /// <param name="Options">
        /// <para>Indicates the type of message box and the possible button combinations. If blank or omitted, it defaults to 0. See the table below for allowed values.</para>
        /// <para>This parameter will not be recognized if it contains an expression or a variable reference such as %option%. Instead, use a literal numeric value.</para>
        /// </param>
        /// <param name="Title">The title of the message box window. If omitted or blank, it defaults to the name of the script (without path).</param>
        /// <param name="Text">
        /// <para>If all the parameters are omitted, the MsgBox will display the text "Press OK to continue." Otherwise, this parameter is the text displayed inside the message box to instruct the user what to do, or to present information.</para>
        /// <para>Escape sequences can be used to denote special characters. For example, `n indicates a linefeed character, which ends the current line and begins a new one. Thus, using text1`n`ntext2 would create a blank line between text1 and text2.</para>
        /// <para>If Text is long, it can be broken up into several shorter lines by means of a continuation section, which might improve readability and maintainability.</para>
        /// </param>
        /// <param name="Timeout">
        /// <para>(optional) Timeout in seconds (can contain a decimal point but cannot be an expression).  If this value exceeds 2147483 (24.8 days), it will be set to 2147483.  After the timeout has elapsed the message box will be automatically closed and the IfMsgBox command will see the value TIMEOUT.</para>
        /// <para>Known limitation: If the MsgBox contains only an OK button, IfMsgBox will think that the OK button was pressed if the MsgBox times out while its own thread is interrupted by another.</para>
        /// </param>
        public static DialogResult MsgBox(int Options, string Title, string Text, int Timeout)
        {
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            switch (Options & 0xf)
            {
                case 0: buttons = MessageBoxButtons.OK; break;
                case 1: buttons = MessageBoxButtons.OKCancel; break;
                case 2: buttons = MessageBoxButtons.AbortRetryIgnore; break;
                case 3: buttons = MessageBoxButtons.YesNoCancel; break;
                case 4: buttons = MessageBoxButtons.YesNo; break;
                case 5: buttons = MessageBoxButtons.RetryCancel; break;
                //case 6: /* Cancel/Try Again/Continue */ ; break;
                //case 7: /* Adds a Help button */ ; break; // help done differently
            }

            MessageBoxIcon icon = MessageBoxIcon.None;
            switch (Options & 0xf0)
            {
                case 16: icon = MessageBoxIcon.Hand; break;
                case 32: icon = MessageBoxIcon.Question; break;
                case 48: icon = MessageBoxIcon.Exclamation; break;
                case 64: icon = MessageBoxIcon.Asterisk; break;
            }

            MessageBoxDefaultButton defaultbutton = MessageBoxDefaultButton.Button1;
            switch (Options & 0xf00)
            {
                case 256: defaultbutton = MessageBoxDefaultButton.Button2; break;
                case 512: defaultbutton = MessageBoxDefaultButton.Button3; break;
            }

            var options = default(MessageBoxOptions);
            switch (Options & 0xf0000)
            {
                case 131072: options = MessageBoxOptions.DefaultDesktopOnly; break;
                case 262144: options = MessageBoxOptions.ServiceNotification; break;
                case 524288: options = MessageBoxOptions.RightAlign; break;
                case 1048576: options = MessageBoxOptions.RtlReading; break;
            }

            bool help = (Options & 0xf000) == 16384;

            if (string.IsNullOrEmpty(Title))
            {
                var script = Environment.GetEnvironmentVariable("SCRIPT");

                if (!string.IsNullOrEmpty(script) && File.Exists(script))
                    Title = Path.GetFileName(script);
            }

            var result = DialogResult.None;

            if (dialogOwner != null)
                result = MessageBox.Show(dialogOwner, Text, Title, buttons, icon, defaultbutton, options);
            else
                result = MessageBox.Show(Text, Title, buttons, icon, defaultbutton, options, help);

            return result;
        }

        #endregion

        #region Progress

        /// <summary>
        /// Creates or updates a window containing a progress bar or an image.
        /// </summary>
        /// <param name="ProgressParam1">
        /// <para>If the progress window already exists: If Param1 is the word OFF, the window is destroyed. If Param1 is the word SHOW, the window is shown if it is currently hidden.</para>
        /// <para>Otherwise, if Param1 is an pure number, its bar's position is changed to that value. If Param1 is blank, its bar position will be unchanged but its text will be updated to reflect any new strings provided in SubText, MainText, and WinTitle. In both of these modes, if the window doesn't yet exist, it will be created with the defaults for all options.</para>
        /// <para>If the progress window does not exist: A new progress window is created (replacing any old one), and Param1 is a string of zero or more options from the list below.</para>
        /// </param>
        /// <param name="SubText">The text to display below the image or bar indicator. Although word-wrapping will occur, to begin a new line explicitly, use linefeed (`n). To set an existing window's text to be blank, specify %A_Space%. For the purpose of auto-calculating the window's height, blank lines can be reserved in a way similar to MainText below.</param>
        /// <param name="MainText">
        /// <para>The text to display above the image or bar indicator (its font is semi-bold). Although word-wrapping will occur, to begin a new line explicitly, use linefeed (`n).</para>
        /// <para>If blank or omitted, no space will be reserved in the window for MainText. To reserve space for single line to be added later, or to set an existing window's text to be blank, specify %A_Space%. To reserve extra lines beyond the first, append one or more linefeeds (`n).</para>
        /// <para>Once the height of MainText's control area has been set, it cannot be changed without recreating the window.</para>
        /// </param>
        /// <param name="WinTitle">The text to display below the image or bar indicator. Although word-wrapping will occur, to begin a new line explicitly, use linefeed (`n). To set an existing window's text to be blank, specify %A_Space%. For the purpose of auto-calculating the window's height, blank lines can be reserved in a way similar to MainText below.</param>
        /// <param name="FontName">
        /// <para>The name of the font to use for both MainText and SubText. The font table lists the fonts included with the various versions of Windows. If unspecified or if the font cannot be found, the system's default GUI font will be used.</para>
        /// <para>See the options section below for how to change the size, weight, and color of the font.</para>
        /// </param>
        public static void Progress(string ProgressParam1, string SubText, string MainText, string WinTitle, string FontName) {

            InitDialoges();

            var progressOptions = new ComplexDlgOptions()
            {
                SubText = SubText,
                MainText = MainText,
                WinTitle = WinTitle,
            };
            progressOptions.ParseGuiID(ProgressParam1);
            progressOptions.ParseComplexOptions(ProgressParam1);

            ProgressAssync(progressOptions);
        }

        private static void ProgressAssync(ComplexDlgOptions Options) {
            ProgressDialog thisProgress = null;

            if(progressDialgos.ContainsKey(Options.GUIID)) {
                thisProgress = progressDialgos[Options.GUIID];
                if(thisProgress.InvokeRequired) {
                    thisProgress.Invoke(new AsyncCallDlgOptions(ProgressAssync), Options);
                }
            }

            if(thisProgress != null) {
                Options.AppendShowHideTo(thisProgress);
            } else {
                thisProgress = new ProgressDialog();
                progressDialgos.Add(Options.GUIID, thisProgress);
            }

            Options.AppendTo(thisProgress);

            #region Parse Progress specific Options

            short num;
            if(!short.TryParse(Options.Param1, out num)) {
                num = 0;
            }
            thisProgress.ProgressValue = num;

            #endregion

            if(!Options.Hide && !thisProgress.Visible)
                thisProgress.Show();
        }

        #endregion

        #region SplashImage

        /// <summary>
        /// Creates or updates a window containing a progress bar or an image.
        /// </summary>
        /// <param name="ImageFile">
        /// <para>If this is the word OFF, the window is destroyed. If this is the word SHOW, the window is shown if it is currently hidden.</para>
        /// <para>Otherwise, this is the file name of the BMP, GIF, or JPG image to display (to display other file formats such as PNG, TIF, and ICO, consider using the Gui command to create a window containing a picture control).</para>
        /// <para>ImageFile is assumed to be in %A_WorkingDir% if an absolute path isn't specified. If ImageFile and Options are blank and the window already exists, its image will be unchanged but its text will be updated to reflect any new strings provided in SubText, MainText, and WinTitle.</para>
        /// <para>For newly created windows, if ImageFile is blank or there is a problem loading the image, the window will be displayed without the picture.</para>
        /// </param>
        /// <param name="Options">A string of zero or more options from the list further below.</param>
        /// <param name="SubText">The text to display below the image or bar indicator. Although word-wrapping will occur, to begin a new line explicitly, use linefeed (`n). To set an existing window's text to be blank, specify %A_Space%. For the purpose of auto-calculating the window's height, blank lines can be reserved in a way similar to MainText below.</param>
        /// <param name="MainText">
        /// <para>The text to display above the image or bar indicator (its font is semi-bold). Although word-wrapping will occur, to begin a new line explicitly, use linefeed (`n).</para>
        /// <para>If blank or omitted, no space will be reserved in the window for MainText. To reserve space for single line to be added later, or to set an existing window's text to be blank, specify %A_Space%. To reserve extra lines beyond the first, append one or more linefeeds (`n).</para>
        /// <para>Once the height of MainText's control area has been set, it cannot be changed without recreating the window.</para>
        /// </param>
        /// <param name="WinTitle">The title of the window. If omitted and the window is being newly created, the title defaults to the name of the script (without path). If the B (borderless) option has been specified, there will be no visible title bar but the window can still be referred to by this title in commands such as WinMove.</param>
        /// <param name="FontName">
        /// <para>The name of the font to use for both MainText and SubText. The font table lists the fonts included with the various versions of Windows. If unspecified or if the font cannot be found, the system's default GUI font will be used.</para>
        /// <para>See the options section below for how to change the size, weight, and color of the font.</para>
        /// </param>
        public static void SplashImage(string ImageFile, string Options, string SubText, string MainText, string WinTitle, string FontName) {

            InitDialoges();

            if(string.IsNullOrEmpty(ImageFile))
                return;

            var splashOptions = new ComplexDlgOptions()
            {
                SubText = SubText,
                MainText = MainText,
                WinTitle = WinTitle,
            };
            splashOptions.ParseGuiID(ImageFile);
            splashOptions.ParseComplexOptions(Options);


            SplashImageAssync(splashOptions);
        }




        private static void SplashImageAssync(ComplexDlgOptions Options) {

            SplashDialog thisSplash = null;
            System.Drawing.Image thisImage = null;


            if(splashDialogs.ContainsKey(Options.GUIID)) {
                thisSplash = splashDialogs[Options.GUIID];
                if(thisSplash.InvokeRequired) {
                    thisSplash.Invoke(new AsyncCallDlgOptions(SplashImageAssync), Options);
                }
            }

            if(thisSplash != null) {
                Options.AppendShowHideTo(thisSplash);
            } else {
                thisSplash = new SplashDialog();
                splashDialogs.Add(Options.GUIID, thisSplash);
            }

            Options.AppendTo(thisSplash);

            #region Splash specific Options

            if(File.Exists(Options.Param1)) {
                try {
                    thisImage = System.Drawing.Bitmap.FromFile(Options.Param1);
                } catch(Exception) {
                    ErrorLevel = 1;
                    return;
                }

                if(thisImage != null) {
                    thisSplash.Image = thisImage;
                }
            }

            #endregion

            if(!Options.Hide && !thisSplash.Visible)
                thisSplash.Show();

        }

        #endregion
    }
}
