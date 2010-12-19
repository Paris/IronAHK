using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace IronAHK.Rusty.Linux
{
        internal class KeyboardHookLinux : Core.KeyboardHook
        {
            // These are the events we subscribe to, in order to allow hotkey/hotstring support
            protected readonly static LinuxAPI.EventMasks SelectMask = LinuxAPI.EventMasks.KeyPress | LinuxAPI.EventMasks.KeyRelease |
                LinuxAPI.EventMasks.Exposure | LinuxAPI.EventMasks.FocusChange | LinuxAPI.EventMasks.SubstructureNofity;
            
            IntPtr Display;
            StringBuilder Dummy = new StringBuilder(); // Somehow needed to get strings from native X11
            Thread Listener;

            Dictionary<char, CachedKey> Cache;
            Dictionary<LinuxAPI.Keys, System.Windows.Forms.Keys> Mapping;
            
            protected override void RegisterHook()
            {
                if(Display == IntPtr.Zero)
                {
                    Display = LinuxAPI.X11.XOpenDisplay(IntPtr.Zero);
                    
                    // Kick off a thread listening to X events
                    Listener = new Thread(Listen);
                    Listener.Start();
                }
            }

            protected override void DeregisterHook()
            {
                Listener.Abort();
                LinuxAPI.X11.XCloseDisplay(Display);
            }
            
            void Listen()
            {
                SetupMapping();
                
                // Select all the windows already present
                RecurseTree(Display, LinuxAPI.X11.XDefaultRootWindow(Display));
                
                while (true)
                {
                    FishEvent();
                    Thread.Sleep(10); // Be polite
                }
            }
            
            private void FishEvent()
            {
                var Event = new LinuxAPI.XEvent();
                LinuxAPI.X11.XNextEvent(Display, ref Event);

                if(Event.type == LinuxAPI.XEventName.KeyPress || Event.type == LinuxAPI.XEventName.KeyRelease)
                    KeyReceived(TranslateKey(Event), Event.type == LinuxAPI.XEventName.KeyPress);
                else if(Event.type == LinuxAPI.XEventName.CreateNotify)
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
                LinuxAPI.X11.XQueryTree(Display, RootWindow, out RootWindowRet, out ParentWindow, out ChildrenPtr, out NChildren);

                if (NChildren != 0)
                {
                    // Fill the array with zeroes to prevent NullReferenceException from glue layer
                    Children = new int[NChildren];
                    Marshal.Copy(ChildrenPtr, Children, 0, NChildren);

                    LinuxAPI.X11.XSelectInput(Display, RootWindow, SelectMask);

                    // Subwindows shouldn't be forgotten, especially since everything is a subwindow of RootWindow
                    for (int i = 0; i < NChildren; i++) 
                    {
                        if (Children[i] != 0)
                        {
                            LinuxAPI.X11.XSelectInput(Display, Children[i], SelectMask);
                            RecurseTree(Display, Children[i]);
                        }
                    }
                }
            }

            void UpdateTree(LinuxAPI.XEvent Event)
            {
                // HACK: X sometimes throws a BadWindow Error because windows are quickly deleted
                // We set a placeholder errorhandler for the duration of the call ...
                LinuxAPI.XErrorHandler Old = LinuxAPI.X11.XSetErrorHandler(delegate { return 0; });
                RecurseTree(Display, Event.CreateWindowEvent.window);
                
                // ... and restore the old one when we're done
                LinuxAPI.X11.XSetErrorHandler(Old);
            }

            System.Windows.Forms.Keys TranslateKey(LinuxAPI.XEvent Event)
            {
                if(Mapping.ContainsKey(Event.KeyEvent.keycode))
                    return Mapping[Event.KeyEvent.keycode];
                else return StringToWFKey(Event);
            }

            System.Windows.Forms.Keys StringToWFKey(LinuxAPI.XEvent Event)
            {
                Dummy.Remove(0, Dummy.Length);
                Dummy.Append(" "); // HACK: Somehow necessary
                
                Event.KeyEvent.state = 16; // Repair any shifting applied by control

                if(LinuxAPI.X11.XLookupString(ref Event, Dummy, 10, IntPtr.Zero, IntPtr.Zero) != 0)
                {
                    string Lookup = Dummy.ToString();
                    
                    if(Dummy.Length == 1 && char.IsLetterOrDigit(Dummy[0]))
                        Lookup = Dummy[0].ToString().ToUpper();
                    
                    if(string.IsNullOrEmpty(Lookup.Trim())) return System.Windows.Forms.Keys.None;
                    
                    try 
                    {
                        return (System.Windows.Forms.Keys) Enum.Parse(typeof(System.Windows.Forms.Keys), Lookup);
                    }
                    catch(ArgumentException)
                    {
                        // TODO
                        Console.Error.WriteLine("Warning, could not look up key: "+Lookup);
                        return System.Windows.Forms.Keys.None;
                    }
                }
                else return System.Windows.Forms.Keys.None;
            }
             
            #region Hotstrings
            
            // Simulate a number of backspaces
            protected override void Backspace (int length)
            {
                for(int i = 0; i < length; i++)
                {
                    LinuxAPI.X11.XTestFakeKeyEvent(Display, (uint)LinuxAPI.Keys.BackSpace, true, 0);
                    LinuxAPI.X11.XTestFakeKeyEvent(Display, (uint)LinuxAPI.Keys.BackSpace, false, 0);
                }                                 
            }
            
            protected internal override void Send(string Sequence)
            {
                foreach(var C in Sequence)
                {
                    CachedKey Key = LookupKeycode(C);
                    
                    // If it is an upper case character, hold the shift key...
                    if(char.IsUpper(C) || Key.Shift)
                        LinuxAPI.X11.XTestFakeKeyEvent(Display, (uint)LinuxAPI.Keys.LeftShift, true, 0);
                    
                    // Make sure the key is up before we press it again.
                    // If X thinks this key is still down, nothing will happen if we press it.
                    // Likewise, if X thinks that the key is up, this will do no harm.
                    LinuxAPI.X11.XTestFakeKeyEvent(Display, Key.Sym, false, 0); 
                    
                    // Fake a key event. Note that some programs filter this kind of event.
                    LinuxAPI.X11.XTestFakeKeyEvent(Display, Key.Sym, true, 0);
                    LinuxAPI.X11.XTestFakeKeyEvent(Display, Key.Sym, false, 0);
                    
                    // ...and release it later on
                    if(char.IsUpper(C) || Key.Shift)
                        LinuxAPI.X11.XTestFakeKeyEvent(Display, (uint)LinuxAPI.Keys.LeftShift, false, 0);
                }
            }

            protected internal override void Send(System.Windows.Forms.Keys key)
            {
                var vk = (uint)key;
                LinuxAPI.X11.XTestFakeKeyEvent(Display, vk, true, 0);
                LinuxAPI.X11.XTestFakeKeyEvent(Display, vk, false, 0);
            }
            
            CachedKey LookupKeycode(char Code)
            {
                // If we have a cache value, return that
                if(Cache.ContainsKey(Code))
                    return Cache[Code];
                
                // First look up the KeySym (XK_* in X11/keysymdef.h)
                uint KeySym = LinuxAPI.X11.XStringToKeysym(Code.ToString());
                // Then look up the appropriate KeyCode
                uint KeyCode = LinuxAPI.X11.XKeysymToKeycode(Display, KeySym);
                
                // Cache for later use
                var Ret = new CachedKey(KeyCode, false);
                Cache.Add(Code, Ret);
                          
                return Ret;
            }            
            
            #endregion

            private struct CachedKey
            {
                public uint Sym;
                public bool Shift;

                public CachedKey(uint Sym, bool Shift) {
                    this.Sym = Sym;
                    this.Shift = Shift;
                }

                public CachedKey(LinuxAPI.Keys Sym, bool Shift)
                    : this((uint)Sym, Shift) {
                }
            }

            public void SetupMapping() {
                Mapping = new Dictionary<LinuxAPI.Keys, System.Windows.Forms.Keys>();
                Cache = new Dictionary<char, CachedKey>();

                Mapping.Add(LinuxAPI.Keys.LeftAlt, System.Windows.Forms.Keys.LMenu);
                Mapping.Add(LinuxAPI.Keys.RightAlt, System.Windows.Forms.Keys.RMenu);

                Mapping.Add(LinuxAPI.Keys.LeftControl, System.Windows.Forms.Keys.LControlKey);
                Mapping.Add(LinuxAPI.Keys.RightControl, System.Windows.Forms.Keys.RControlKey);

                Mapping.Add(LinuxAPI.Keys.LeftSuper, System.Windows.Forms.Keys.LWin);
                Mapping.Add(LinuxAPI.Keys.RightSuper, System.Windows.Forms.Keys.RWin);

                Mapping.Add(LinuxAPI.Keys.LeftShift, System.Windows.Forms.Keys.LShiftKey);
                Mapping.Add(LinuxAPI.Keys.RightShift, System.Windows.Forms.Keys.RShiftKey);

                Mapping.Add(LinuxAPI.Keys.F1, System.Windows.Forms.Keys.F1);
                Mapping.Add(LinuxAPI.Keys.F2, System.Windows.Forms.Keys.F2);
                Mapping.Add(LinuxAPI.Keys.F3, System.Windows.Forms.Keys.F3);
                Mapping.Add(LinuxAPI.Keys.F4, System.Windows.Forms.Keys.F4);
                Mapping.Add(LinuxAPI.Keys.F5, System.Windows.Forms.Keys.F5);
                Mapping.Add(LinuxAPI.Keys.F6, System.Windows.Forms.Keys.F6);
                Mapping.Add(LinuxAPI.Keys.F7, System.Windows.Forms.Keys.F7);
                Mapping.Add(LinuxAPI.Keys.F8, System.Windows.Forms.Keys.F8);
                Mapping.Add(LinuxAPI.Keys.F9, System.Windows.Forms.Keys.F9);
                Mapping.Add(LinuxAPI.Keys.F10, System.Windows.Forms.Keys.F10);
                // Missing: F11 (Caught by WM)
                Mapping.Add(LinuxAPI.Keys.F12, System.Windows.Forms.Keys.F12);

                Mapping.Add(LinuxAPI.Keys.Escape, System.Windows.Forms.Keys.Escape);
                Mapping.Add(LinuxAPI.Keys.Tab, System.Windows.Forms.Keys.Tab);
                Mapping.Add(LinuxAPI.Keys.CapsLock, System.Windows.Forms.Keys.CapsLock);
                Mapping.Add(LinuxAPI.Keys.Tilde, System.Windows.Forms.Keys.Oemtilde);
                Mapping.Add(LinuxAPI.Keys.Backslash, System.Windows.Forms.Keys.OemBackslash);
                Mapping.Add(LinuxAPI.Keys.BackSpace, System.Windows.Forms.Keys.Back);

                Mapping.Add(LinuxAPI.Keys.ScrollLock, System.Windows.Forms.Keys.Scroll);
                Mapping.Add(LinuxAPI.Keys.Pause, System.Windows.Forms.Keys.Pause);
                Mapping.Add(LinuxAPI.Keys.Insert, System.Windows.Forms.Keys.Insert);
                Mapping.Add(LinuxAPI.Keys.Delete, System.Windows.Forms.Keys.Delete);
                Mapping.Add(LinuxAPI.Keys.Home, System.Windows.Forms.Keys.Home);
                Mapping.Add(LinuxAPI.Keys.End, System.Windows.Forms.Keys.End);
                Mapping.Add(LinuxAPI.Keys.PageUp, System.Windows.Forms.Keys.PageUp);
                Mapping.Add(LinuxAPI.Keys.PageDown, System.Windows.Forms.Keys.PageDown);
                Mapping.Add(LinuxAPI.Keys.NumLock, System.Windows.Forms.Keys.NumLock);

                Mapping.Add(LinuxAPI.Keys.SpaceBar, System.Windows.Forms.Keys.Space);
                Mapping.Add(LinuxAPI.Keys.Return, System.Windows.Forms.Keys.Return);

                Mapping.Add(LinuxAPI.Keys.Slash, System.Windows.Forms.Keys.OemQuestion);
                Mapping.Add(LinuxAPI.Keys.Dot, System.Windows.Forms.Keys.OemPeriod);
                Mapping.Add(LinuxAPI.Keys.Comma, System.Windows.Forms.Keys.Oemcomma);
                Mapping.Add(LinuxAPI.Keys.Semicolon, System.Windows.Forms.Keys.OemSemicolon);
                Mapping.Add(LinuxAPI.Keys.OpenSquareBracket, System.Windows.Forms.Keys.OemOpenBrackets);
                Mapping.Add(LinuxAPI.Keys.CloseSquareBracket, System.Windows.Forms.Keys.OemCloseBrackets);

                Mapping.Add(LinuxAPI.Keys.Up, System.Windows.Forms.Keys.Up);
                Mapping.Add(LinuxAPI.Keys.Down, System.Windows.Forms.Keys.Down);
                Mapping.Add(LinuxAPI.Keys.Right, System.Windows.Forms.Keys.Right);
                Mapping.Add(LinuxAPI.Keys.Left, System.Windows.Forms.Keys.Left);

                // Not sure about these ....
                Mapping.Add(LinuxAPI.Keys.Dash, System.Windows.Forms.Keys.OemMinus);
                Mapping.Add(LinuxAPI.Keys.Equals, System.Windows.Forms.Keys.Oemplus);

                // No windows equivalent?
                Mapping.Add(LinuxAPI.Keys.NumpadSlash, System.Windows.Forms.Keys.None);
                Mapping.Add(LinuxAPI.Keys.NumpadAsterisk, System.Windows.Forms.Keys.None);
                Mapping.Add(LinuxAPI.Keys.NumpadDot, System.Windows.Forms.Keys.None);
                Mapping.Add(LinuxAPI.Keys.NumpadEnter, System.Windows.Forms.Keys.None);
                Mapping.Add(LinuxAPI.Keys.NumpadPlus, System.Windows.Forms.Keys.None);
                Mapping.Add(LinuxAPI.Keys.NumpadMinus, System.Windows.Forms.Keys.None);

                #region Cache
                // Add keys to the cache that can not be looked up with XLookupKeysym

                // HACK: I'm not sure these will work on other keyboard layouts.
                Cache.Add('(', new CachedKey(LinuxAPI.Keys.OpenParens, true));
                Cache.Add(')', new CachedKey(LinuxAPI.Keys.CloseParens, true));
                Cache.Add('[', new CachedKey(LinuxAPI.Keys.OpenSquareBracket, true));
                Cache.Add(']', new CachedKey(LinuxAPI.Keys.CloseSquareBracket, true));
                Cache.Add('=', new CachedKey(LinuxAPI.Keys.Equals, true));
                Cache.Add('-', new CachedKey(LinuxAPI.Keys.Dash, true));
                Cache.Add('!', new CachedKey(LinuxAPI.Keys.ExMark, true));
                Cache.Add('@', new CachedKey(LinuxAPI.Keys.At, true));
                Cache.Add('#', new CachedKey(LinuxAPI.Keys.Hash, true));
                Cache.Add('$', new CachedKey(LinuxAPI.Keys.Dollar, true));
                Cache.Add('%', new CachedKey(LinuxAPI.Keys.Percent, true));
                Cache.Add('^', new CachedKey(LinuxAPI.Keys.Circumflex, true));
                Cache.Add('&', new CachedKey(LinuxAPI.Keys.Ampersand, true));
                Cache.Add('*', new CachedKey(LinuxAPI.Keys.Asterisk, true));
                Cache.Add(' ', new CachedKey(LinuxAPI.Keys.SpaceBar, false));

                #endregion
            }
        }
}