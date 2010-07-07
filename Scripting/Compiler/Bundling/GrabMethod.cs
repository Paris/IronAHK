using System;
using System.Reflection;
using System.Reflection.Emit;

namespace IronAHK.Scripting
{
    partial class ILMirror
    {
        public MethodInfo GrabMethod(MethodInfo Original)
        {
            return GrabMethod(Original, Target);
        }
        
        MethodInfo GrabMethod(MethodInfo Original, TypeBuilder On)
        {
            return GrabMethod(Original, On, false);
        }
        
        MethodInfo GrabMethod(MethodInfo Original, TypeBuilder On, bool Force)
        {
            if(Original == null) return null;
            
            if(!Force && !Sources.Contains(Original.DeclaringType))
                return MethodReplaceGenerics(Original);
            
            if(MethodsDone.ContainsKey(Original)) 
                return MethodsDone[Original];
            
            Type ReturnType;
            if(Sources.Contains(Original.ReturnType.DeclaringType))
                ReturnType = GrabType(Original.ReturnType);
            else ReturnType = Original.ReturnType;
            
            MethodBuilder Builder = On.DefineMethod(Prefix+Original.Name, Original.Attributes, 
                ReturnType, ParameterTypes(Original));
            
            MethodsDone.Add(Original, Builder);
            
            MethodImplAttributes Attr = Original.GetMethodImplementationFlags();
            Builder.SetImplementationFlags(Attr);
            
            if((Attr & MethodImplAttributes.Runtime) == MethodImplAttributes.Runtime &&
               (Attr & MethodImplAttributes.Managed) == MethodImplAttributes.Managed)
                return Builder;
            
            Console.WriteLine("Copying "+Original.Name);
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

