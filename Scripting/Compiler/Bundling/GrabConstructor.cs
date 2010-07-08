using System;
using System.Reflection;
using System.Reflection.Emit;

namespace IronAHK.Scripting
{
    partial class ILMirror
    {
        ConstructorInfo GrabConstructor(ConstructorInfo Original)
        {
            if(Sources.Contains(Original.Module))
                return GrabConstructor(Original, GrabType(Original.DeclaringType) as TypeBuilder);
            
            return GrabConstructor(Original, Target);
        }
        
        ConstructorInfo GrabConstructor(ConstructorInfo Original, TypeBuilder On)
        {
            if(ConstructorsDone.ContainsKey(Original))
                return ConstructorsDone[Original];
            
            if(!Sources.Contains(Original.Module))
                return ConstructorReplaceGenerics(Original);
            
            ConstructorBuilder Builder = On.DefineConstructor(Original.Attributes, 
                Original.CallingConvention, ParameterTypes(Original));
            
            MethodImplAttributes Attr = Original.GetMethodImplementationFlags();
            Builder.SetImplementationFlags(Attr);
            
            ConstructorsDone.Add(Original, Builder);
            
            if((Attr & MethodImplAttributes.Runtime) == MethodImplAttributes.Runtime &&
               (Attr & MethodImplAttributes.Managed) == MethodImplAttributes.Managed)
                return Builder;
            
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

