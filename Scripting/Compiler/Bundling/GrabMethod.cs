using System;
using System.Reflection;
using System.Reflection.Emit;

namespace IronAHK.Scripting
{
    partial class ILMirror
    {
        public MethodInfo GrabMethod(MethodInfo Original)
        {
            if(Sources.Contains(Original.Module))
               return GrabMethod(Original, GrabType(Original.DeclaringType) as TypeBuilder);
            
            return GrabMethod(Original, null);
        }
        
        MethodInfo GrabMethod(MethodInfo Original, TypeBuilder On)
        {
            if(Original == null) return null;
            
            if(!Sources.Contains(Original.Module))
                return MethodReplaceGenerics(Original);
            
            if(MethodsDone.ContainsKey(Original)) 
                return MethodsDone[Original];
            
            Type ReturnType;
            if(Sources.Contains(Original.ReturnType.Module))
                ReturnType = GrabType(Original.ReturnType);
            else ReturnType = Original.ReturnType;
            
            MethodBuilder Builder = On.DefineMethod(Original.Name, Original.Attributes, 
                ReturnType, ParameterTypes(Original));
            
            MethodsDone.Add(Original, Builder);
            
            MethodImplAttributes Attr = Original.GetMethodImplementationFlags();
            Builder.SetImplementationFlags(Attr);
            
            if((Attr & MethodImplAttributes.Runtime) == MethodImplAttributes.Runtime &&
               (Attr & MethodImplAttributes.Managed) == MethodImplAttributes.Managed)
                return Builder;
            
            CopyMethodBody(Original.GetMethodBody(), Builder.GetILGenerator(), Original.Module);
            
            return Builder; 
        }
        
        MethodInfo MethodReplaceGenerics(MethodInfo Original)
        {
            if(Original.IsGenericMethod)
            {
                Type[] Replace = ReplaceGenericArguments(Original.GetGenericArguments());
                return Original.GetGenericMethodDefinition().MakeGenericMethod(Replace);
            }
            else return Original;
        }
    }
}

