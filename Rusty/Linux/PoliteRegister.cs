// PoliteRegister.cs created by tobias at 5:21 PM 2/10/2009
// This Hotkey/Hotsring-register uses a polite implementation: 
// it asks the X Server for events on all windows it can find
// This does not work for certain windows, such as that of Gnome DO and firefox

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using IronAHK.Rusty;

namespace IronAHK.Rusty.Linux
{    
    class PoliteRegister : PlatformInvokes,  IHotkeyRegister
    {
        public string Platform { get { return "Linux/X11"; } }
        
        // These are the events we subscribe to, in order to allow hotkey/hotstring support
        protected readonly static EventMasks SelectMask = EventMasks.KeyPress | EventMasks.KeyRelease | 
            EventMasks.Exposure | EventMasks.FocusChange | EventMasks.SubstructureNofity;
        
        protected List<KeyCombination> Hotkeys;
        protected Dictionary <string, string> Hotstrings;
        
        protected IntPtr Display;
        protected StringBuilder Buffer;
        
        // libX11 does not offer modifier information, so we'll keep track of the modifiers ourselves
        public KeyModifiers Modifiers;

        public PoliteRegister()
        {
            Buffer = new StringBuilder();
            Hotkeys = new List<KeyCombination>();
            Hotstrings = new Dictionary<string, string>();
            Modifiers = new KeyModifiers();
            
            Display = XOpenDisplay(IntPtr.Zero);
            RecurseTree(Display, XDefaultRootWindow(Display));
        }
        
        ~PoliteRegister()
        {
            XCloseDisplay(Display);
        }

        private void RecurseTree(IntPtr Display, int RootWindow)
        {
            int RootWindowRet, ParentWindow, NChildren;
            IntPtr ChildrenPtr;
            int[] Children;

            XQueryTree(Display, RootWindow, out RootWindowRet, out ParentWindow, out ChildrenPtr, out NChildren); 
                
            if(NChildren != 0)
            {
                Children = new int[NChildren];
                Marshal.Copy(ChildrenPtr, Children, 0, NChildren); // To prevent NullReferenceException
                
                XSelectInput(Display, RootWindow, SelectMask);
    
                for(int i = 0; i < NChildren; i++) // Subwindows shouldn't be forgotten, especially since everything is a subwindow of RootWindow
                {
                    if(Children[i] != 0) 
                    {
                        XSelectInput(Display, Children[i], SelectMask);
                        RecurseTree(Display, Children[i]);
                    }
                }                    
            }

        } 
        
        public bool RegisterHotkey(KeyCombination Keycode)
        {
            Hotkeys.Add(Keycode);

            return true;
        }

        public bool RegisterHotstring(string Sequence, string Replace)
        {
            Hotstrings.Add(Sequence, Replace);

            return true;
        }
        
        public KeyEvent NextEvent()
        {
            KeyEvent? Ret;
            while((Ret = FishEvent()) == null)
                System.Threading.Thread.Sleep(10);    // Be polite
            
            return Ret.Value;
        }
        
