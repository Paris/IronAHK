using System;
using System.CodeDom;
using System.Reflection.Emit;

namespace IronAHK.Scripting
{
    partial class Compiler
    {
        void EmitAttribute(AssemblyBuilder parent, CodeAttributeDeclaration attribute)
        {
            var type = (Type)attribute.AttributeType.UserData[Parser.RawData];
            var vals = new object[attribute.Arguments.Count];
            var args = new Type[vals.Length];

            for (int i = 0; i < vals.Length; i++)
            {
                vals[i] = ((CodePrimitiveExpression)attribute.Arguments[i].Value).Value;
                args[i] = vals[i].GetType();
            }

            var ctor = type.GetConstructor(args);
            var builder = new CustomAttributeBuilder(ctor, vals);
            parent.SetCustomAttribute(builder);
        }
    }
}
