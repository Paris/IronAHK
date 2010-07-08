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
            
            if(Sources.Contains(Copy.Module))
                return GrabType(Copy, GrabType(Copy.DeclaringType) as TypeBuilder);
            
            return GrabType(Copy, null);
        }
        
        Type GrabType(Type Copy, TypeBuilder On)
        {
            if(TypesDone.ContainsKey(Copy))
                return TypesDone[Copy];
            
            if(!Sources.Contains(Copy.Module)) 
                return TypeReplaceGenerics(Copy);
            
            TypeBuilder Ret;
            
            if(On == null)
                Ret = Module.DefineType(Copy.Name, Copy.Attributes, GrabType(Copy.BaseType), Copy.GetInterfaces());
            else Ret = On.DefineNestedType(Copy.Name, Copy.Attributes, GrabType(Copy.BaseType), Copy.GetInterfaces());
            TypesDone.Add(Copy, Ret);
            
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