        private KeyEvent? FishEvent()
        {
            XEvent Event = new XEvent();
            XNextEvent(Display, ref Event);
            
            if(Event.type == XEventName.KeyPress || Event.type == XEventName.KeyRelease)
            {
                switch(Event.KeyEvent.keycode)
                {
                    case Keys.LeftControl: 
                        Modifiers.Control.Flip(KeyType.Left);
                        break;
                    case Keys.RightControl: 
                        Modifiers.Control.Flip(KeyType.Right);
                        break;
                    case Keys.LeftShift: 
                        Modifiers.Shift.Flip(KeyType.Left);
                        break;
                    case Keys.RightShift: 
                        Modifiers.Shift.Flip(KeyType.Right);
                        break;
                    case Keys.LeftSuper:
                        Modifiers.Super.Flip(KeyType.Left);
                        break;
                    case Keys.RightSuper:
                        Modifiers.Super.Flip(KeyType.Right);
                        break;
                    case Keys.RightAlt: 
                        Modifiers.Alt.Flip(KeyType.Right);
                        break;
                    case Keys.LeftAlt: 
                        Modifiers.Alt.Flip(KeyType.Left);
                        break;
                }
            }  
            
            if(Event.type == XEventName.KeyRelease)
            {                
                StringBuilder TempBuilder = new StringBuilder();
                Event.KeyEvent.state = 16; // Repair any shifting applied by control
                XLookupString(ref Event, TempBuilder, 1, IntPtr.Zero, IntPtr.Zero);
                
                if(Event.KeyEvent.keycode == Keys.BackSpace && !Modifiers.Control.Down && Buffer.Length != 0)
                {
                    Buffer.Remove(Buffer.Length-1, 1);
                }
                else if(((Event.KeyEvent.keycode >= Keys.LowerLetter && Event.KeyEvent.keycode <= Keys.UpperLetter) 
                         || Event.KeyEvent.keycode == Keys.SpaceBar) && (!Modifiers.Control.Down && !Modifiers.Alt.Down))
                {                    
                    Buffer.Append(TempBuilder);
                }
                else if(Event.KeyEvent.keycode != Keys.RightShift && Event.KeyEvent.keycode != Keys.LeftShift)
                {
                    Buffer.Remove(0, Buffer.Length);
                }
                
                string CurrentBuffer = Buffer.ToString();
                
                // Long hotstrings get prevelance, otherwise we'll get this:
                // Consider two hotstrings: apple -> blegh and pineapple -> yummy (in order of registration)
                // If we don't look for the ending, "pineapple" will become "pineblegh"
                // [And we don't like pinebleghs, do we?]
                                
                int Length = -1;
                string Hit = string.Empty;
                foreach(string Stroke in Hotstrings.Keys)
                {
                    if(CurrentBuffer.EndsWith(Stroke) && Length < Stroke.Length)
                    {
                        Length = Stroke.Length;
                        Hit = Stroke;
                    }
                }
                
                if(Length > -1 && Hit != string.Empty)
                {
                    // Delete the string to be replaced
                    for(int i = 0; i < Hit.Length; i++) 
                        TapKey(Display, (uint)Keys.BackSpace);
                    
                    for(int i = 0; i < Hotstrings[Hit].Length; i++)
                    {
                        string Character = Hotstrings[Hit].Substring(i, 1);
                        if(Character.ToUpperInvariant() == Character) TouchKey(Display, (uint) Keys.RightShift, true);
                        uint KeyCode = XKeysymToKeycode(Display, XStringToKeysym(Character == " " ? "space" : Character));
                        TapKey(Display, KeyCode);
                        if(Character.ToUpperInvariant() == Character) TouchKey(Display, (uint) Keys.RightShift, false);
                    }
                    
                    return new KeyEvent(KeyEventType.Hotstring, Hit);
                }
                else 
                {
                    foreach(KeyCombination Combo in Hotkeys)
                    {
                        if(Combo.Trigger.ToString() == TempBuilder.ToString() && Modifiers.Matches(Combo.Modifiers))
                        {
                            return new KeyEvent();
                        }
                    }
                }
                
                return null;
            }
            else if(Event.type == XEventName.FocusIn)
            {
                Buffer.Length = 0;
                
                return null;
            }
            else if(Event.type == XEventName.CreateNotify)
            {
                // X sometimes throws a BadWindow Error because windows are quickly deleted
                XErrorHandler Old = XSetErrorHandler(delegate { return 0; });
                RecurseTree(Display, Event.CreateWindowEvent.window); 
                XSetErrorHandler(Old);
                
                return null;
            }
            
            return null;
        }
        
        private static void TouchKey(IntPtr Display, uint KeyCode, bool IsPress)
        {
            XTestFakeKeyEvent(Display, KeyCode, IsPress, 0);
        }
        
        private static void TapKey(IntPtr Display, uint KeyCode)
        {
            TouchKey(Display, KeyCode, true);
            TouchKey(Display, KeyCode, false);
        }

    }
}
