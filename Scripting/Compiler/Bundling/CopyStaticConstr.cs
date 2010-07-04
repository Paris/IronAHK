using System;
using System.Reflection;
using System.Reflection.Emit;

namespace IronAHK.Scripting
{
    partial class ILMirror
    {
        public void SimulateStaticConstructors(ILGenerator On)
        {
            foreach(Type Source in Sources)
            {
                ConstructorInfo Const = FindStaticConstructor(Source);
                
                if(Const == null) continue;
                
                On.Emit(OpCodes.Call, CopyStaticConstructor(Const));
            }
        }
        
        ConstructorInfo FindStaticConstructor(Type From)
        {
            // We need to specify NonPublic and Static to get the .cctor (static constructor)
            foreach(ConstructorInfo Info in From.GetConstructors(BindingFlags.NonPublic | BindingFlags.Static))
                return Info;
            
            return null;
        }
        
        MethodInfo CopyStaticConstructor(ConstructorInfo Info)
        {
            MethodBuilder Pseudo = Target.DefineMethod(StatConPrefix+Info.DeclaringType, 
                MethodAttributes.Static, typeof(void), Type.EmptyTypes);
            
            CopyMethodBody(Info.GetMethodBody(), Pseudo.GetILGenerator(), Info.Module);
            
            return Pseudo;
        }        
    }
}

