using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
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
                form.SuspendLayout();
                Font = form.Font;
                Location = form.Location;
                Size = form.Size;
                Margin = new Point(form.Margin.Left, form.Margin.Top);
                WindowColour = form.BackColor;
                ControlColour = form.ForeColor;
                form.StartPosition = FormStartPosition.Manual;

                form.FormClosed += new FormClosedEventHandler(delegate(object sender, FormClosedEventArgs e)
                {
                    OnClosed(new ClosedArgs());
                });

                form.KeyPress += new KeyPressEventHandler(delegate(object sender, KeyPressEventArgs e)
                {
                    if (e.KeyChar == (char)Keys.Escape)
                        OnEscaped(new EscapedArgs());
                });

                form.Resize += new EventHandler(delegate(object sender, EventArgs e)
                {
                    int mode = 0;

                    switch (form.WindowState)
                    {
                        case FormWindowState.Minimized: mode = 1; break;
                        case FormWindowState.Maximized: mode = 2; break;
                    }

                    OnResized(new ResizedArgs(mode));
                });
            }

            [DllImport("user32.dll")]
            static extern bool FlashWindow(IntPtr hWnd, bool bInvert);

            #region Methods

            public override void Add(BaseGui.Control control)
            {
                form.Controls.Add((System.Windows.Forms.Control)control.NativeComponent);
                control.Parent.Controls.Add(control);
                control.Draw();
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

                form.ResumeLayout(true);
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

            if (information.Location.IsEmpty)
            {
                int n = information.Parent.Controls.Count - 1;

                if (n == 0)
                    information.Location = information.Parent.Margin;
                else
                {
                    var last = information.Parent.Controls[n - 1];
                    var component = (System.Windows.Forms.Control)information.NativeComponent;
                    var location = last.Location;
                    location.Y += last.Size.Height + component.Padding.Bottom;
                    information.Location = location;
                }
            }

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
            public Edit()
            {
                NativeComponent = new TextBox();
            }

            public override void Draw()
            {
                var edit = (TextBox)NativeComponent;
                ApplyStyles(edit, this);
                if (Size.IsEmpty)
                {
                    float w = 5 + edit.CreateGraphics().MeasureString(Contents, edit.Font).Width;
                    edit.Size = new Size((int)w, edit.Size.Height);
                }
                edit.MaxLength = Limit < 0 ? int.MaxValue : Limit;
                edit.CharacterCasing = Lowercase ? CharacterCasing.Lower : Uppercase ? CharacterCasing.Upper : CharacterCasing.Normal;
                edit.Multiline = Multi;
                if (Password)
                    edit.PasswordChar = '●';
                edit.ReadOnly = ReadOnly;
                edit.AcceptsReturn = WantReturn;
                edit.WordWrap = Wrap;
                edit.AcceptsTab = WantTab;
                edit.Show();
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
            public DropDownList()
            {
                NativeComponent = new System.Windows.Forms.ComboBox();
            }

            public override void Draw()
            {
                var ddl = (System.Windows.Forms.ComboBox)NativeComponent;
                if (Size.IsEmpty)
                {
                    float w = 5 + ddl.CreateGraphics().MeasureString(Contents, ddl.Font).Width;
                    Size = new Size((int)w, ddl.Size.Height);
                }
                ApplyStyles(ddl, this);
                ddl.Sorted = Sort;
                if (Uppercase)
                    Contents = Contents.ToUpperInvariant();
                else if (Lowercase)
                    Contents = Contents.ToLowerInvariant();
                ddl.Items.AddRange(Contents.Split(new[] { Parent.Delimieter }));
                if (Choose > -1 && Choose <= ddl.Items.Count)
                    ddl.SelectedIndex = Choose - 1;
                ddl.DropDownStyle = ComboBoxStyle.DropDownList;
                ddl.Show();
            }
        }

        public new class ComboBox : BaseGui.ComboBox
        {
            public ComboBox()
            {
                NativeComponent = new System.Windows.Forms.ComboBox();
            }

            public override void Draw()
            {
                var combo = (System.Windows.Forms.ComboBox)NativeComponent;
                if (Size.IsEmpty)
                {
                    float w = 5 + combo.CreateGraphics().MeasureString(Contents, combo.Font).Width;
                    Size = new Size((int)w, combo.Size.Height);
                }
                ApplyStyles(combo, this);
                combo.Items.AddRange(Contents.Split(new[] { Parent.Delimieter }));
                if (combo.Items.Count > 0)
                    combo.SelectedIndex = 0;
                combo.Show();
            }
        }

        public new class ListBox : BaseGui.ListBox
        {
            public ListBox()
            {
                NativeComponent = new System.Windows.Forms.ListBox();
            }

            public override void Draw()
            {
                var listbox = (System.Windows.Forms.ListBox)NativeComponent;
                ApplyStyles(listbox, this);
                listbox.AutoSize = Size.IsEmpty;
                listbox.Items.AddRange(Contents.Split(new[] { Parent.Delimieter }));
                if (Choose > -1 && Choose <= listbox.Items.Count)
                    listbox.SelectedIndex = Choose - 1;
                listbox.SelectionMode = MultiSelect ? SelectionMode.MultiExtended : ReadOnly ? SelectionMode.None : SelectionMode.One;
                listbox.Sorted = Sort;
                listbox.Show();
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
            Keys key, mod;

            public Hotkey()
            {
                var ctrl = new TextBox();
                NativeComponent = ctrl;

                ctrl.Multiline = false;
                ctrl.ContextMenu = new ContextMenu();
                key = mod = Keys.None;
                ctrl.Text = Enum.GetName(typeof(Keys), key);

                ctrl.KeyPress += new KeyPressEventHandler(delegate(object sender, KeyPressEventArgs e)
                {
                    e.Handled = true;
                });

                ctrl.KeyUp += new KeyEventHandler(delegate(object sender, KeyEventArgs e)
                {
                    if (e.KeyCode == Keys.None && e.Modifiers == Keys.None)
                        key = Keys.None;
                });

                ctrl.KeyDown += new KeyEventHandler(delegate(object sender, KeyEventArgs e)
                {
                    if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
                        key = mod = Keys.None;
                    else
                    {
                        key = e.KeyCode;
                        mod = e.Modifiers;
                        Validate();
                    }

                    SetText();
                });
            }

            void Validate()
            {
                Keys[,] sym = { { Keys.Control, Keys.ControlKey }, { Keys.Shift, Keys.ShiftKey }, { Keys.Alt, Keys.Menu } };

                for (int i = 0; i < 3; i++)
                {
                    if (key == sym[i, 1] && (mod & sym[i, 0]) == sym[i, 0])
                        mod &= ~sym[i, 0];
                }

                if ((Limit & 1) == 1)
                {
                    if (mod == Keys.None)
                        key = Keys.None;
                }

                if ((Limit & 2) == 2)
                {
                    if (mod == Keys.Shift)
                        key = mod = Keys.None;
                }

                if ((Limit & 4) == 4)
                {
                    if (mod == Keys.Control)
                        key = mod = Keys.None;
                }

                if ((Limit & 8) == 8)
                {
                    if (mod == Keys.Alt)
                        key = mod = Keys.None;
                }

                if ((Limit & 16) == 16)
                {
                    if ((mod & Keys.Shift) == Keys.Shift && (mod & Keys.Control) == Keys.Control && (mod & Keys.Alt) != Keys.Alt)
                        key = mod = Keys.None;
                }

                if ((Limit & 32) == 32)
                {
                    if ((mod & Keys.Shift) == Keys.Shift && (mod & Keys.Control) != Keys.Control && (mod & Keys.Control) == Keys.Alt)
                        key = mod = Keys.None;
                }

                if ((Limit & 128) == 128)
                {
                    if ((mod & Keys.Shift) == Keys.Shift && (mod & Keys.Control) == Keys.Control && (mod & Keys.Control) == Keys.Alt)
                        key = mod = Keys.None;
                }
            }

            void SetText()
            {
                var ctrl = (TextBox)NativeComponent;
                var buf = new StringBuilder(45);
                const string sep = " + ";

                if ((mod & Keys.Control) == Keys.Control)
                {
                    buf.Append(Enum.GetName(typeof(Keys), Keys.Control));
                    buf.Append(sep);
                }

                if ((mod & Keys.Shift) == Keys.Shift)
                {
                    buf.Append(Enum.GetName(typeof(Keys), Keys.Shift));
                    buf.Append(sep);
                }

                if ((mod & Keys.Alt) == Keys.Alt)
                {
                    buf.Append(Enum.GetName(typeof(Keys), Keys.Alt));
                    buf.Append(sep);
                }

                buf.Append(key.ToString());
                ctrl.Text = buf.ToString();
            }

            public override void Draw()
            {
                var ctrl = (TextBox)NativeComponent;
                ApplyStyles(ctrl, this);
                if (Size.IsEmpty)
                {
                    float w = 5 + ctrl.CreateGraphics().MeasureString(Contents, ctrl.Font).Width;
                    ctrl.Size = new Size((int)w, ctrl.Size.Height);
                }
                SetText();
                ctrl.Show();
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
            public MonthCal()
            {
                NativeComponent = new MonthCalendar();
            }

            public override void Draw()
            {
                var cal = (MonthCalendar)NativeComponent;
                ApplyStyles(cal, this);
                cal.AutoSize = Size.IsEmpty;
                cal.MaxSelectionCount = MultiSelect ? int.MaxValue : 1;
                if (RangeMinimum != default(System.DateTime) && RangeMaximum != default(System.DateTime) && RangeMinimum < RangeMaximum)
                    cal.SelectionRange = new SelectionRange(RangeMinimum, RangeMaximum);
                cal.SelectionStart = RangeMinimum;
                cal.ShowWeekNumbers = DisplayWeek;
                cal.ShowTodayCircle = HightlightToday;
                cal.ShowToday = DisplayToday;
                long time;
                if (long.TryParse(Contents, out time))
                    cal.TodayDate = new System.DateTime(time);
                cal.Show();
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
            public Progress()
            {
                NativeComponent = new ProgressBar();
            }

            public override void Draw()
            {
                var progress = (ProgressBar)NativeComponent;
                ApplyStyles(progress, this);
                if (Size.IsEmpty)
                    progress.Size = new Size((int)(progress.Parent.Font.Size * 15), (int)(progress.Parent.Font.Size * 2));
                progress.BackColor = BackgroundColor;
                progress.Minimum = RangeMinimum;
                progress.Maximum = RangeMaximum;
                int n;
                if (int.TryParse(Contents, out n))
                    progress.Value = n;
                progress.AutoSize = Size.IsEmpty;
                progress.Show();
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

            public override void SetText(int part, string text)
            {
                var status = (System.Windows.Forms.StatusStrip)NativeComponent;

                if (part < status.Items.Count)
                {
                    status.Items[part].Text = text;
                    return;
                }

                for (int m = status.Items.Count - 1, i = 0; i < m; i++)
                    status.Items.Add(string.Empty);

                status.Items.Add(text);
            }

            public override void SetParts(params int[] width)
            {
                var status = (System.Windows.Forms.StatusStrip)NativeComponent;

                if (width.Length == 0)
                {
                    status.Items.Clear();
                    return;
                }

                for (int m = status.Items.Count, i = 0; i < width.Length; i++)
                {
                    if (i >= m)
                        status.Items.Add(string.Empty);
                    status.Items[i].Width = width[i];
                }
           } 

            public override void SetIcon(int part, Image icon)
            {
                var status = (System.Windows.Forms.StatusStrip)NativeComponent;

                if (part < status.Items.Count)
                    status.Items[part].Image = icon;
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
