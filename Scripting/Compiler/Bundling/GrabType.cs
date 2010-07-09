using System;
using System.Reflection;
using System.Reflection.Emit;

namespace IronAHK.Scripting
{
    partial class ILMirror
    {
        public Type GrabType(Type Copy)
        {
            if(Copy == null) return null;
            
            if(TypesDone.ContainsKey(Copy))
                return TypesDone[Copy];
            
            if(!Sources.Contains(Copy.Module)) 
                return TypeReplaceGenerics(Copy);
            
            TypeBuilder Ret, On = GrabType(Copy.DeclaringType) as TypeBuilder;
            
            if(On == null) Ret = Module.DefineType(Copy.Name, Copy.Attributes, GrabType(Copy.BaseType), Copy.GetInterfaces());
            else Ret = On.DefineNestedType(Copy.Name, Copy.Attributes, GrabType(Copy.BaseType), Copy.GetInterfaces());
            
            TypesDone.Add(Copy, Ret);
            
            // We need to copy over the static constructor explicitly, because it is never called in the IL
            ConstructorInfo StaticConstr = FindStaticConstructor(Copy);
            if(StaticConstr != null)
                GrabConstructor(StaticConstr);
            
            // Enum fields need to be copied over, too, since the IL relies on 
            // their numerical values rather than their field references
            if(Copy.BaseType == typeof(Enum))
                GrabField(Copy.GetField("value__"));
            
            if(Copy.BaseType == typeof(MulticastDelegate))
            {
                foreach(MethodInfo Method in Copy.GetMethods())
                    GrabMethod(Method);
            }
                 
            return Ret;            
        }
        
        Type TypeReplaceGenerics(Type Orig)
        {
            if(Orig.IsGenericType)
            {
                Type[] Replace = ReplaceGenericArguments(Orig.GetGenericArguments());
                return Orig.GetGenericTypeDefinition().MakeGenericType(Replace);
            }
            else return Orig;
        }
        
        Type[] ReplaceGenericArguments(Type[] Replace)
        {
            for(int i = 0; i < Replace.Length; i++)
                Replace[i] = GrabType(Replace[i]);
            
            return Replace;
        }
        
        Type[] ParameterTypes(MethodBase Original)
        {
            ParameterInfo[] Params = Original.GetParameters();
            Type[] Ret = new Type[Params.Length];
            
            for(int i = 0; i < Params.Length; i++)
            {
                if(Sources.Contains(Params[i].ParameterType.Module))
                    Ret[i] = GrabType(Params[i].ParameterType);
                else Ret[i] = Params[i].ParameterType;
            }
            
            return Ret;
        }
    }
}

