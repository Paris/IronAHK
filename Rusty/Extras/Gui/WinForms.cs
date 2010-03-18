using System;
using System.Collections.Generic;
using System.Drawing;
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
                form.StartPosition = FormStartPosition.Manual;
            }

            [DllImport("user32.dll")]
            static extern bool FlashWindow(IntPtr hWnd, bool bInvert);

            #region Methods

            public override void Add(BaseGui.Control control)
            {
                throw new NotImplementedException();
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
                return new Text();
            }

            public override BaseGui.Edit CreateEdit()
            {
                return new Edit();
            }

            public override BaseGui.UpDown CreateUpDown()
            {
                return new UpDown();
            }

            public override BaseGui.Picture CreatePicture()
            {
                return new Picture();
            }

            public override BaseGui.Button CreateButton()
            {
                return new Button();
            }

            public override BaseGui.Checkbox CreateCheckbox()
            {
                return new Checkbox();
            }

            public override BaseGui.Radio CreateRadio()
            {
                return new Radio();
            }

            public override BaseGui.DropDownList CreateDropDownList()
            {
                return new DropDownList();
            }

            public override BaseGui.ComboBox CreateComboBox()
            {
                return new ComboBox();
            }

            public override BaseGui.ListBox CreateListBox()
            {
                return new ListBox();
            }

            public override BaseGui.ListView CreateListView()
            {
                return new ListView();
            }

            public override BaseGui.TreeView CreateTreeView()
            {
                return new TreeView();
            }

            public override BaseGui.Hotkey CreateHotkey()
            {
                return new Hotkey();
            }

            public override BaseGui.DateTime CreateDateTime()
            {
                return new DateTime();
            }

            public override BaseGui.MonthCal CreateMonthCal()
            {
                return new MonthCal();
            }

            public override BaseGui.Slider CreateSider()
            {
                return new Slider();
            }

            public override BaseGui.Progress CreateProgress()
            {
                return new Progress();
            }

            public override BaseGui.GroupBox CreateGroupBox()
            {
                return new GroupBox();
            }

            public override BaseGui.Tab CreateTab()
            {
                return new Tab();
            }

            public override BaseGui.WebBrowser CreateWebBrowser()
            {
                return new WebBrowser();
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
            public override void Draw()
            {
                throw new NotImplementedException();
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
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class Button : BaseGui.Button
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class Checkbox : BaseGui.Checkbox
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class Radio : BaseGui.Radio
        {
            public override void Draw()
            {
                throw new NotImplementedException();
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
            public override void Draw()
            {
                throw new NotImplementedException();
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
            public override void Draw()
            {
                throw new NotImplementedException();
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
