using System;
using System.Drawing;

namespace IronAHK.Rusty.GUI
{
    class WinForms : Base
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

        public override Base.Window CreateWindow()
        {
            return new Window();
        }

        #endregion

        #region Window

        public new class Window : Base.Window
        {
            #region Methods

            public override void Add(Base.Control control)
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

            public override Base.Text CreateText()
            {
                return new Text();
            }

            public override Base.Edit CreateEdit()
            {
                return new Edit();
            }

            public override Base.UpDown CreateUpDown()
            {
                return new UpDown();
            }

            public override Base.Picture CreatePicture()
            {
                return new Picture();
            }

            public override Base.Button CreateButton()
            {
                return new Button();
            }

            public override Base.Checkbox CreateCheckbox()
            {
                return new Checkbox();
            }

            public override Base.Radio CreateRadio()
            {
                return new Radio();
            }

            public override Base.DropDownList CreateDropDownList()
            {
                return new DropDownList();
            }

            public override Base.ComboBox CreateComboBox()
            {
                return new ComboBox();
            }

            public override Base.ListBox CreateListBox()
            {
                return new ListBox();
            }

            public override Base.ListView CreateListView()
            {
                return new ListView();
            }

            public override Base.TreeView CreateTreeView()
            {
                return new TreeView();
            }

            public override Base.Hotkey CreateHotkey()
            {
                return new Hotkey();
            }

            public override Base.DateTime CreateDateTime()
            {
                return new DateTime();
            }

            public override Base.MonthCal CreateMonthCal()
            {
                return new MonthCal();
            }

            public override Base.Slider CreateSider()
            {
                return new Slider();
            }

            public override Base.Progress CreateProgress()
            {
                return new Progress();
            }

            public override Base.GroupBox CreateGroupBox()
            {
                return new GroupBox();
            }

            public override Base.Tab CreateTab()
            {
                return new Tab();
            }

            public override Base.WebBrowser CreateWebBrowser()
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

            protected override void OnResized(Base.Window.ResizedArgs e)
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

        static void ApplyStyles(System.Windows.Forms.Control control, Base.Control information)
        {
            control.Enabled = information.Enabled;
            control.Location = information.Location;
            control.Size = information.Size;
            control.TabStop = information.TabStop;
            control.Visible = information.Visible;
            
            if (information.Transparent)
                control.BackColor = Color.Transparent;
        }

        public new class Text : Base.Text
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class Edit : Base.Edit
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class UpDown : Base.UpDown
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class Picture : Base.Picture
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class Button : Base.Button
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class Checkbox : Base.Checkbox
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class Radio : Base.Radio
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class DropDownList : Base.DropDownList
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class ComboBox : Base.ComboBox
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class ListBox : Base.ListBox
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class ListView : Base.ListView
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class TreeView : Base.TreeView
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class Hotkey : Base.Hotkey
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class DateTime : Base.DateTime
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class MonthCal : Base.MonthCal
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class Slider : Base.Slider
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class Progress : Base.Progress
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class GroupBox : Base.GroupBox
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class Tab : Base.Tab
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class StatusBar : Base.StatusBar
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        public new class WebBrowser : Base.WebBrowser
        {
            public override void Draw()
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
