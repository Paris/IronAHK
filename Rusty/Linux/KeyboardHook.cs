using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using IronAHK.Rusty.Linux.X11;
using IronAHK.Rusty.Linux.X11.Events;

namespace IronAHK.Rusty.Linux
{
        internal class KeyboardHook : Common.Keyboard.KeyboardHook
        {
            StringBuilder Dummy = new StringBuilder(); // Somehow needed to get strings from native X11

            Dictionary<char, CachedKey> Cache;
            Dictionary<XKeys, System.Windows.Forms.Keys> Mapping;
			XConnectionSingleton XConn;
		
            protected override void RegisterHook()
            {
	            SetupMapping();
				XConn = XConnectionSingleton.GetInstance();
				XConn.OnEvent += HandleXEvent;
            }

            void HandleXEvent (XEvent Event)
            {
            	if(Event.type == XEventName.KeyPress || Event.type == XEventName.KeyRelease)
                	KeyReceived(TranslateKey(Event), Event.type == XEventName.KeyPress);
            }

            protected override void DeregisterHook()
            {
                // TODO disposal
            }

            System.Windows.Forms.Keys TranslateKey(XEvent Event)
            {
                if(Mapping.ContainsKey(Event.KeyEvent.keycode))
                    return Mapping[Event.KeyEvent.keycode];
                else return StringToWFKey(Event);
            }

            System.Windows.Forms.Keys StringToWFKey(XEvent Event)
            {
                Dummy.Remove(0, Dummy.Length);
                Dummy.Append(" "); // HACK: Somehow necessary
                
                Event.KeyEvent.state = 16; // Repair any shifting applied by control

                if(Xlib.XLookupString(ref Event, Dummy, 10, IntPtr.Zero, IntPtr.Zero) != 0)
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
		            Xlib.XTestFakeKeyEvent(XConn.Handle, (uint)XKeys.BackSpace, true, 0);
		            Xlib.XTestFakeKeyEvent(XConn.Handle, (uint)XKeys.BackSpace, false, 0);
		        }                                 
		    }
		    
		    protected internal override void Send(string Sequence)
		    {
		        foreach(var C in Sequence)
		        {
		            CachedKey Key = LookupKeycode(C);
		            
		            // If it is an upper case character, hold the shift key...
		            if(char.IsUpper(C) || Key.Shift)
		                Xlib.XTestFakeKeyEvent(XConn.Handle, (uint)XKeys.LeftShift, true, 0);
		            
		            // Make sure the key is up before we press it again.
		            // If X thinks this key is still down, nothing will happen if we press it.
		            // Likewise, if X thinks that the key is up, this will do no harm.
		            Xlib.XTestFakeKeyEvent(XConn.Handle, Key.Sym, false, 0); 
		            
		            // Fake a key event. Note that some programs filter this kind of event.
		            Xlib.XTestFakeKeyEvent(XConn.Handle, Key.Sym, true, 0);
		            Xlib.XTestFakeKeyEvent(XConn.Handle, Key.Sym, false, 0);
		            
		            // ...and release it later on
		            if(char.IsUpper(C) || Key.Shift)
		                Xlib.XTestFakeKeyEvent(XConn.Handle, (uint)XKeys.LeftShift, false, 0);
		        }
		    }
		
		    protected internal override void Send(System.Windows.Forms.Keys key)
		    {
		        var vk = (uint)key;
		        Xlib.XTestFakeKeyEvent(XConn.Handle, vk, true, 0);
		        Xlib.XTestFakeKeyEvent(XConn.Handle, vk, false, 0);
		    }		
            
            CachedKey LookupKeycode(char Code)
            {
                // If we have a cache value, return that
                if(Cache.ContainsKey(Code))
                    return Cache[Code];
                
                // First look up the KeySym (XK_* in X11/keysymdef.h)
                uint KeySym = Xlib.XStringToKeysym(Code.ToString());
                // Then look up the appropriate KeyCode
                uint KeyCode = Xlib.XKeysymToKeycode(XConn.Handle, KeySym);
                
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

                public CachedKey(XKeys Sym, bool Shift)
                    : this((uint)Sym, Shift) {
                }
            }

