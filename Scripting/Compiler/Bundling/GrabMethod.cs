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
            if(Original == null) return null;
            
            if(!Sources.Contains(Original.DeclaringType))
                return Original;
            
            if(MethodsDone.ContainsKey(Original)) 
                return MethodsDone[Original];
            
            Type ReturnType;
            if(Sources.Contains(Original.ReturnType.DeclaringType))
                ReturnType = GrabType(Original.ReturnType);
            else ReturnType = Original.ReturnType;
            
            MethodBuilder Builder = On.DefineMethod(Prefix+Original.Name, Original.Attributes, 
                ReturnType, ParameterTypes(Original));
            ILGenerator Gen = Builder.GetILGenerator();
            
            MethodsDone.Add(Original, Builder);
            
            CopyMethodBody(Original.GetMethodBody(), Gen, Original.Module);
            
            return Builder; 
        }
    }
}

