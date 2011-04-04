#pragma warning disable 649

using System;
using System.Runtime.InteropServices;
using System.Timers;
using IronAHK.Rusty.Linux.X11.Events;

// X11 Version
namespace IronAHK.Rusty.Linux
{
    partial class LinuxAPI
    {
        //
        // In the structures below, fields of type long are mapped to IntPtr.
        // This will work on all platforms where sizeof(long)==sizeof(void*), which
        // is almost all platforms except WIN64.
        //


        internal enum CreateWindowArgs
        {
            CopyFromParent = 0,
            ParentRelative = 1,
            InputOutput = 1,
            InputOnly = 2
        }

        



        internal enum PropertyMode
        {
            Replace = 0,
            Prepend = 1,
            Append = 2
        }

        [Flags]
        internal enum GCFunction
        {
            GCFunction = 1 << 0,
            GCPlaneMask = 1 << 1,
            GCForeground = 1 << 2,
            GCBackground = 1 << 3,
            GCLineWidth = 1 << 4,
            GCLineStyle = 1 << 5,
            GCCapStyle = 1 << 6,
            GCJoinStyle = 1 << 7,
            GCFillStyle = 1 << 8,
            GCFillRule = 1 << 9,
            GCTile = 1 << 10,
            GCStipple = 1 << 11,
            GCTileStipXOrigin = 1 << 12,
            GCTileStipYOrigin = 1 << 13,
            GCFont = 1 << 14,
            GCSubwindowMode = 1 << 15,
            GCGraphicsExposures = 1 << 16,
            GCClipXOrigin = 1 << 17,
            GCClipYOrigin = 1 << 18,
            GCClipMask = 1 << 19,
            GCDashOffset = 1 << 20,
            GCDashList = 1 << 21,
            GCArcMode = 1 << 22
        }

        internal enum GCJoinStyle
        {
            JoinMiter = 0,
            JoinRound = 1,
            JoinBevel = 2
        }

        internal enum GCLineStyle
        {
            LineSolid = 0,
            LineOnOffDash = 1,
            LineDoubleDash = 2
        }

        internal enum GCCapStyle
        {
            CapNotLast = 0,
            CapButt = 1,
            CapRound = 2,
            CapProjecting = 3
        }

        internal enum GCFillStyle
        {
            FillSolid = 0,
            FillTiled = 1,
            FillStippled = 2,
            FillOpaqueStppled = 3
        }

        internal enum GCFillRule
        {
            EvenOddRule = 0,
            WindingRule = 1
        }

        internal enum GCArcMode
        {
            ArcChord = 0,
            ArcPieSlice = 1
        }

        internal enum GCSubwindowMode
        {
            ClipByChildren = 0,
            IncludeInferiors = 1
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct XGCValues
        {
            internal GXFunction function;
            internal IntPtr plane_mask;
            internal IntPtr foreground;
            internal IntPtr background;
            internal int line_width;
            internal GCLineStyle line_style;
            internal GCCapStyle cap_style;
            internal GCJoinStyle join_style;
            internal GCFillStyle fill_style;
            internal GCFillRule fill_rule;
            internal GCArcMode arc_mode;
            internal IntPtr tile;
            internal IntPtr stipple;
            internal int ts_x_origin;
            internal int ts_y_origin;
            internal IntPtr font;
            internal GCSubwindowMode subwindow_mode;
            internal bool graphics_exposures;
            internal int clip_x_origin;
            internal int clib_y_origin;
            internal IntPtr clip_mask;
            internal int dash_offset;
            internal byte dashes;
        }

