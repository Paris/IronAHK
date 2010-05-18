using System;
using System.CodeDom;
using System.Reflection;

namespace IronAHK.Scripting
{
    class MethodReference
    {
        Type targetObject;
        string methodName;
        Type[] typeArguments;

        public MethodReference(Type targetObject, string methodName)
            : this(targetObject, methodName, null) { }

        public MethodReference(Type targetObject, string methodName, Type[] typeArguments)
        {
            this.targetObject = targetObject;
            this.methodName = methodName;
            this.typeArguments = typeArguments;
        }

        public Type TargetObject
        {
            get { return targetObject; }
            set { targetObject = value; }
        }

        public string MethodName
        {
            get { return methodName; }
            set { methodName = value; }
        }

        public Type[] TypeArguments
        {
            get { return typeArguments; }
            set { typeArguments = value; }
        }

        public static explicit operator CodeMethodReferenceExpression(MethodReference source)
        {
            var method = new CodeMethodReferenceExpression();

            method.TargetObject = new CodeTypeReferenceExpression(source.targetObject);
            method.MethodName = source.methodName;

            if (source.typeArguments != null)
            {
                foreach (var argument in source.typeArguments)
                {
                    method.TypeArguments.Add(new CodeTypeReference(argument));
                }
            }

            method.UserData.Add("MethodInfo", (MethodInfo)source);

            return method;
        }

        public static explicit operator CodeMethodInvokeExpression(MethodReference source)
        {
            var method = new CodeMethodInvokeExpression();
            method.Method = (CodeMethodReferenceExpression)source;
            return method;
        }

        public static explicit operator MethodInfo(MethodReference source)
        {
            return source.typeArguments == null ? source.targetObject.GetMethod(source.methodName) : source.targetObject.GetMethod(source.methodName, source.typeArguments);
        }
    }
}
