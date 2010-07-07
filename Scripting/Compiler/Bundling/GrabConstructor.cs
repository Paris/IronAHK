using System;
using System.Reflection;
using System.Reflection.Emit;

namespace IronAHK.Scripting
{
    partial class ILMirror
    {
        ConstructorInfo GrabConstructor(ConstructorInfo Original)
        {
            return GrabConstructor(Original, Target, false);
        }
        
        ConstructorInfo GrabConstructor(ConstructorInfo Original, TypeBuilder On, bool Force)
        {
            if(ConstructorsDone.ContainsKey(Original))
                return ConstructorsDone[Original];
            
            if(!Force && !Sources.Contains(Original.DeclaringType))
                return ConstructorReplaceGenerics(Original);
            
            ConstructorBuilder Builder = On.DefineConstructor(Original.Attributes, 
                Original.CallingConvention, ParameterTypes(Original));
            
            Builder.SetImplementationFlags(Original.GetMethodImplementationFlags());
            
            ConstructorsDone.Add(Original, Builder);
            
            CopyMethodBody(Original.GetMethodBody(), Builder.GetILGenerator(), Original.Module);
            
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
    }
}

