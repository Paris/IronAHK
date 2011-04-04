using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace IronAHK.Rusty.Linux.X11.Types
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct XSetWindowAttributes
    {
        internal IntPtr background_pixmap;
        internal IntPtr background_pixel;
        internal IntPtr border_pixmap;
        internal IntPtr border_pixel;
        internal Gravity bit_gravity;
        internal Gravity win_gravity;
        internal int backing_store;
        internal IntPtr backing_planes;
        internal IntPtr backing_pixel;
        internal bool save_under;
        internal IntPtr event_mask;
        internal IntPtr do_not_propagate_mask;
        internal bool override_redirect;
        internal IntPtr colormap;
        internal IntPtr cursor;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct XWindowAttributes
    {
        internal int x;
        internal int y;
        internal int width;
        internal int height;
        internal int border_width;
        internal int depth;
        internal IntPtr visual;
        internal IntPtr root;
        internal int c_class;
        internal Gravity bit_gravity;
        internal Gravity win_gravity;
        internal int backing_store;
        internal IntPtr backing_planes;
        internal IntPtr backing_pixel;
        internal bool save_under;
        internal IntPtr colormap;
        internal bool map_installed;
        internal MapState map_state;
        internal IntPtr all_event_masks;
        internal IntPtr your_event_mask;
        internal IntPtr do_not_propagate_mask;
        internal bool override_direct;
        internal IntPtr screen;
    }


}
