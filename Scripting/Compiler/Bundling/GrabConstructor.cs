using System;
using System.Reflection;
using System.Reflection.Emit;

namespace IronAHK.Scripting
{
    partial class ILMirror
    {
        ConstructorInfo GrabConstructor(ConstructorInfo Original, TypeBuilder On)
        {
            if(ConstructorsDone.ContainsKey(Original))
                return ConstructorsDone[Original];
            
            if(!Sources.Contains(Original.DeclaringType))
                return Original;
            
            ConstructorBuilder Builder = On.DefineConstructor(Original.Attributes, 
                Original.CallingConvention, ParameterTypes(Original));
            ILGenerator Gen = Builder.GetILGenerator();
            
            ConstructorsDone.Add(Original, Builder);
            
            CopyMethodBody(Original.GetMethodBody(), Gen, Original.Module);
            
            return Builder;
        }
    }
}