        internal enum GXFunction
        {
            GXclear = 0x0,        /* 0 */
            GXand = 0x1,        /* src AND dst */
            GXandReverse = 0x2,        /* src AND NOT dst */
            GXcopy = 0x3,        /* src */
            GXandInverted = 0x4,        /* NOT src AND dst */
            GXnoop = 0x5,        /* dst */
            GXxor = 0x6,        /* src XOR dst */
            GXor = 0x7,        /* src OR dst */
            GXnor = 0x8,        /* NOT src AND NOT dst */
            GXequiv = 0x9,        /* NOT src XOR dst */
            GXinvert = 0xa,        /* NOT dst */
            GXorReverse = 0xb,        /* src OR NOT dst */
            GXcopyInverted = 0xc,        /* NOT src */
            GXorInverted = 0xd,        /* NOT src OR dst */
            GXnand = 0xe,        /* NOT src OR NOT dst */
            GXset = 0xf        /* 1 */
        }

        internal enum NetWindowManagerState
        {
            Remove = 0,
            Add = 1,
            Toggle = 2
        }

        internal enum RevertTo
        {
            None = 0,
            PointerRoot = 1,
            Parent = 2
        }



        internal enum SystrayRequest
        {
            SYSTEM_TRAY_REQUEST_DOCK = 0,
            SYSTEM_TRAY_BEGIN_MESSAGE = 1,
            SYSTEM_TRAY_CANCEL_MESSAGE = 2
        }

        [Flags]
        internal enum XSizeHintsFlags
        {
            USPosition = (1 << 0),
            USSize = (1 << 1),
            PPosition = (1 << 2),
            PSize = (1 << 3),
            PMinSize = (1 << 4),
            PMaxSize = (1 << 5),
            PResizeInc = (1 << 6),
            PAspect = (1 << 7),
            PAllHints = (PPosition | PSize | PMinSize | PMaxSize | PResizeInc | PAspect),
            PBaseSize = (1 << 8),
            PWinGravity = (1 << 9),
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct XSizeHints
        {
            internal IntPtr flags;
            internal int x;
            internal int y;
            internal int width;
            internal int height;
            internal int min_width;
            internal int min_height;
            internal int max_width;
            internal int max_height;
            internal int width_inc;
            internal int height_inc;
            internal int min_aspect_x;
            internal int min_aspect_y;
            internal int max_aspect_x;
            internal int max_aspect_y;
            internal int base_width;
            internal int base_height;
            internal int win_gravity;
        }

        [Flags]
        internal enum XWMHintsFlags
        {
            InputHint = (1 << 0),
            StateHint = (1 << 1),
            IconPixmapHint = (1 << 2),
            IconWindowHint = (1 << 3),
            IconPositionHint = (1 << 4),
            IconMaskHint = (1 << 5),
            WindowGroupHint = (1 << 6),
            AllHints = (InputHint | StateHint | IconPixmapHint | IconWindowHint | IconPositionHint | IconMaskHint | WindowGroupHint)
        }

