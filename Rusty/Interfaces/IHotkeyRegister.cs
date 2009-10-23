// IHotkeyRegister.cs. Created by tobias at 5:19 PM 2/10/2009
// Specifies the interface which all hotkeyregisters have to obey

namespace IronAHK.Rusty
{
    internal enum KeyEventType { Hotkey, Hotstring };

    internal struct KeyEvent
    {
        public KeyEventType Type;
        public object ActualEvent;
        
        public KeyEvent(KeyEventType Type, object ActualEvent)
        {
            this.Type = Type;
            this.ActualEvent = ActualEvent;
        }
    }
    
    internal interface IHotkeyRegister
    {
        string Platform { get; }

        bool RegisterHotkey(KeyCombination Key);
        bool RegisterHotstring(string Sequence, string Replace);

        KeyEvent NextEvent();
    }
}
