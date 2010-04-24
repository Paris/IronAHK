using System;
using System.Collections.Generic;
using System.Drawing;
using Gtk;

[assembly: CLSCompliant(true)]

namespace IronAHK.Rusty
{
    public class GtkForms : BaseGui
    {
        #region Information

        public override bool Available
        {
            get { return true; }
        }

        public override string Name
        {
            get { return "Gtk"; }
        }

        public override BaseGui.Window CreateWindow()
        {
            return new Window();
        }

        #endregion

        #region Window

        public new class Window : BaseGui.Window
        {
            Gtk.Window form;

            public Window()
            {
                form = new Gtk.Window(string.Empty);
            }

            #region Methods

            public override void Add(BaseGui.Control control)
            {
                throw new NotImplementedException();
            }

            public override void Remove(Control control)
            {
                throw new NotImplementedException();
            }

            public override void Draw(string title)
            {
                throw new NotImplementedException();
            }

            public override void Show()
            {
                throw new NotImplementedException();
            }

            public override void AutoSize()
            {
                throw new NotImplementedException();
            }

            public override Dictionary<string, string> Submit(bool hide)
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

            public override void Flash(bool off)
            {
                throw new NotImplementedException();
            }

            public override void Focus(Control control)
            {
                throw new NotImplementedException();
            }

            public override void ChangeFont(Control control)
            {
                throw new NotImplementedException();
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

            public override int Add(string options, params string[] fields)
            {
                throw new NotImplementedException();
            }

            public override int CountColumn(string options)
            {
                throw new NotImplementedException();
            }

            public override int Delete(int row)
            {
                throw new NotImplementedException();
            }

            public override int DeleteColumn(int column)
            {
                throw new NotImplementedException();
            }

            public override int Insert(int row, string options, params string[] columns)
            {
                throw new NotImplementedException();
            }

            public override int InsertColumn(int column, string options, string title)
            {
                throw new NotImplementedException();
            }

            public override int Modify(int row, string options, params string[] columns)
            {
                throw new NotImplementedException();
            }

            public override int ModifyColumn(int column, string options, string title)
            {
                throw new NotImplementedException();
            }

            public override int Next(int row, string options)
            {
                throw new NotImplementedException();
            }

            public override string Text(int row, int column)
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

            public override int Add(string text, int parent)
            {
                throw new NotImplementedException();
            }

            public override int Child(int parent)
            {
                throw new NotImplementedException();
            }

            public override int Count()
            {
                throw new NotImplementedException();
            }

            public override void Delete(int id)
            {
                throw new NotImplementedException();
            }

            public override int Get(int id, string mode)
            {
                throw new NotImplementedException();
            }

            public override int Modify(int id, string options, string text)
            {
                throw new NotImplementedException();
            }

            public override int Next(int id, string mode)
            {
                throw new NotImplementedException();
            }

            public override int ParentOf(int id)
            {
                throw new NotImplementedException();
            }

            public override int Previous(int id)
            {
                throw new NotImplementedException();
            }

            public override int Selection()
            {
                throw new NotImplementedException();
            }

            public override string Text(int id)
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

            public override void SetIcon(int part, System.Drawing.Image icon)
            {
                throw new NotImplementedException();
            }

            public override void SetParts(params int[] width)
            {
                throw new NotImplementedException();
            }

            public override void SetText(int part, string text)
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
