using System;
using System.Runtime.InteropServices;
using System.Text;
using IronAHK.Rusty.Linux.X11.Events;
using IronAHK.Rusty.Linux.X11.Types;

namespace IronAHK.Rusty.Linux.X11
{
        internal delegate int XErrorHandler(IntPtr DisplayHandle, ref XErrorEvent error_event);

        internal class Xlib
        {

            [DllImport("X11")]
            extern public static int XFree(IntPtr ptr);

            [DllImport("libX11")]
            public static extern IntPtr XOpenDisplay(IntPtr From);

            [DllImport("libX11")]
            public static extern void XCloseDisplay(IntPtr Dpy);

            [DllImport("libX11")]
            public static extern int XDefaultRootWindow(IntPtr Display);

            [DllImport("libX11")]
            public static extern IntPtr XSelectInput(IntPtr Display, int Window, EventMasks EventMask);

            /// <summary>
            /// The XQueryTree() function returns the root ID, the parent window ID,
            /// a pointer to the list of children windows (NULL when there are no children),
            /// and the number of children in the list for the specified window. 
            /// The children are listed in current stacking order, from bottommost (first) to topmost (last).
            /// XQueryTree() returns zero if it fails and nonzero if it succeeds. 
            /// To free a non-NULL children list when it is no longer needed, use XFree().
            /// </summary>
            /// <param name="display">Specifies the connection to the X server.</param>
            /// <param name="w">Specifies the window whose list of children, root, parent, and number of children you want to obtain.</param>
            /// <param name="root_return">Returns the root window.</param>
            /// <param name="parent_return">Returns the parent window.</param>
            /// <param name="children_return">Returns the list of children.</param>
            /// <param name="nchildren_return">Returns the number of children.</param>
            /// <returns></returns>
            [DllImport("libX11")]
            public static extern int XQueryTree(IntPtr display, int w, out int root_return, out int parent_return,
                                                 out IntPtr children_return, out int nchildren_return);

            [DllImport("libX11")]
            internal extern static int XGetWindowAttributes(IntPtr display, int window, ref XWindowAttributes attributes);

            [DllImport("libX11")]
            internal extern static int XGetInputFocus(IntPtr display, out int window, out int focusState);

            [DllImport("libX11")]
            public static extern void XNextEvent(IntPtr Display, ref XEvent Event);

            [DllImport("libX11")]
            public static extern int XLookupString(ref XEvent Key, StringBuilder Buffer, int Count, IntPtr KeySym, IntPtr Useless);

            [DllImport("libX11")]
            public static extern XErrorHandler XSetErrorHandler(XErrorHandler Handler);

            [DllImport("libX11")]
            public static extern uint XStringToKeysym(string Convert);

            [DllImport("X11")]
            extern public static int XStringListToTextProperty
                    (ref IntPtr argv, int argc, ref XTextProperty textprop);

            [DllImport("X11")]
            extern public static int XStringListToTextProperty
                    (IntPtr[] argv, int argc, ref XTextProperty textprop);


            [DllImport("libXtst.so.6")]
            public extern static void XTestFakeKeyEvent(IntPtr Display, uint KeyCode, bool isPress, ulong delay);

            [DllImport("libX11")]
            public extern static uint XKeysymToKeycode(IntPtr Display, uint Keysym);
			
			[DllImport("libX11")]
			public extern static int XGetTextProperty(IntPtr Display, int Window, ref XTextProperty Return, XAtom Property);
			
			[DllImport("libX11")]
			public extern static void XSetTextProperty(IntPtr Display, int Window, ref XTextProperty Prop, XAtom property);
        }
}
