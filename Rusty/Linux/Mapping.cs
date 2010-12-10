using System.Collections.Generic;

namespace IronAHK.Rusty
{
    partial class LinuxAPI
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
                Mapping = new Dictionary<Keys, System.Windows.Forms.Keys>();
                Cache = new Dictionary<char, CachedKey>();
                
                Mapping.Add(Keys.LeftAlt, System.Windows.Forms.Keys.LMenu);
                Mapping.Add(Keys.RightAlt, System.Windows.Forms.Keys.RMenu);
                
                Mapping.Add(Keys.LeftControl, System.Windows.Forms.Keys.LControlKey);
                Mapping.Add(Keys.RightControl, System.Windows.Forms.Keys.RControlKey);
                
                Mapping.Add(Keys.LeftSuper, System.Windows.Forms.Keys.LWin);
                Mapping.Add(Keys.RightSuper, System.Windows.Forms.Keys.RWin);
                
                Mapping.Add(Keys.LeftShift, System.Windows.Forms.Keys.LShiftKey);
                Mapping.Add(Keys.RightShift, System.Windows.Forms.Keys.RShiftKey);
                
                Mapping.Add(Keys.F1, System.Windows.Forms.Keys.F1);
                Mapping.Add(Keys.F2, System.Windows.Forms.Keys.F2);
                Mapping.Add(Keys.F3, System.Windows.Forms.Keys.F3);
                Mapping.Add(Keys.F4, System.Windows.Forms.Keys.F4);
                Mapping.Add(Keys.F5, System.Windows.Forms.Keys.F5);
                Mapping.Add(Keys.F6, System.Windows.Forms.Keys.F6);
                Mapping.Add(Keys.F7, System.Windows.Forms.Keys.F7);
                Mapping.Add(Keys.F8, System.Windows.Forms.Keys.F8);
                Mapping.Add(Keys.F9, System.Windows.Forms.Keys.F9);
                Mapping.Add(Keys.F10, System.Windows.Forms.Keys.F10);
                // Missing: F11 (Caught by WM)
                Mapping.Add(Keys.F12, System.Windows.Forms.Keys.F12);
                
                Mapping.Add(Keys.Escape, System.Windows.Forms.Keys.Escape);
                Mapping.Add(Keys.Tab, System.Windows.Forms.Keys.Tab);
                Mapping.Add(Keys.CapsLock, System.Windows.Forms.Keys.CapsLock);
                Mapping.Add(Keys.Tilde, System.Windows.Forms.Keys.Oemtilde);
                Mapping.Add(Keys.Backslash, System.Windows.Forms.Keys.OemBackslash);
                Mapping.Add(Keys.BackSpace, System.Windows.Forms.Keys.Back);
                
                Mapping.Add(Keys.ScrollLock, System.Windows.Forms.Keys.Scroll);
                Mapping.Add(Keys.Pause, System.Windows.Forms.Keys.Pause);
                Mapping.Add(Keys.Insert, System.Windows.Forms.Keys.Insert);
                Mapping.Add(Keys.Delete, System.Windows.Forms.Keys.Delete);
                Mapping.Add(Keys.Home, System.Windows.Forms.Keys.Home);
                Mapping.Add(Keys.End, System.Windows.Forms.Keys.End);
                Mapping.Add(Keys.PageUp, System.Windows.Forms.Keys.PageUp);
                Mapping.Add(Keys.PageDown, System.Windows.Forms.Keys.PageDown);
                Mapping.Add(Keys.NumLock, System.Windows.Forms.Keys.NumLock);
                
                Mapping.Add(Keys.SpaceBar, System.Windows.Forms.Keys.Space);
                Mapping.Add(Keys.Return, System.Windows.Forms.Keys.Return);
                
                Mapping.Add(Keys.Slash, System.Windows.Forms.Keys.OemQuestion);
                Mapping.Add(Keys.Dot, System.Windows.Forms.Keys.OemPeriod);
                Mapping.Add(Keys.Comma, System.Windows.Forms.Keys.Oemcomma);
                Mapping.Add(Keys.Semicolon, System.Windows.Forms.Keys.OemSemicolon);
                Mapping.Add(Keys.OpenSquareBracket, System.Windows.Forms.Keys.OemOpenBrackets);
                Mapping.Add(Keys.CloseSquareBracket, System.Windows.Forms.Keys.OemCloseBrackets);
                
                Mapping.Add(Keys.Up, System.Windows.Forms.Keys.Up);
                Mapping.Add(Keys.Down, System.Windows.Forms.Keys.Down);
                Mapping.Add(Keys.Right, System.Windows.Forms.Keys.Right);
                Mapping.Add(Keys.Left, System.Windows.Forms.Keys.Left);
                
                // Not sure about these ....
                Mapping.Add(Keys.Dash, System.Windows.Forms.Keys.OemMinus);
                Mapping.Add(Keys.Equals, System.Windows.Forms.Keys.Oemplus);
                
                // No windows equivalent?
                Mapping.Add(Keys.NumpadSlash, System.Windows.Forms.Keys.None);
                Mapping.Add(Keys.NumpadAsterisk, System.Windows.Forms.Keys.None);
                Mapping.Add(Keys.NumpadDot, System.Windows.Forms.Keys.None);
                Mapping.Add(Keys.NumpadEnter, System.Windows.Forms.Keys.None);
                Mapping.Add(Keys.NumpadPlus, System.Windows.Forms.Keys.None);
                Mapping.Add(Keys.NumpadMinus, System.Windows.Forms.Keys.None);
                
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
                Cache.Add(' ', new CachedKey(Keys.SpaceBar, false));
                
                #endregion
            }
        }
    }
}

