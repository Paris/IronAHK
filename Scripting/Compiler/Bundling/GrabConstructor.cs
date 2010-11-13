using System;
using System.Reflection;
using System.Reflection.Emit;

namespace IronAHK.Scripting
{
    partial class ILMirror
    {
        public ConstructorInfo GrabConstructor(ConstructorInfo Original)
        {
            return GrabConstructor(Original, null);
        }
        
        protected ConstructorInfo GrabConstructor(ConstructorInfo Original, TypeBuilder On)
        {
            if(Original == null) return null;
            
            if(ConstructorsDone.ContainsKey(Original))
                return ConstructorsDone[Original];
            
            if(!Sources.Contains(Original.Module))
                return ConstructorReplaceGenerics(Original);
            
            if(On == null)
                On = GrabType(Original.DeclaringType) as TypeBuilder;
            
            ConstructorBuilder Builder = On.DefineConstructor(Original.Attributes, 
                Original.CallingConvention, ParameterTypes(Original));
            
            Builder.SetImplementationFlags(Original.GetMethodImplementationFlags());
            
            if(ConstructorsDone.ContainsKey(Original))
                return ConstructorsDone[Original];
            
            ConstructorsDone.Add(Original, Builder);
            
            CopyMethodBody(Original, Builder);
            
            return Builder;
        }
        
        ConstructorInfo ConstructorReplaceGenerics(ConstructorInfo Orig)
        {
            if(Orig.DeclaringType.IsGenericType)
            {
                Type NewDeclarator = TypeReplaceGenerics(Orig.DeclaringType);
                
                if(NewDeclarator == Orig.DeclaringType)
                    return Orig;
                
                ConstructorInfo Generic = FindMatchingGenericMethod(Orig) as ConstructorInfo;
                return TypeBuilder.GetConstructor(NewDeclarator, Generic);
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

