using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;

namespace IronAHK.Rusty
{
    partial class Core
    {
        class ComplexDlgOptions
        {
            #region Public Option Properties

            public int GUIID = 1;
            public string Param1; //Progress percent or Image path or off

            public bool AlwaysOnTop = true;
            public bool Borderless = false;
            public bool ShowInTaskBar = false;
            public bool Hide = false;
            public bool CenterSubText = true;
            public bool CenterMainText = true;

            public Rectangle ObjectGeometry = new Rectangle(0, 0, 0, 0);

            public Rectangle WindowGeometry = new Rectangle(0, 0, 0, 0);

            public string MainText = "";
            public string SubText = "";
            public string WinTitle = "";
            public Font MainTextFont = new Font(FontFamily.GenericSansSerif, 10);
            public Font SubTextFont = new Font(FontFamily.GenericSansSerif, 8);

            public Color? BackgroundColor = null;
            public Color TextColor = Color.Black;

            #endregion

            #region Constructor

            public ComplexDlgOptions() { }

            #endregion

            public void ParseGuiID(string Param1) {

                #region Parse Param 1:

                var RegEx_IsGUIID = new Regex("([0-9]{1,}):");
                var RegEx_GUIID = new Regex("([0-9]*):(.*)");


                if(RegEx_IsGUIID.IsMatch(Param1)) {
                    this.GUIID = Int32.Parse(RegEx_GUIID.Match(Param1).Groups[1].Captures[0].ToString());
                    this.Param1 = RegEx_GUIID.Match(Param1).Groups[2].Captures[0].ToString();
                } else {
                    this.GUIID = 1;
                    this.Param1 = Param1;
                }

                #endregion
            }

            public void ParseComplexOptions(string Options) {

                #region Setup RegExParser for Options

                var optsItems = new Dictionary<string, Regex>();

                #region RegEx: Window Size, Position, and Behavior

                optsItems.Add(Keyword_NotAlwaysOnTop, new Regex("(" + Keyword_NotAlwaysOnTop + ")"));
                optsItems.Add(Keyword_Borderless, new Regex("(" + Keyword_Borderless + ")"));
                optsItems.Add(Keyword_ProgressStartPos, new Regex(Keyword_ProgressStartPos + @"(\d*)"));
                optsItems.Add(Keyword_ProgressRange, new Regex(Keyword_ProgressRange + @"(\d*-\d*)"));
                optsItems.Add(Keyword_ShowInTaskbar, new Regex("(" + Keyword_ShowInTaskbar + ")"));

                optsItems.Add(Keyword_X, new Regex(Keyword_X + @"(\d*)"));
                optsItems.Add(Keyword_Y, new Regex(Keyword_Y + @"(\d*)"));
                optsItems.Add(Keyword_W, new Regex(Keyword_W + @"(\d*)"));
                optsItems.Add(Keyword_H, new Regex(Keyword_H + @"(\d*)"));

                optsItems.Add(Keyword_Hide, new Regex("(" + Keyword_Hide + ")"));

                #endregion

                #region RegEx: Layout of Objects in the Window

                optsItems.Add(Keyword_Centered, new Regex(Keyword_Centered + @"([0,1]{2})"));

                optsItems.Add(Keyword_ZH, new Regex(Keyword_ZH + @"([-,\d]*)"));
                optsItems.Add(Keyword_ZW, new Regex(Keyword_ZW + @"([-,\d]*)"));
                optsItems.Add(Keyword_ZX, new Regex(Keyword_ZX + @"(\d*)"));
                optsItems.Add(Keyword_ZY, new Regex(Keyword_ZY + @"(\d*)"));


                #endregion

                #region RegEx: Font Size and Weight
                //ToDo
                #endregion

                #region RegEx: Object Colors
                //ToDo
                #endregion

                #endregion

                var DicOptions = ParseOptionsRegex(ref Options, optsItems, true);

                #region Read Dict Data into Properties

                this.AlwaysOnTop = (DicOptions[Keyword_NotAlwaysOnTop] == "");
                this.Borderless = (DicOptions[Keyword_Borderless] != "");

                this.ShowInTaskBar = DicOptions[Keyword_ShowInTaskbar] != "";

                try {
                    if(DicOptions[Keyword_X] != "")
                        this.WindowGeometry.X = Int32.Parse(DicOptions[Keyword_X]);
                    if(DicOptions[Keyword_Y] != "")
                        this.WindowGeometry.Y = Int32.Parse(DicOptions[Keyword_Y]);
                    if(DicOptions[Keyword_W] != "")
                        this.WindowGeometry.Width = Int32.Parse(DicOptions[Keyword_W]);
                    if(DicOptions[Keyword_H] != "")
                        this.WindowGeometry.Height = Int32.Parse(DicOptions[Keyword_H]);
                } catch(FormatException) {
                    this.WindowGeometry = new Rectangle(0, 0, 0, 0);
                }
                this.Hide = DicOptions[Keyword_Hide] != "";

                if(DicOptions[Keyword_Centered] != "") {
                    this.CenterMainText = DicOptions[Keyword_Centered].Substring(0, 1) == "1";
                    this.CenterSubText = DicOptions[Keyword_Centered].Substring(1, 1) == "1";
                }

                try {
                    if(DicOptions[Keyword_ZX] != "")
                        this.ObjectGeometry.X = Int32.Parse(DicOptions[Keyword_ZX]);
                    if(DicOptions[Keyword_ZY] != "")
                        this.ObjectGeometry.Y = Int32.Parse(DicOptions[Keyword_ZY]);
                    if(DicOptions[Keyword_ZW] != "")
                        this.ObjectGeometry.Width = Int32.Parse(DicOptions[Keyword_ZW]);
                    if(DicOptions[Keyword_ZH] != "")
                        this.ObjectGeometry.Height = Int32.Parse(DicOptions[Keyword_ZH]);
                } catch(FormatException) {
                    this.ObjectGeometry = new Rectangle(0, 0, 0, 0);
                }
                #endregion
            }

            /// <summary>
            /// Append the given Options here to a IComplexDialoge.
            /// </summary>
            /// <param name="complexDlg"></param>
            public void AppendTo(IComplexDialoge complexDlg) {
                //ToDo: Implement the missing Options

                if(complexDlg.InvokeRequired) {
                    complexDlg.Invoke(new AsyncComplexDialoge(AppendTo), complexDlg);
                }

                complexDlg.TopMost = this.AlwaysOnTop;

                if(this.WinTitle != "")
                    complexDlg.Title = this.WinTitle;

                if(this.SubText != "")
                    complexDlg.Subtext = this.SubText;

                if(this.MainText != "")
                    complexDlg.MainText = this.MainText;


                if(!this.Hide) {
                    complexDlg.Show();
                }
            }

            public void AppendShowHideTo(IComplexDialoge complexDlg) {

                #region Handle OFF

                if(this.Param1.ToLowerInvariant() == Keyword_Off) {
                    complexDlg.Close();
                    complexDlg.Dispose();

                    if(complexDlg is SplashDialog) {
                        splashDialogs.Remove(this.GUIID);
                    } else if(complexDlg is ProgressDialog) {
                        progressDialgos.Remove(this.GUIID);
                    }

                    return;
                }

                #endregion

                #region Handle Show

                if(this.Param1.ToLowerInvariant() == Keyword_Show) {
                    if(!complexDlg.Visible)
                        complexDlg.Show();
                }

                #endregion
            }


        }
    }
}
