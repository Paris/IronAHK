using System;
using System.Threading;
using System.Runtime.InteropServices;
using WF = System.Windows.Forms;
using System.Collections.Generic;
using System.Text;

namespace IronAHK.Rusty
{
    partial class Linux
    {
        public partial class KeyboardHook : Core.KeyboardHook
        {
            // These are the events we subscribe to, in order to allow hotkey/hotstring support
            protected readonly static EventMasks SelectMask = EventMasks.KeyPress | EventMasks.KeyRelease |
                EventMasks.Exposure | EventMasks.FocusChange | EventMasks.SubstructureNofity;
            
            IntPtr Display;
            StringBuilder Dummy = new StringBuilder(); // Somehow needed to get strings from native X11
            Thread Listener;
            
            Dictionary<Keys, WF.Keys> Mapping;
            
            protected override void RegisterHook()
            {
                if(Display == IntPtr.Zero)
                {
                    Display = X11.XOpenDisplay(IntPtr.Zero);
                    
                    // Kick off a thread listening to X events
                    Listener = new Thread(Listen);
                    Listener.Start();
                }
            }

            protected override void DeregisterHook()
            {
                Listener.Abort();
                X11.XCloseDisplay(Display);
            }
            
            void Listen()
            {
                SetupMapping();
                
                // Select all the windows already present
                RecurseTree(Display, X11.XDefaultRootWindow(Display));
                
                while (true)
                {
                    FishEvent();
                    System.Threading.Thread.Sleep(10); // Be polite
                }
            }
            
            private void FishEvent()
            {
                XEvent Event = new XEvent();
                X11.XNextEvent(Display, ref Event);

                if (Event.type == XEventName.KeyPress || Event.type == XEventName.KeyRelease)
                    KeyReceived(TranslateKey(Event), Event.type == XEventName.KeyPress);
                else if (Event.type == XEventName.CreateNotify)
                    UpdateTree(Event); // New window created, be sure to snoop it
            }            
            
            // In the X Window system, windows can have sub windows. This function crawls a
            // specific function, and then recurses on all child windows. It is called to 
            // select all initial windows, and later when a new window appears.
            void RecurseTree(IntPtr Display, int RootWindow)
            {
                int RootWindowRet, ParentWindow, NChildren;
                IntPtr ChildrenPtr;
                int[] Children;

                // Request all children of the given window, along with the parent
                X11.XQueryTree(Display, RootWindow, out RootWindowRet, out ParentWindow, out ChildrenPtr, out NChildren);

                if (NChildren != 0)
                {
                    // Fill the array with zeroes to prevent NullReferenceException from glue layer
                    Children = new int[NChildren];
                    Marshal.Copy(ChildrenPtr, Children, 0, NChildren); 

                    X11.XSelectInput(Display, RootWindow, SelectMask);

                    // Subwindows shouldn't be forgotten, especially since everything is a subwindow of RootWindow
                    for (int i = 0; i < NChildren; i++) 
                    {
                        if (Children[i] != 0)
                        {
                            X11.XSelectInput(Display, Children[i], SelectMask);
                            RecurseTree(Display, Children[i]);
                        }
                    }
                }
            }        
            
            void UpdateTree(XEvent Event)
            {
                // HACK: X sometimes throws a BadWindow Error because windows are quickly deleted
                // We set a placeholder errorhandler for the duration of the call ...
                XErrorHandler Old = X11.XSetErrorHandler(delegate { return 0; });
                RecurseTree(Display, Event.CreateWindowEvent.window);
                
                // ... and restore the old one when we're done
                X11.XSetErrorHandler(Old);
            }
            
            WF.Keys TranslateKey(XEvent Event)
            {
                if(Mapping.ContainsKey(Event.KeyEvent.keycode))
                    return Mapping[Event.KeyEvent.keycode];
                else return StringToWFKey(Event);
            }
            
            WF.Keys StringToWFKey(XEvent Event)
            {
                Dummy.Remove(0, Dummy.Length);
                Dummy.Append(" "); // HACK: Somehow necessary
                
                Event.KeyEvent.state = 16; // Repair any shifting applied by control
                
                if(X11.XLookupString(ref Event, Dummy, 10, IntPtr.Zero, IntPtr.Zero) != 0)
                {
                    string Lookup = Dummy.ToString();
                    
                    if(Dummy.Length == 1 && char.IsLetterOrDigit(Dummy[0]))
                        Lookup = Dummy[0].ToString().ToUpper();
                    
                    if(string.IsNullOrEmpty(Lookup.Trim())) return WF.Keys.None;
                    
                    return (WF.Keys) Enum.Parse(typeof(WF.Keys), Lookup);
                }
                else return WF.Keys.None;
            }
        }
    }
}