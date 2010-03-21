using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    class WinForms : BaseGui
    {
        #region Information

        public override bool Available
        {
            get { return true; }
        }

        public override string Name
        {
            get { return "WinForms"; }
        }

        public override BaseGui.Window CreateWindow()
        {
            return new Window();
        }

        #endregion

        #region Window

        public new class Window : BaseGui.Window
        {
            Form form;

            public Window()
            {
                form = new Form();
                Font = form.Font;
                Location = form.Location;
                Size = form.Size;
                WindowColour = form.BackColor;
                ControlColour = form.ForeColor;
                form.StartPosition = FormStartPosition.Manual;
            }

            [DllImport("user32.dll")]
            static extern bool FlashWindow(IntPtr hWnd, bool bInvert);

            #region Methods

            public override void Add(BaseGui.Control control)
            {
                form.Controls.Add((System.Windows.Forms.Control)control.NativeComponent);
                control.Draw();
                form.PerformLayout();
            }

            public override void Draw(string title)
            {
                form.Size = Size;
                form.Location = Location;
                form.Text = title;
                form.Enabled = Enabled;
                form.MaximizeBox = MaximiseBox;
                form.MinimizeBox = MinimiseBox;
                form.MinimumSize = MinimumSize;
                form.MaximumSize = MaximumSize;

                if (Resize)
                    form.FormBorderStyle = ToolWindow ? FormBorderStyle.SizableToolWindow : FormBorderStyle.Sizable;
                else
                    form.FormBorderStyle = ToolWindow ? FormBorderStyle.FixedToolWindow : (Border ? FormBorderStyle.Fixed3D : FormBorderStyle.FixedSingle);

                if (!Caption)
                    form.FormBorderStyle = FormBorderStyle.None;

                if (!SysMenu)
                {
                    form.FormBorderStyle = FormBorderStyle.FixedDialog;
                    form.MinimizeBox = form.MaximizeBox = false;
                }

                form.TopMost = AlwaysOnTop;

                if (Theme)
                    Application.EnableVisualStyles();
            }

            public override void Show()
            {
                form.Show();
            }

            public override void AutoSize()
            {
                form.AutoSize = true;
            }

            public override Dictionary<string, string> Submit(bool hide)
            {
                if (hide)
                    Cancel();

                var table = new Dictionary<string, string>(Controls.Count);

                foreach (var control in Controls)
                    table.Add(control.Id, control.Contents);

                return table;
            }

            public override void Cancel()
            {
                form.Hide();
            }

            public override void Destroy()
            {
                form.Dispose();
            }

            public override void Minimise()
            {
                form.WindowState = FormWindowState.Minimized;
            }

            public override void Maximise()
            {
                form.WindowState = FormWindowState.Minimized;
            }

            public override void Restore()
            {
                form.WindowState = FormWindowState.Normal;
            }

            public override void Flash(bool off)
            {
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    FlashWindow(form.Handle, off);
            }

            #region Controls

            public override BaseGui.Text CreateText()
            {
                return new Text { Parent = this };
            }

            public override BaseGui.Edit CreateEdit()
            {
                return new Edit { Parent = this };
            }

            public override BaseGui.UpDown CreateUpDown()
            {
                return new UpDown { Parent = this };
            }

            public override BaseGui.Picture CreatePicture()
            {
                return new Picture { Parent = this };
            }

            public override BaseGui.Button CreateButton()
            {
                return new Button { Parent = this };
            }

            public override BaseGui.Checkbox CreateCheckbox()
            {
                return new Checkbox { Parent = this };
            }

            public override BaseGui.Radio CreateRadio()
            {
                return new Radio { Parent = this };
            }

            public override BaseGui.DropDownList CreateDropDownList()
            {
                return new DropDownList { Parent = this };
            }

            public override BaseGui.ComboBox CreateComboBox()
            {
                return new ComboBox { Parent = this };
            }

            public override BaseGui.ListBox CreateListBox()
            {
                return new ListBox { Parent = this };
            }

            public override BaseGui.ListView CreateListView()
            {
                return new ListView { Parent = this };
            }

            public override BaseGui.TreeView CreateTreeView()
            {
                return new TreeView { Parent = this };
            }

            public override BaseGui.Hotkey CreateHotkey()
            {
                return new Hotkey { Parent = this };
            }

            public override BaseGui.DateTime CreateDateTime()
            {
                return new DateTime { Parent = this };
            }

            public override BaseGui.MonthCal CreateMonthCal()
            {
                return new MonthCal { Parent = this };
            }

            public override BaseGui.Slider CreateSider()
            {
                return new Slider { Parent = this };
            }

            public override BaseGui.Progress CreateProgress()
            {
                return new Progress { Parent = this };
            }

            public override BaseGui.GroupBox CreateGroupBox()
            {
                return new GroupBox { Parent = this };
            }

            public override BaseGui.Tab CreateTab()
            {
                return new Tab { Parent = this };
            }

            public override BaseGui.StatusBar CreateStatusBar()
            {
                return new StatusBar { Parent = this };
            }

            public override BaseGui.WebBrowser CreateWebBrowser()
            {
                return new WebBrowser { Parent = this };
            }

            #endregion

            #endregion

            #region Events

            protected override void OnClosed(ClosedArgs e)
            {
                base.OnClosed(e);
            }

            protected override void OnEscaped(EscapedArgs e)
            {
                base.OnEscaped(e);
            }

            protected override void OnResized(BaseGui.Window.ResizedArgs e)
            {
                base.OnResized(e);
            }

            protected override void OnContextMenu(ContextMenuArgs e)
            {
                base.OnContextMenu(e);
            }

            protected override void OnDroppedFiles(DroppedFilesArgs e)
            {
                base.OnDroppedFiles(e);
            }

            #endregion
        }

        #endregion

        #region Controls

        static void ApplyStyles(System.Windows.Forms.Control control, BaseGui.Control information)
        {
            if (!information.Size.IsEmpty && information.Size.Height == 0)
                information.Size = new Size(information.Size.Width, (int)information.Parent.Font.GetHeight());

            control.Text = information.Contents;
            control.Enabled = information.Enabled;
            control.Location = information.Location;
            control.Size = information.Size;
            control.TabStop = information.TabStop;
            control.Visible = information.Visible;

            if (information.Transparent)
                control.BackColor = Color.Transparent;
        }

        public new class Text : BaseGui.Text
        {
            public Text()
            {
                NativeComponent = new Label { AutoSize = true };
            }

            public override void Draw()
            {
                var text = (Label)NativeComponent;
                ApplyStyles(text, this);
                text.BorderStyle = Border ? BorderStyle.FixedSingle : BorderStyle.None;
                text.AutoSize = Size.IsEmpty;
                text.Show();
            }
        }

        public new class Edit : BaseGui.Edit
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class UpDown : BaseGui.UpDown
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class Picture : BaseGui.Picture
        {
            public Picture()
            {
                NativeComponent = new PictureBox();
            }

            public override void Draw()
            {
                var pic = (PictureBox)NativeComponent;

                if (Size.IsEmpty)
                {
                    try
                    {
                        var image = Image.FromFile(Contents);
                        Size = image.Size;
                    }
                    catch (FileNotFoundException) { }
                }

                ApplyStyles(pic, this);
                pic.ImageLocation = Contents;
                pic.BorderStyle = Border ? BorderStyle.FixedSingle : BorderStyle.None;
                pic.Show();
            }
        }

        public new class Button : BaseGui.Button
        {
            public Button()
            {
                NativeComponent = new System.Windows.Forms.Button();
            }

            public override void Draw()
            {
                var button = (System.Windows.Forms.Button)NativeComponent;
                ApplyStyles(button, this);
                button.AutoSize = Size.IsEmpty;
                button.Show();
            }
        }

        public new class Checkbox : BaseGui.Checkbox
        {
            public Checkbox()
            {
                NativeComponent = new CheckBox();
            }

            public override void Draw()
            {
                var check = (CheckBox)NativeComponent;
                ApplyStyles(check, this);
                check.AutoSize = Size.IsEmpty;
                check.CheckState = State;
                check.Show();
            }
        }

        public new class Radio : BaseGui.Radio
        {
            public Radio()
            {
                NativeComponent = new RadioButton();
            }

            public override void Draw()
            {
                var radio = (RadioButton)NativeComponent;
                ApplyStyles(radio, this);
                radio.AutoSize = Size.IsEmpty;
                radio.Checked = Checked;
                radio.Show();
            }
        }

        public new class DropDownList : BaseGui.DropDownList
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class ComboBox : BaseGui.ComboBox
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class ListBox : BaseGui.ListBox
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class ListView : BaseGui.ListView
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class TreeView : BaseGui.TreeView
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class Hotkey : BaseGui.Hotkey
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class DateTime : BaseGui.DateTime
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class MonthCal : BaseGui.MonthCal
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class Slider : BaseGui.Slider
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class Progress : BaseGui.Progress
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class GroupBox : BaseGui.GroupBox
        {
            public GroupBox()
            {
                NativeComponent = new System.Windows.Forms.GroupBox();
            }

            public override void Draw()
            {
                var group = (System.Windows.Forms.GroupBox)NativeComponent;
                ApplyStyles(group, this);
                group.AutoSize = Size.IsEmpty;
                group.Show();
            }
        }

        public new class Tab : BaseGui.Tab
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class StatusBar : BaseGui.StatusBar
        {
            public StatusBar()
            {
                NativeComponent = new System.Windows.Forms.StatusStrip();
            }

            public override void Draw()
            {
                var status = (System.Windows.Forms.StatusStrip)NativeComponent;

                if (status.Items.Count == 0)
                    status.Items.Add(Contents);
                else
                    status.Items[0].Text = Contents;

                ApplyStyles(status, this);
                status.Show();
            }
        }

        public new class WebBrowser : BaseGui.WebBrowser
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
