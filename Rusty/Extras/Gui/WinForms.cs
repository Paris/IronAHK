using System;
using System.Drawing;

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
            #region Methods

            public override void Add(BaseGui.Control control)
            {
                throw new NotImplementedException();
            }

            public override void Show()
            {
                throw new NotImplementedException();
            }

            public override void Submit(bool hide)
            {
                throw new NotImplementedException();
            }

            public override void Cancel()
            {
                throw new NotImplementedException();
            }

            public override void Destroy()
            {
                throw new NotImplementedException();
            }

            public override void Minimise()
            {
                throw new NotImplementedException();
            }

            public override void Maximise()
            {
                throw new NotImplementedException();
            }

            public override void Restore()
            {
                throw new NotImplementedException();
            }

            public override void Flash()
            {
                throw new NotImplementedException();
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
