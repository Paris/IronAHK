using System;
using System.Reflection;
using System.Reflection.Emit;

namespace IronAHK.Scripting
{
    partial class ILMirror
    {
        public ConstructorInfo GrabConstructor(ConstructorInfo Original)
        {
            if(Original == null) return null;
            
            if(ConstructorsDone.ContainsKey(Original))
                return ConstructorsDone[Original];
            
            if(!Sources.Contains(Original.Module))
                return ConstructorReplaceGenerics(Original);
            
            TypeBuilder On = GrabType(Original.DeclaringType) as TypeBuilder;
            
            ConstructorBuilder Builder = On.DefineConstructor(Original.Attributes, 
                Original.CallingConvention, ParameterTypes(Original));
            
            Builder.SetImplementationFlags(Original.GetMethodImplementationFlags());
            
            ConstructorsDone.Add(Original, Builder);
            
            CopyMethodBody(Original, Builder.GetILGenerator());
            
            return Builder;
        }
        
        ConstructorInfo ConstructorReplaceGenerics(ConstructorInfo Orig)
        {
            if(Orig.DeclaringType.IsGenericType)
            {
                Type NewDeclarator = TypeReplaceGenerics(Orig.DeclaringType);
                return NewDeclarator.GetConstructor(ParameterTypes(Orig));
            }
            else return Orig;
        }
        
        ConstructorInfo FindStaticConstructor(Type Origin)
        {
            foreach(ConstructorInfo Info in Origin.GetConstructors(BindingFlags.NonPublic | BindingFlags.Static))
            {
                if(Info.GetParameters().Length == 0)
                    return Info;
            }
            
            return null;
        }
    }
}

