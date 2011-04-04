using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using IronAHK.Rusty.Linux.X11.Events;

namespace IronAHK.Rusty.Linux.X11
{

    [StructLayout(LayoutKind.Explicit)]
    internal struct XEvent
    {
        [FieldOffset(0)]
        internal XEventName type;
        [FieldOffset(0)]
        internal XAnyEvent AnyEvent;
        [FieldOffset(0)]
        internal XKeyEvent KeyEvent;
        [FieldOffset(0)]
        internal XButtonEvent ButtonEvent;
        [FieldOffset(0)]
        internal XMotionEvent MotionEvent;
        [FieldOffset(0)]
        internal XCrossingEvent CrossingEvent;
        [FieldOffset(0)]
        internal XFocusChangeEvent FocusChangeEvent;
        [FieldOffset(0)]
        internal XExposeEvent ExposeEvent;
        [FieldOffset(0)]
        internal XGraphicsExposeEvent GraphicsExposeEvent;
        [FieldOffset(0)]
        internal XNoExposeEvent NoExposeEvent;
        [FieldOffset(0)]
        internal XVisibilityEvent VisibilityEvent;
        [FieldOffset(0)]
        internal XCreateWindowEvent CreateWindowEvent;
        [FieldOffset(0)]
        internal XDestroyWindowEvent DestroyWindowEvent;
        [FieldOffset(0)]
        internal XUnmapEvent UnmapEvent;
        [FieldOffset(0)]
        internal XMapEvent MapEvent;
        [FieldOffset(0)]
        internal XMapRequestEvent MapRequestEvent;
        [FieldOffset(0)]
        internal XReparentEvent ReparentEvent;
        [FieldOffset(0)]
        internal XConfigureEvent ConfigureEvent;
        [FieldOffset(0)]
        internal XGravityEvent GravityEvent;
        [FieldOffset(0)]
        internal XResizeRequestEvent ResizeRequestEvent;
        [FieldOffset(0)]
        internal XConfigureRequestEvent ConfigureRequestEvent;
        [FieldOffset(0)]
        internal XCirculateEvent CirculateEvent;
        [FieldOffset(0)]
        internal XCirculateRequestEvent CirculateRequestEvent;
        [FieldOffset(0)]
        internal XPropertyEvent PropertyEvent;
        [FieldOffset(0)]
        internal XSelectionClearEvent SelectionClearEvent;
        [FieldOffset(0)]
        internal XSelectionRequestEvent SelectionRequestEvent;
        [FieldOffset(0)]
        internal XSelectionEvent SelectionEvent;
        [FieldOffset(0)]
        internal XColormapEvent ColormapEvent;
        [FieldOffset(0)]
        internal XClientMessageEvent ClientMessageEvent;
        [FieldOffset(0)]
        internal XMappingEvent MappingEvent;
        [FieldOffset(0)]
        internal XErrorEvent ErrorEvent;
        [FieldOffset(0)]
        internal XKeymapEvent KeymapEvent;
        //[ FieldOffset(0) ] internal XTimerNotifyEvent TimerNotifyEvent;

        //[MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst=24)]
        //[ FieldOffset(0) ] internal int[] pad;
        [FieldOffset(0)]
        internal XEventPad Pad;
        public override string ToString() {
            return type.ToString();
        }
    }
}
