using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace IronAHK.Scripting
{
    partial class MethodCollection : List<MethodInfo>
    {
        Dictionary<MethodBase, MethodBuilder> Done;
        
        public List<Type> Sources;
        public TypeBuilder Target;
        public ModuleBuilder Module;
        
        public MethodCollection()
        {
            Sources = new List<Type>();
            Done = new Dictionary<MethodBase, MethodBuilder>();
        }
        
        public MethodBuilder GrabMethod(MethodInfo Original)
        {
            if(Original == null) return null;
            
            if(Done.ContainsKey(Original)) 
                return Done[Original];
            
            MethodBuilder Builder = Target.DefineMethod("builtin_"+Original.Name, Original.Attributes, 
                Original.ReturnType, ParameterTypes(Original));
            ILGenerator Gen = Builder.GetILGenerator();
            
            Gen.Emit(OpCodes.Ret);
            
            Done.Add(Original, Builder);
            return Builder; 
        }
        
        static Type[] ParameterTypes(MethodInfo Original)
        {
            ParameterInfo[] Params = Original.GetParameters();
            Type[] Ret = new Type[Params.Length];
            
            for(int i = 0; i < Params.Length; i++)
                Ret[i] = Params[i].ParameterType;
            
            return Ret;
        }
    }
}

