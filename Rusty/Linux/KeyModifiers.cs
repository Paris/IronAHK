
using System;

namespace IronAHK.Rusty
{
    class KeyModifier
    {
        public bool LeftDown = false;
        public bool RightDown = false;
        public string Name;
        
        public KeyModifier(string Name)
        {
            this.Name = Name;
        }
        
        public void Dump()
        {
            Console.WriteLine("[{0}] Right: {1}, Left: {2}", Name, RightDown, LeftDown);
        }
        
        public bool Down 
        {
            get { return LeftDown || RightDown; }
            set { LeftDown = RightDown = value; }
        }
        
        internal void Flip()
        {
            Flip(KeyType.None);
        }
        
        internal void Flip(KeyType Kind)
        {
            if(Kind == KeyType.None) Down = !Down;
            else if(Kind == KeyType.Left) LeftDown = !LeftDown;
            else if(Kind == KeyType.Right) RightDown = !RightDown;
        }
        
        internal bool Compatible(KeyModifier Modifier)
        {
            // Left or right is down and either will do
            if(Modifier.LeftDown && Modifier.RightDown && Down) return true;
            
            // Left is required and is not down
            if(Modifier.LeftDown && !LeftDown) return false;
            // Right is required and is not down
            if(Modifier.RightDown && !RightDown) return false;
            
            return true;
        }
    }
    
    class KeyModifiers
    {
        public KeyModifier Super;
        public KeyModifier Alt;
        public KeyModifier Shift;
        public KeyModifier Control;
        
        public KeyModifiers()
        {
            Super = new KeyModifier("Super");
            Alt = new KeyModifier("Alt");
            Shift = new KeyModifier("Shift");
            Control = new KeyModifier("Control");
        }
        
        public bool Matches(KeyModifiers Compare)
        {
            return Matches(Compare, true);    
        }
        
        public bool Matches(KeyModifiers Compare, bool Strict)
        {
            if(Strict)
            {
                return Super.Compatible(Compare.Super) && Alt.Compatible(Compare.Alt) && 
                    Shift.Compatible(Compare.Shift) && Control.Compatible(Compare.Control);
            }
            else
            {
                if(Compare.Super.Down && !Super.Compatible(Compare.Super)) return false;
                if(Compare.Alt.Down && !Alt.Compatible(Compare.Alt)) return false;
                if(Compare.Shift.Down && !Shift.Compatible(Compare.Shift)) return false;
                if(Compare.Control.Down && !Control.Compatible(Compare.Control)) return false;
                   
                return true;
            }
        }
        
        public void Dump()
        {
            Super.Dump();
            Alt.Dump();
            Shift.Dump();
            Control.Dump();
        }
    }
}
