
using System;

namespace IronAHK.Rusty
{
    enum KeyType { Left, None, Right };
    
    class KeyCombination
    {
        public KeyModifiers Modifiers;
        public char Trigger;
        
        public KeyCombination(string Expression)
        {
            Modifiers = new KeyModifiers();
            KeyType Kind = KeyType.None;
            
            for(int i = 0; i < Expression.Length; i++)
            {
                switch(Expression[i])
                {
                    case '#': 
                        SetKey(Modifiers.Super, Kind);
                        break;
                    case '!':
                        SetKey(Modifiers.Alt, Kind);
                        break;
                    case '^':
                        SetKey(Modifiers.Control, Kind);
                        break;
                    case '+':
                        SetKey(Modifiers.Shift, Kind);
                        break;
                    
                    case '>':
                        Kind = KeyType.Right;
                        break;
                        
                    case '<':
                        Kind = KeyType.Left;
                        break;
                        
                    default: 
                        Kind = KeyType.None;
                        Trigger = Expression[i];
                        break;
                }
            }
            
        }
        
        private void SetKey(KeyModifier Modifier, KeyType Kind)
        {
            SetKey(Modifier, Kind, true);
        }
        
        private void SetKey(KeyModifier Modifier, KeyType Kind, bool Value)
        {
            if(Kind == KeyType.None) Modifier.Down = Value;
            else if(Kind == KeyType.Left) Modifier.LeftDown = Value;
            else if(Kind == KeyType.Right) Modifier.RightDown = Value;
        }
    }
}