        internal enum XInitialState
        {
            DontCareState = 0,
            NormalState = 1,
            ZoomState = 2,
            IconicState = 3,
            InactiveState = 4
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct XWMHints
        {
            internal IntPtr flags;
            internal bool input;
            internal XInitialState initial_state;
            internal IntPtr icon_pixmap;
            internal IntPtr icon_window;
            internal int icon_x;
            internal int icon_y;
            internal IntPtr icon_mask;
            internal IntPtr window_group;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct XIconSize
        {
            internal int min_width;
            internal int min_height;
            internal int max_width;
            internal int max_height;
            internal int width_inc;
            internal int height_inc;
        }

        internal enum NA
        {
            WM_PROTOCOLS,
            WM_DELETE_WINDOW,
            WM_TAKE_FOCUS,

            _NET_SUPPORTED,
            _NET_CLIENT_LIST,
            _NET_NUMBER_OF_DESKTOPS,
            _NET_DESKTOP_GEOMETRY,
            _NET_DESKTOP_VIEWPORT,
            _NET_CURRENT_DESKTOP,
            _NET_DESKTOP_NAMES,
            _NET_ACTIVE_WINDOW,
            _NET_WORKAREA,
            _NET_SUPPORTING_WM_CHECK,
            _NET_VIRTUAL_ROOTS,
            _NET_DESKTOP_LAYOUT,
            _NET_SHOWING_DESKTOP,

            _NET_CLOSE_WINDOW,
            _NET_MOVERESIZE_WINDOW,
            _NET_WM_MOVERESIZE,
            _NET_RESTACK_WINDOW,
            _NET_REQUEST_FRAME_EXTENTS,

            _NET_WM_NAME,
            _NET_WM_VISIBLE_NAME,
            _NET_WM_ICON_NAME,
            _NET_WM_VISIBLE_ICON_NAME,
            _NET_WM_DESKTOP,
            _NET_WM_WINDOW_TYPE,
            _NET_WM_STATE,
            _NET_WM_ALLOWED_ACTIONS,
            _NET_WM_STRUT,
            _NET_WM_STRUT_PARTIAL,
            _NET_WM_ICON_GEOMETRY,
            _NET_WM_ICON,
            _NET_WM_PID,
            _NET_WM_HANDLED_ICONS,
            _NET_WM_USER_TIME,
            _NET_FRAME_EXTENTS,

            _NET_WM_PING,
            _NET_WM_SYNC_REQUEST,

            _NET_SYSTEM_TRAY_S,
            _NET_SYSTEM_TRAY_ORIENTATION,
            _NET_SYSTEM_TRAY_OPCODE,

            _NET_WM_STATE_MAXIMIZED_HORZ,
            _NET_WM_STATE_MAXIMIZED_VERT,

            _XEMBED,
            _XEMBED_INFO,

            _MOTIF_WM_HINTS,

            _NET_WM_STATE_NO_TASKBAR,
            _NET_WM_STATE_ABOVE,
            _NET_WM_STATE_MODAL,
            _NET_WM_STATE_HIDDEN,
            _NET_WM_CONTEXT_HELP,

            _NET_WM_WINDOW_OPACITY,

            _NET_WM_WINDOW_TYPE_DESKTOP,
            _NET_WM_WINDOW_TYPE_DOCK,
            _NET_WM_WINDOW_TYPE_TOOLBAR,
            _NET_WM_WINDOW_TYPE_MENU,
            _NET_WM_WINDOW_TYPE_UTILITY,
            _NET_WM_WINDOW_TYPE_SPLASH,
            _NET_WM_WINDOW_TYPE_DIALOG,
            _NET_WM_WINDOW_TYPE_NORMAL,

            CLIPBOARD,
            DIB,
            OEMTEXT,
            UNICODETEXT,
            TARGETS,

            LAST_NET_ATOM
        }

        internal struct CaretStruct
        {
            internal Timer Timert;                // Blink interval
            internal IntPtr Hwnd;                // Window owning the caret
            internal IntPtr Window;                // Actual X11 handle of the window
            internal int X;                // X position of the caret
            internal int Y;                // Y position of the caret
            internal int Width;                // Width of the caret; if no image used
            internal int Height;                // Height of the caret, if no image used
            internal bool Visible;            // Is caret visible?
            internal bool On;                // Caret blink display state: On/Off
            internal IntPtr gc;                // Graphics context
            internal bool Paused;                // Don't update right now
        }

        


        [Flags]
        internal enum XIMProperties
        {
            XIMPreeditArea = 0x0001,
            XIMPreeditCallbacks = 0x0002,
            XIMPreeditPosition = 0x0004,
            XIMPreeditNothing = 0x0008,
            XIMPreeditNone = 0x0010,
            XIMStatusArea = 0x0100,
            XIMStatusCallbacks = 0x0200,
            XIMStatusNothing = 0x0400,
            XIMStatusNone = 0x0800,
        }

        [Flags]
        internal enum WindowType
        {
            Client = 1,
            Whole = 2,
            Both = 3
        }

    }
}
