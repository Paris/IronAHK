using System;
using System.Collections.Generic;
using System.Text;

namespace IronAHK.Rusty.Linux.X11
{
    internal enum XKeys
    {
        LeftControl = 37,
        RightControl = 105,
        LeftShift = 50,
        RightShift = 62,
        LeftAlt = 64,
        RightAlt = 108,
        LeftSuper = 133,
        RightSuper = 134,
        SpaceBar = 65,
        LowerLetter = 24,     // q on qwerty
        UpperLetter = 58,     // m 
        BackSpace = 22,
        Return = 36,
        // Missing: "Menu key"

        F1 = 67,
        F2 = 68,
        F3 = 69,
        F4 = 70,
        F5 = 71,
        F6 = 72,
        F7 = 73,
        F8 = 74,
        F9 = 75,
        F10 = 76,
        // Missing: F11
        F12 = 96,

        Escape = 9,
        Tab = 23,
        CapsLock = 66,
        Tilde = 49,
        Backslash = 51,

        // Missing: PrintScrn
        ScrollLock = 78,
        Pause = 127,
        Insert = 118,
        Delete = 119,
        Home = 110,
        End = 115,
        PageUp = 112,
        PageDown = 117,
        NumLock = 77,

        Slash = 61,
        Dot = 60,
        Comma = 59,
        Quote = 48,
        Semicolon = 47,
        OpenSquareBracket = 34,
        CloseSquareBracket = 35,

        ExMark = 10,
        At = 11,
        Hash = 12,
        Dollar = 13,
        Percent = 14,
        Circumflex = 15,
        Ampersand = 16,
        Asterisk = 17,
        OpenParens = 18,
        CloseParens = 19,
        Dash = 20,
        Equals = 21,

        NumpadSlash = 106,
        NumpadAsterisk = 63,
        NumpadMinus = 82,
        NumpadPlus = 86,
        NumpadEnter = 104,
        NumpadDot = 91,

        Left = 113,
        Right = 114,
        Up = 111,
        Down = 116
    }

    [Flags]
    internal enum KeyMasks
    {
        ShiftMask = (1 << 0),
        LockMask = (1 << 1),
        ControlMask = (1 << 2),
        Mod1Mask = (1 << 3),
        Mod2Mask = (1 << 4),
        Mod3Mask = (1 << 5),
        Mod4Mask = (1 << 6),
        Mod5Mask = (1 << 7),

        ModMasks = Mod1Mask | Mod2Mask | Mod3Mask | Mod4Mask | Mod5Mask
    }
}
