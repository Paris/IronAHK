using System;
using System.Collections.Generic;
using WF = System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Linux
    {
        public partial class KeyboardHook : Core.KeyboardHook
        {
            private struct CachedKey
            {
                public uint Sym;
                public bool Shift;
                
                public CachedKey(uint Sym, bool Shift)
                {
                    this.Sym = Sym;
                    this.Shift = Shift;
                }
                
                public CachedKey(Keys Sym, bool Shift) : this((uint) Sym, Shift)
                {
                }
            }
            
            public void SetupMapping()
            {
                Mapping = new Dictionary<Keys, WF.Keys>();
                Cache = new Dictionary<char, CachedKey>();
                
                Mapping.Add(Keys.LeftAlt, WF.Keys.LMenu);
                Mapping.Add(Keys.RightAlt, WF.Keys.RMenu);
                
                Mapping.Add(Keys.LeftControl, WF.Keys.LControlKey);
                Mapping.Add(Keys.RightControl, WF.Keys.RControlKey);
                
                Mapping.Add(Keys.LeftSuper, WF.Keys.LWin);
                Mapping.Add(Keys.RightSuper, WF.Keys.RWin);
                
                Mapping.Add(Keys.LeftShift, WF.Keys.LShiftKey);
                Mapping.Add(Keys.RightShift, WF.Keys.RShiftKey);
                
                Mapping.Add(Keys.F1, WF.Keys.F1);
                Mapping.Add(Keys.F2, WF.Keys.F2);
                Mapping.Add(Keys.F3, WF.Keys.F3);
                Mapping.Add(Keys.F4, WF.Keys.F4);
                Mapping.Add(Keys.F5, WF.Keys.F5);
                Mapping.Add(Keys.F6, WF.Keys.F6);
                Mapping.Add(Keys.F7, WF.Keys.F7);
                Mapping.Add(Keys.F8, WF.Keys.F8);
                Mapping.Add(Keys.F9, WF.Keys.F9);
                Mapping.Add(Keys.F10, WF.Keys.F10);
                // Missing: F11 (Caught by WM)
                Mapping.Add(Keys.F12, WF.Keys.F12);
                
                Mapping.Add(Keys.Escape, WF.Keys.Escape);
                Mapping.Add(Keys.Tab, WF.Keys.Tab);
                Mapping.Add(Keys.CapsLock, WF.Keys.CapsLock);
                Mapping.Add(Keys.Tilde, WF.Keys.Oemtilde);
                Mapping.Add(Keys.Backslash, WF.Keys.OemBackslash);
                Mapping.Add(Keys.BackSpace, WF.Keys.Back);
                
                Mapping.Add(Keys.ScrollLock, WF.Keys.Scroll);
                Mapping.Add(Keys.Pause, WF.Keys.Pause);
                Mapping.Add(Keys.Insert, WF.Keys.Insert);
                Mapping.Add(Keys.Delete, WF.Keys.Delete);
                Mapping.Add(Keys.Home, WF.Keys.Home);
                Mapping.Add(Keys.End, WF.Keys.End);
                Mapping.Add(Keys.PageUp, WF.Keys.PageUp);
                Mapping.Add(Keys.PageDown, WF.Keys.PageDown);
                Mapping.Add(Keys.NumLock, WF.Keys.NumLock);
                
                Mapping.Add(Keys.SpaceBar, WF.Keys.Space);
                Mapping.Add(Keys.Return, WF.Keys.Return);
                
                Mapping.Add(Keys.Slash, WF.Keys.OemQuestion);
                Mapping.Add(Keys.Dot, WF.Keys.OemPeriod);
                Mapping.Add(Keys.Comma, WF.Keys.Oemcomma);
                Mapping.Add(Keys.Semicolon, WF.Keys.OemSemicolon);
                Mapping.Add(Keys.OpenSquareBracket, WF.Keys.OemOpenBrackets);
                Mapping.Add(Keys.CloseSquareBracket, WF.Keys.OemCloseBrackets);
                
                Mapping.Add(Keys.Up, WF.Keys.Up);
                Mapping.Add(Keys.Down, WF.Keys.Down);
                Mapping.Add(Keys.Right, WF.Keys.Right);
                Mapping.Add(Keys.Left, WF.Keys.Left);
                
                // Not sure about these ....
                Mapping.Add(Keys.Dash, WF.Keys.OemMinus);
                Mapping.Add(Keys.Equals, WF.Keys.Oemplus);
                
                // No windows equivalent?
                Mapping.Add(Keys.NumpadSlash, WF.Keys.None);
                Mapping.Add(Keys.NumpadAsterisk, WF.Keys.None);
                Mapping.Add(Keys.NumpadDot, WF.Keys.None);
                Mapping.Add(Keys.NumpadEnter, WF.Keys.None);
                Mapping.Add(Keys.NumpadPlus, WF.Keys.None);
                Mapping.Add(Keys.NumpadMinus, WF.Keys.None);
                
                #region Cache
                // Add keys to the cache that can not be looked up with XLookupKeysym
                
                // HACK: I'm not sure these will work on other keyboard layouts.
                Cache.Add('(', new CachedKey(Keys.OpenParens, true));
                Cache.Add(')', new CachedKey(Keys.CloseParens, true));
                Cache.Add('[', new CachedKey(Keys.OpenSquareBracket, true));
                Cache.Add(']', new CachedKey(Keys.CloseSquareBracket, true));
                Cache.Add('=', new CachedKey(Keys.Equals, true));
                Cache.Add('-', new CachedKey(Keys.Dash, true));
                Cache.Add('!', new CachedKey(Keys.ExMark, true));
                Cache.Add('@', new CachedKey(Keys.At, true));
                Cache.Add('#', new CachedKey(Keys.Hash, true));
                Cache.Add('$', new CachedKey(Keys.Dollar, true));
                Cache.Add('%', new CachedKey(Keys.Percent, true));
                Cache.Add('^', new CachedKey(Keys.Circumflex, true));
                Cache.Add('&', new CachedKey(Keys.Ampersand, true));
                Cache.Add('*', new CachedKey(Keys.Asterisk, true));
                
                #endregion
            }
        }
    }
}

