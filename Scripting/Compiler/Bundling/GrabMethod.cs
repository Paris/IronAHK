using System;
using System.Reflection;
using System.Reflection.Emit;

namespace IronAHK.Scripting
{
    partial class ILMirror
    {
        public MethodInfo GrabMethod(MethodInfo Original)
        {
            if(Original == null) return null;
            
            if(MethodsDone.ContainsKey(Original)) 
                return MethodsDone[Original];
            
            if(!Sources.Contains(Original.Module))
                return MethodReplaceGenerics(Original);
            
            TypeBuilder On = GrabType(Original.DeclaringType) as TypeBuilder;
            
            Type ReturnType;
            if(Sources.Contains(Original.ReturnType.Module))
                ReturnType = GrabType(Original.ReturnType);
            else ReturnType = Original.ReturnType;
            
            MethodBuilder Builder = On.DefineMethod(Original.Name, Original.Attributes, 
                ReturnType, ParameterTypes(Original));
            
            MethodsDone.Add(Original, Builder);
            
            Builder.SetImplementationFlags(Original.GetMethodImplementationFlags());
            
            CopyMethodBody(Original, Builder.GetILGenerator());
            
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