            public void SetupMapping() {
                Mapping = new Dictionary<XKeys, System.Windows.Forms.Keys>();
                Cache = new Dictionary<char, CachedKey>();

                Mapping.Add(XKeys.LeftAlt, System.Windows.Forms.Keys.LMenu);
                Mapping.Add(XKeys.RightAlt, System.Windows.Forms.Keys.RMenu);

                Mapping.Add(XKeys.LeftControl, System.Windows.Forms.Keys.LControlKey);
                Mapping.Add(XKeys.RightControl, System.Windows.Forms.Keys.RControlKey);

                Mapping.Add(XKeys.LeftSuper, System.Windows.Forms.Keys.LWin);
                Mapping.Add(XKeys.RightSuper, System.Windows.Forms.Keys.RWin);

                Mapping.Add(XKeys.LeftShift, System.Windows.Forms.Keys.LShiftKey);
                Mapping.Add(XKeys.RightShift, System.Windows.Forms.Keys.RShiftKey);

                Mapping.Add(XKeys.F1, System.Windows.Forms.Keys.F1);
                Mapping.Add(XKeys.F2, System.Windows.Forms.Keys.F2);
                Mapping.Add(XKeys.F3, System.Windows.Forms.Keys.F3);
                Mapping.Add(XKeys.F4, System.Windows.Forms.Keys.F4);
                Mapping.Add(XKeys.F5, System.Windows.Forms.Keys.F5);
                Mapping.Add(XKeys.F6, System.Windows.Forms.Keys.F6);
                Mapping.Add(XKeys.F7, System.Windows.Forms.Keys.F7);
                Mapping.Add(XKeys.F8, System.Windows.Forms.Keys.F8);
                Mapping.Add(XKeys.F9, System.Windows.Forms.Keys.F9);
                Mapping.Add(XKeys.F10, System.Windows.Forms.Keys.F10);
                // Missing: F11 (Caught by WM)
                Mapping.Add(XKeys.F12, System.Windows.Forms.Keys.F12);

                Mapping.Add(XKeys.Escape, System.Windows.Forms.Keys.Escape);
                Mapping.Add(XKeys.Tab, System.Windows.Forms.Keys.Tab);
                Mapping.Add(XKeys.CapsLock, System.Windows.Forms.Keys.CapsLock);
                Mapping.Add(XKeys.Tilde, System.Windows.Forms.Keys.Oemtilde);
                Mapping.Add(XKeys.Backslash, System.Windows.Forms.Keys.OemBackslash);
                Mapping.Add(XKeys.BackSpace, System.Windows.Forms.Keys.Back);

                Mapping.Add(XKeys.ScrollLock, System.Windows.Forms.Keys.Scroll);
                Mapping.Add(XKeys.Pause, System.Windows.Forms.Keys.Pause);
                Mapping.Add(XKeys.Insert, System.Windows.Forms.Keys.Insert);
                Mapping.Add(XKeys.Delete, System.Windows.Forms.Keys.Delete);
                Mapping.Add(XKeys.Home, System.Windows.Forms.Keys.Home);
                Mapping.Add(XKeys.End, System.Windows.Forms.Keys.End);
                Mapping.Add(XKeys.PageUp, System.Windows.Forms.Keys.PageUp);
                Mapping.Add(XKeys.PageDown, System.Windows.Forms.Keys.PageDown);
                Mapping.Add(XKeys.NumLock, System.Windows.Forms.Keys.NumLock);

                Mapping.Add(XKeys.SpaceBar, System.Windows.Forms.Keys.Space);
                Mapping.Add(XKeys.Return, System.Windows.Forms.Keys.Return);
                            
                Mapping.Add(XKeys.Slash, System.Windows.Forms.Keys.OemQuestion);
                Mapping.Add(XKeys.Dot, System.Windows.Forms.Keys.OemPeriod);
                Mapping.Add(XKeys.Comma, System.Windows.Forms.Keys.Oemcomma);
                Mapping.Add(XKeys.Semicolon, System.Windows.Forms.Keys.OemSemicolon);
                Mapping.Add(XKeys.OpenSquareBracket, System.Windows.Forms.Keys.OemOpenBrackets);
                Mapping.Add(XKeys.CloseSquareBracket, System.Windows.Forms.Keys.OemCloseBrackets);
                            
                Mapping.Add(XKeys.Up, System.Windows.Forms.Keys.Up);
                Mapping.Add(XKeys.Down, System.Windows.Forms.Keys.Down);
                Mapping.Add(XKeys.Right, System.Windows.Forms.Keys.Right);
                Mapping.Add(XKeys.Left, System.Windows.Forms.Keys.Left);

                // Not sure about these ....
                Mapping.Add(XKeys.Dash, System.Windows.Forms.Keys.OemMinus);
                Mapping.Add(XKeys.Equals, System.Windows.Forms.Keys.Oemplus);

                // No windows equivalent?
                Mapping.Add(XKeys.NumpadSlash, System.Windows.Forms.Keys.None);
                Mapping.Add(XKeys.NumpadAsterisk, System.Windows.Forms.Keys.None);
                Mapping.Add(XKeys.NumpadDot, System.Windows.Forms.Keys.None);
                Mapping.Add(XKeys.NumpadEnter, System.Windows.Forms.Keys.None);
                Mapping.Add(XKeys.NumpadPlus, System.Windows.Forms.Keys.None);
                Mapping.Add(XKeys.NumpadMinus, System.Windows.Forms.Keys.None);

                #region Cache
                // Add keys to the cache that can not be looked up with XLookupKeysym

                // HACK: I'm not sure these will work on other keyboard layouts.
                Cache.Add('(', new CachedKey(XKeys.OpenParens, true));
                Cache.Add(')', new CachedKey(XKeys.CloseParens, true));
                Cache.Add('[', new CachedKey(XKeys.OpenSquareBracket, true));
                Cache.Add(']', new CachedKey(XKeys.CloseSquareBracket, true));
                Cache.Add('=', new CachedKey(XKeys.Equals, true));
                Cache.Add('-', new CachedKey(XKeys.Dash, true));
                Cache.Add('!', new CachedKey(XKeys.ExMark, true));
                Cache.Add('@', new CachedKey(XKeys.At, true));
                Cache.Add('#', new CachedKey(XKeys.Hash, true));
                Cache.Add('$', new CachedKey(XKeys.Dollar, true));
                Cache.Add('%', new CachedKey(XKeys.Percent, true));
                Cache.Add('^', new CachedKey(XKeys.Circumflex, true));
                Cache.Add('&', new CachedKey(XKeys.Ampersand, true));
                Cache.Add('*', new CachedKey(XKeys.Asterisk, true));
                Cache.Add(' ', new CachedKey(XKeys.SpaceBar, false));

                #endregion
            }
        }
}