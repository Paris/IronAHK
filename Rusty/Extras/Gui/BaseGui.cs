using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    abstract class BaseGui
    {
        #region Information

        public abstract bool Available { get; }

        public abstract string Name { get; }

        public abstract Window CreateWindow();

        #endregion

        #region Window

        public abstract class Window
        {
            #region Methods

            public abstract void Add(Control control);
            public abstract void Draw(string title);
            public abstract void Show();
            public abstract void AutoSize();
            public abstract Dictionary<string, string> Submit(bool hide);
            public abstract void Cancel();
            public abstract void Destroy();
            public abstract void Minimise();
            public abstract void Maximise();
            public abstract void Restore();
            public abstract void Flash(bool off);

            #region Controls

            public abstract Text CreateText();
            public abstract Edit CreateEdit();
            public abstract UpDown CreateUpDown();
            public abstract Picture CreatePicture();
            public abstract Button CreateButton();
            public abstract Checkbox CreateCheckbox();
            public abstract Radio CreateRadio();
            public abstract DropDownList CreateDropDownList();
            public abstract ComboBox CreateComboBox();
            public abstract ListBox CreateListBox();
            public abstract ListView CreateListView();
            public abstract TreeView CreateTreeView();
            public abstract Hotkey CreateHotkey();
            public abstract DateTime CreateDateTime();
            public abstract MonthCal CreateMonthCal();
            public abstract Slider CreateSider();
            public abstract Progress CreateProgress();
            public abstract GroupBox CreateGroupBox();
            public abstract StatusBar CreateStatusBar();
            public abstract Tab CreateTab();
            public abstract WebBrowser CreateWebBrowser();

            #endregion

            #endregion

            #region Events

            #region Args

            public class ClosedArgs : EventArgs { }

            public class EscapedArgs : EventArgs { }

            public class ResizedArgs : EventArgs
            {
                int mode;

                public ResizedArgs(int mode)
                {
                    this.mode = mode;
                }

                public int Mode
                {
                    get { return mode; }
                }
            }

            public class ContextMenuArgs : EventArgs
            {
                Control control;
                object target;
                Point location;
                bool rightclick;

                public ContextMenuArgs(Control control, object target, Point point, bool rightclick)
                {
                    this.control = control;
                    this.target = target;
                    this.location = point;
                    this.rightclick = rightclick;
                }

                public Control Control
                {
                    get { return control; }
                }

                public object Target
                {
                    get { return target; }
                }

                public Point Location
                {
                    get { return location; }
                }

                public bool RightClick
                {
                    get { return rightclick; }
                }
            }

            public class DroppedFilesArgs : EventArgs
            {
                Control control;
                int number;
                Point location;
                string[] filenames;

                public DroppedFilesArgs(Control control, int number, Point location, string[] filenames)
                {
                    this.control = control;
                    this.number = number;
                    this.location = location;
                    this.filenames = filenames;
                }

                public Control Control
                {
                    get { return control; }
                }

                public int Number
                {
                    get { return number; }
                }

                public Point Location
                {
                    get { return location; }
                }

                public string[] FileNames
                {
                    get { return filenames; }
                }
            }

            #endregion

            #region Handlers

            public event EventHandler<ClosedArgs> Closed;

            public event EventHandler<EscapedArgs> Escaped;

            public event EventHandler<ResizedArgs> Resized;

            public event EventHandler<ContextMenuArgs> ContextOpened;

            public event EventHandler<DroppedFilesArgs> DroppedFiles;

            #endregion

            #region Methods

            protected virtual void OnClosed(ClosedArgs e)
            {
                var handler = Closed;

                if (handler != null)
                    handler(this, e);
            }

            protected virtual void OnEscaped(EscapedArgs e)
            {
                var handler = Escaped;

                if (handler != null)
                    handler(this, e);
            }

            protected virtual void OnResized(ResizedArgs e)
            {
                var handler = Resized;

                if (handler != null)
                    handler(this, e);
            }

            protected virtual void OnContextMenu(ContextMenuArgs e)
            {
                var handler = ContextOpened;

                if (handler != null)
                    handler(this, e);
            }

            protected virtual void OnDroppedFiles(DroppedFilesArgs e)
            {
                var handler = DroppedFiles;

                if (handler != null)
                    handler(this, e);
            }

            #endregion

            #endregion

            #region Properties

            List<Control> controls = new List<Control>();

            Size size;
            Point location;
            Font font;
            Color windowColour, controlColour;
            Point margin;
            Menu menu;

            bool alwaysontop = false;
            bool border = false;
            bool caption = true;
            char delimiter = '|';
            bool enabled = true;
            string label = "Gui";
            bool lastfound = false;
            bool lastfoundexist = false;
            bool maxbox = true;
            bool minbox = true;
            Size minsize, maxsize;
            bool owndialogs = false;
            bool owner = false;
            bool resize = true;
            bool sysmenu = true;
            bool theme = true;
            bool toolwindow = false;

            StatusBar statusBar;

            #endregion

            #region Accessors

            public List<Control> Controls
            {
                get { return controls; }
            }

            public Size Size
            {
                get { return size; }
                set { size = value; }
            }

            public Point Location
            {
                get { return location; }
                set { location = value; }
            }

            public Font Font
            {
                get { return font; }
                set { font = value; }
            }

            public Color WindowColour
            {
                get { return windowColour; }
                set { windowColour = value; }
            }

            public Color ControlColour
            {
                get { return controlColour; }
                set { controlColour = value; }
            }

            public Point Margin
            {
                get { return margin; }
                set { margin = value; }
            }

            public Menu Menu
            {
                get { return menu; }
                set { menu = value; }
            }

            public bool AlwaysOnTop
            {
                get { return alwaysontop; }
                set { alwaysontop = value; }
            }

            public bool Border
            {
                get { return border; }
                set { border = value; }
            }

            public bool Caption
            {
                get { return caption; }
                set { caption = value; }
            }

            public char Delimieter
            {
                get { return delimiter; }
                set { delimiter = value; }
            }

            public bool Enabled
            {
                get { return enabled; }
                set { enabled = value; }
            }

            public string Label
            {
                get { return label; }
                set { label = value; }
            }

            public bool LastFound
            {
                get { return lastfound; }
                set { lastfound = value; }
            }

            public bool LastFoundExist
            {
                get { return lastfoundexist; }
                set { lastfoundexist = value; }
            }

            public bool MinimiseBox
            {
                get { return minbox; }
                set { minbox = value; }
            }

            public bool MaximiseBox
            {
                get { return maxbox; }
                set { maxbox = value; }
            }

            public Size MinimumSize
            {
                get { return minsize; }
                set { minsize = value; }
            }

            public Size MaximumSize
            {
                get { return maxsize; }
                set { maxsize = value; }
            }

            public bool OwnDialogs
            {
                get { return owndialogs; }
                set { owndialogs = value; }
            }

            public bool Owner
            {
                get { return owner; }
                set { owner = value; }
            }

            public bool Resize
            {
                get { return resize; }
                set { resize = value; }
            }

            public bool SysMenu
            {
                get { return sysmenu; }
                set { sysmenu = value; }
            }

            public bool Theme
            {
                get { return theme; }
                set { theme = value; }
            }

            public bool ToolWindow
            {
                get { return toolwindow; }
                set { toolwindow = value; }
            }

            public StatusBar StatusBar
            {
                get { return statusBar; }
                set { statusBar = value; }
            }

            #endregion
        }

        #endregion

        #region Controls

        public abstract class Control
        {
            public abstract void Draw();

            object native;
            Window parent;
            Point location;
            Size size;
            string id;
            string contents;

            bool altsubmit = false;
            Color colour;
            bool enabled = true;
            bool visible = true;
            ContentAlignment align;
            bool tabstop = true;
            bool wrap = false;
            bool vscroll = false;
            bool hscroll = false;

            bool transparent = false;
            bool background = true;
            bool border = false;
            bool theme = true;

            public object NativeComponent
            {
                get { return native; }
                set { native = value; }
            }

            public Window Parent
            {
                get { return parent; }
                set { parent = value; }
            }

            public Point Location
            {
                get { return location; }
                set { location = value; }
            }

            public Size Size
            {
                get { return size; }
                set { size = value; }
            }

            public string Id
            {
                get { return id; }
                set { id = value; }
            }

            public string Contents
            {
                get { return contents; }
                set { contents = value; }
            }

            public bool AltSubmit
            {
                get { return altsubmit; }
                set { altsubmit = value; }
            }

            public Color Colour
            {
                get { return colour; }
                set { colour = value; }
            }

            public bool Enabled
            {
                get { return enabled; }
                set { enabled = value; }
            }

            public bool Visible
            {
                get { return visible; }
                set { visible = value; }
            }

            public ContentAlignment Alignment
            {
                get { return align; }
                set { align = value; }
            }

            public bool TabStop
            {
                get { return tabstop; }
                set { tabstop = value; }
            }

            public bool Wrap
            {
                get { return wrap; }
                set { wrap = value; }
            }

            public bool VerticalScroll
            {
                get { return vscroll; }
                set { vscroll = value; }
            }

            public bool HorizontalScroll
            {
                get { return hscroll; }
                set { hscroll = value; }
            }

            public bool Transparent
            {
                get { return transparent; }
                set { transparent = value; }
            }

            public bool Background
            {
                get { return background; }
                set { background = value; }
            }

            public bool Border
            {
                get { return border; }
                set { border = value; }
            }

            public bool Theme
            {
                get { return theme; }
                set { theme = value; }
            }
        }

        public abstract class Text : Control
        {
            bool readOnly = false;

            public bool ReadOnly
            {
                get { return readOnly; }
                set { readOnly = value; }
            }
        }

        public abstract class Edit : Text
        {
            int limit = -1;
            bool lowercase = false;
            bool multi = false;
            bool number = false;
            bool password = false;
            int tabstops = -1;
            bool uppercase = false;
            bool wantctrla = true;
            bool wantreturn = true;
            bool wanttab = false;

            public int Limit
            {
                get { return limit; }
                set { limit = value; }
            }

            public bool Lowercase
            {
                get { return lowercase; }
                set { lowercase = value; }
            }

            public bool Multi
            {
                get { return multi; }
                set { multi = value; }
            }

            public bool Number
            {
                get { return number; }
                set { number = value; }
            }

            public bool Password
            {
                get { return password; }
                set { password = value; }
            }

            public int TabStops
            {
                get { return tabstops; }
                set { tabstops = value; }
            }

            public bool Uppercase
            {
                get { return uppercase; }
                set { uppercase = value; }
            }

            public bool WantCtrlA
            {
                get { return wantctrla; }
                set { wantctrla = value; }
            }

            public bool WantReturn
            {
                get { return wantreturn; }
                set { wantreturn = value; }
            }

            public bool WantTab
            {
                get { return wanttab; }
                set { wanttab = value; }
            }
        }

        public abstract class UpDown : Control
        {
            bool horizontal = false;
            int rangeMin = 0, rangeMax = 100;
            bool isolated = false;
            bool formatted = true;
            bool left = false;
            bool thousands = false;
            Edit buddy;

            public bool Horizontal
            {
                get { return horizontal; }
                set { horizontal = value; }
            }

            public int RangeMinimum
            {
                get { return rangeMin; }
                set { rangeMin = value; }
            }

            public int RangeMaximum
            {
                get { return rangeMax; }
                set { rangeMax = value; }
            }

            public bool Isolated
            {
                get { return isolated; }
                set { isolated = value; }
            }

            public bool Formatted
            {
                get { return formatted; }
                set { formatted = value; }
            }

            public Edit Buddy
            {
                get { return buddy; }
                set { buddy = value; }
            }

            public bool Left
            {
                get { return left; }
                set { left = value; }
            }

            public bool ThousandsSeperator
            {
                get { return thousands; }
                set { thousands = value; }
            }
        }

        public abstract class Picture : Control { }

        public abstract class Button : Control { }

        public abstract class Checkbox : Control
        {
            CheckState state = CheckState.Unchecked;

            public CheckState State
            {
                get { return state; }
                set { state = value; }
            }
        }

        public abstract class Radio : Checkbox
        {
            bool state;

            public bool Checked
            {
                get { return state; }
                set { state = value; }
            }
        }

        public abstract class DropDownList : Control
        {
            int choose = -1;
            bool lowercase = false;
            bool uppercase = false;
            bool sort = false;

            public int Choose
            {
                get { return choose; }
                set { choose = value; }
            }

            public bool Lowercase
            {
                get { return lowercase; }
                set { lowercase = value; }
            }

            public bool Uppercase
            {
                get { return uppercase; }
                set { uppercase = value; }
            }

            public bool Sort
            {
                get { return sort; }
                set { sort = value; }
            }
        }

        public abstract class ComboBox : Control
        {
            bool limit = false;
            bool simple = false;

            public bool Limit
            {
                get { return limit; }
                set { limit = value; }
            }

            public bool Simple
            {
                get { return simple; }
                set { simple = value; }
            }
        }

        public abstract class ListBox : Control
        {
            int choose = -1;
            bool sort = false;
            int tabstops = -1;
            bool multi = false;
            bool readOnly = false;
            bool exactheight = false;

            public int Choose
            {
                get { return choose; }
                set { choose = value; }
            }

            public bool Sort
            {
                get { return sort; }
                set { sort = value; }
            }

            public int TabStops
            {
                get { return tabstops; }
                set { tabstops = value; }
            }

            public bool MultiSelect
            {
                get { return multi; }
                set { multi = value; }
            }

            public bool ReadOnly
            {
                get { return readOnly; }
                set { readOnly = value; }
            }

            public bool ExactHeight
            {
                get { return exactheight; }
                set { exactheight = value; }
            }
        }

        public abstract class ListView : Control
        {
            bool checklist = false;
            int count;
            bool grid = false;
            bool header = true;
            bool movableColumns = true;
            bool clickToSelect = false;
            bool multi = true;
            bool sortableHeader = true;
            bool headerSort = true;
            bool sort = false;
            bool sortDesc = false;
            bool wantF2 = false;
            bool readOnly = false;

            public bool Checklist
            {
                get { return checklist; }
                set { checklist = value; }
            }

            public int Count
            {
                get { return count; }
                set { count = value; }
            }

            public bool Grid
            {
                get { return grid; }
                set { grid = value; }
            }

            public bool Header
            {
                get { return header; }
                set { header = value; }
            }

            public bool MovableColumns
            {
                get { return movableColumns; }
                set { movableColumns = value; }
            }

            public bool ClickToSelect
            {
                get { return clickToSelect; }
                set { clickToSelect = value; }
            }

            public bool MultiSelect
            {
                get { return multi; }
                set { multi = value; }
            }

            public bool SortableHeader
            {
                get { return sortableHeader; }
                set { sortableHeader = value; }
            }

            public bool HeaderSort
            {
                get { return headerSort; }
                set { headerSort = value; }
            }

            public bool Sort
            {
                get { return sort; }
                set { sort = value; }
            }

            public bool SortDesc
            {
                get { return sortDesc; }
                set { sortDesc = value; }
            }

            public bool WantF2
            {
                get { return wantF2; }
                set { wantF2 = value; }
            }

            public bool ReadOnly
            {
                get { return readOnly; }
                set { readOnly = value; }
            }
        }

        public abstract class TreeView : Control
        {
            bool buttons = true;
            bool checklist = false;
            ImageList imageList;
            bool lines = true;
            bool wantF2 = false;
            bool readOnly = false;

            public bool Buttons
            {
                get { return buttons; }
                set { buttons = value; }
            }

            public bool Checklist
            {
                get { return checklist; }
                set { checklist = value; }
            }

            public ImageList ImageList
            {
                get { return imageList; }
                set { imageList = value; }
            }

            public bool Lines
            {
                get { return lines; }
                set { lines = value; }
            }

            public bool WantF2
            {
                get { return wantF2; }
                set { wantF2 = value; }
            }

            public bool ReadOnly
            {
                get { return readOnly; }
                set { readOnly = value; }
            }
        }

        public abstract class Hotkey : Control
        {
            int limit = 0;

            public int Limit
            {
                get { return limit; }
                set { limit = value; }
            }
        }

        public abstract class DateTime : Control
        {
            string format;
            int choose = -1;
            System.DateTime rangeMin, rangeMax;
            bool updown = false;
            bool checklist = false;
            bool right = false;
            bool longdate = false;
            bool time = false;

            public string Format
            {
                get { return format; }
                set { format = value; }
            }

            public int Choose
            {
                get { return choose; }
                set { choose = value; }
            }

            public System.DateTime RangeMinimum
            {
                get { return rangeMin; }
                set { rangeMin = value; }
            }

            public System.DateTime RangeMaximum
            {
                get { return rangeMax; }
                set { rangeMax = value; }
            }

            public bool UpDown
            {
                get { return updown; }
                set { updown = value; }
            }

            public bool Checklist
            {
                get { return checklist; }
                set { checklist = value; }
            }

            public bool Right
            {
                get { return right; }
                set { right = value; }
            }

            public bool LongDate
            {
                get { return longdate; }
                set { longdate = value; }
            }

            public bool Time
            {
                get { return time; }
                set { time = value; }
            }
        }

        public abstract class MonthCal : Control
        {
            bool multi = false;
            System.DateTime rangeMin, rangeMax;
            bool displayWeek = false;
            bool highlightToday = true;
            bool displayToday = true;

            public bool MultiSelect
            {
                get { return multi; }
                set { multi = value; }
            }

            public System.DateTime RangeMinimum
            {
                get { return rangeMin; }
                set { rangeMin = value; }
            }

            public System.DateTime RangeMaximum
            {
                get { return rangeMax; }
                set { rangeMax = value; }
            }

            public bool DisplayWeek
            {
                get { return displayWeek; }
                set { displayWeek = value; }
            }

            public bool HightlightToday
            {
                get { return highlightToday; }
                set { highlightToday = value; }
            }

            public bool DisplayToday
            {
                get { return displayToday; }
                set { displayToday = value; }
            }
        }

        public abstract class Slider : Control
        {
            Control buddy1, buddy2;
            bool rounded = false;
            bool invert = false;
            bool left = false;
            int line;
            bool tick = true;
            int page;
            int rangeMin = 0, rangeMax = 100;
            bool thick = false;
            int tickInterval;
            int tooltip = -1;
            bool vertical;

            public Control Buddy1
            {
                get { return buddy1; }
                set { buddy1 = value; }
            }

            public Control Buddy2
            {
                get { return buddy2; }
                set { buddy2 = value; }
            }

            public bool Rounded
            {
                get { return rounded; }
                set { rounded = value; }
            }

            public bool Invert
            {
                get { return invert; }
                set { invert = value; }
            }

            public bool Left
            {
                get { return left; }
                set { left = value; }
            }

            public int Line
            {
                get { return line; }
                set { line = value; }
            }

            public bool Tick
            {
                get { return tick; }
                set { tick = value; }
            }

            public int Page
            {
                get { return page; }
                set { page = value; }
            }

            public int RangeMinimum
            {
                get { return rangeMin; }
                set { rangeMin = value; }
            }

            public int RangeMaximum
            {
                get { return rangeMax; }
                set { rangeMax = value; }
            }

            public bool Thick
            {
                get { return thick; }
                set { thick = value; }
            }

            public int TickInterval
            {
                get { return tickInterval; }
                set { tickInterval = value; }
            }

            public int ToolTip
            {
                get { return tooltip; }
                set { tooltip = value; }
            }

            public bool Vertical
            {
                get { return vertical; }
                set { vertical = value; }
            }
        }

        public abstract class Progress : Control
        {
            Color back;
            int rangeMin = 0, rangeMax = 100;
            bool smooth = true;
            bool vertical = false;

            public int RangeMinimum
            {
                get { return rangeMin; }
                set { rangeMin = value; }
            }

            public int RangeMaximum
            {
                get { return rangeMax; }
                set { rangeMax = value; }
            }

            public bool Smooth
            {
                get { return smooth; }
                set { smooth = value; }
            }

            public bool Vertical
            {
                get { return vertical; }
                set { vertical = value; }
            }

            public Color BackgroundColor
            {
                get { return back; }
                set { back = value; }
            }
        }

        public abstract class GroupBox : Text { }

        public abstract class Tab : Control
        {
            int choose = -1;
            bool buttons = false;
            int align = 0;

            public int Choose
            {
                get { return choose; }
                set { choose = value; }
            }

            public bool Buttons
            {
                get { return buttons; }
                set { buttons = value; }
            }

            public int Align
            {
                get { return align; }
                set { align = value; }
            }
        }

        public abstract class StatusBar : Text
        {
            public abstract void SetText(int part, string text);
            public abstract void SetParts(params int[] width);
            public abstract void SetIcon(int part, Image icon);
        }

        public abstract class WebBrowser : Text { }

        #endregion
    }
}
