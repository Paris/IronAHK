using System;
using System.Reflection;
using System.Reflection.Emit;

namespace IronAHK.Scripting
{
    partial class ILMirror
    {
        Type GrabType(Type Copy)
        {
            return GrabType(Copy, false, false);
        }
        
        Type GrabType(Type Copy, TypeBuilder On, bool AvoidSelf)
        {
            return GrabType(Copy, On, AvoidSelf, false);
        }
        
        Type GrabType(Type Copy, TypeBuilder On, bool AvoidSelf, bool Force)
        {
            if(TypesDone.ContainsKey(Copy))
                return TypesDone[Copy];
            
            if(!AvoidSelf && Sources.Contains(Copy))
               return Target;
            
            if(!Force && !Sources.Contains(Copy.DeclaringType)) 
                return TypeReplaceGenerics(Copy);
            
            TypeBuilder Ret = On.DefineNestedType(string.Format("{0}{1}_{2}", Prefix, Copy.Assembly.GetName().Name, Copy.Name), 
                                                      Copy.Attributes, Copy.BaseType, Copy.GetInterfaces());
            TypesDone.Add(Copy, Ret);
            
            // Walk through methods and fields, and copy them
            foreach(MemberInfo Member in Copy.GetMembers())
            {
                if(Member.DeclaringType != Copy && !Force) continue;
                
                switch(Member.MemberType)
                {
                    case MemberTypes.Method:
                        GrabMethod(Member as MethodInfo, Ret, true);
                        break;
                        
                    case MemberTypes.Field:
                        GrabField(Member as FieldInfo, Ret, true);
                        break;
                        
                    case MemberTypes.Constructor:
                        GrabConstructor(Member as ConstructorInfo, Ret, true);
                        break;
                }
            }
            
            
            Ret.CreateType();
            
            return Ret;            
        }
        
        public Type GrabType(Type Copy, bool AvoidSelf, bool Force)
        {
            return GrabType(Copy, Target, AvoidSelf, Force);
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
                if(Sources.Contains(Params[i].ParameterType.DeclaringType))
                    Ret[i] = GrabType(Params[i].ParameterType);
                else Ret[i] = Params[i].ParameterType;
            }
            
            return Ret;
        }
    }
}

