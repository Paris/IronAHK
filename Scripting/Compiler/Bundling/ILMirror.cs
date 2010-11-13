using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace IronAHK.Scripting
{
    partial class ILMirror 
    {
        static OpCode [] one_byte_opcodes;
        static OpCode [] two_bytes_opcodes;
        
        public List<Module> Sources;
        
        public ModuleBuilder Module {
            get { return mModule; }
            set {
                mModule = value;
                ImplementationDetails = value.DefineType("<CopiedImplementationDetails>");
            }
        }
        
        ModuleBuilder mModule;
        TypeBuilder ImplementationDetails;
        
        Dictionary<MethodBase, MethodBuilder> MethodsDone;
        Dictionary<ConstructorInfo, ConstructorBuilder> ConstructorsDone;
        Dictionary<FieldInfo, FieldBuilder> FieldsDone;
        Dictionary<Type, TypeBuilder> TypesDone;
        Dictionary<FieldInfo, FieldInfo> BackingFields;
        Dictionary<PropertyInfo, PropertyBuilder> PropertiesDone;
        
        static ILMirror ()
        {
            // Courtesy of Jb Evain, http://github.com/jbevain/mono.reflection
            one_byte_opcodes = new OpCode [0xe1];
            two_bytes_opcodes = new OpCode [0x1f];

            var fields = typeof (OpCodes).GetFields (
                BindingFlags.Public | BindingFlags.Static);

            for (int i = 0; i < fields.Length; i++) {
                var opcode = (OpCode) fields [i].GetValue (null);
                if (opcode.OpCodeType == OpCodeType.Nternal)
                    continue;

                if (opcode.Size == 1)
                    one_byte_opcodes [opcode.Value] = opcode;
                else
                    two_bytes_opcodes [opcode.Value & 0xff] = opcode;
            }
        }
        
        public ILMirror()
        {
            Sources = new List<Module>();
            
            MethodsDone = new Dictionary<MethodBase, MethodBuilder>();
            ConstructorsDone = new Dictionary<ConstructorInfo, ConstructorBuilder>();
            FieldsDone = new Dictionary<FieldInfo, FieldBuilder>();
            TypesDone = new Dictionary<Type, TypeBuilder>();
            PropertiesDone = new Dictionary<PropertyInfo, PropertyBuilder>();
            
            BackingFields = new Dictionary<FieldInfo, FieldInfo>();
        }
        
        public void Complete()
        {
            foreach(TypeBuilder T in TypesDone.Values)
                T.CreateType();
            
            ImplementationDetails.CreateType();
        }
        
        static OpCode GetOpcode(byte[] Bytes, ref int i)
        {
            if(Bytes[i] == 0xFE) return two_bytes_opcodes[Bytes[++i]];
            else return one_byte_opcodes[Bytes[i]];
        }
        
        static T FindCustomAttribute<T>(MemberInfo Info) where T : Attribute
        {
            foreach(object Attr in Info.GetCustomAttributes(false))
            {
                if(Attr is T)
                    return Attr as T;
            }
            
            return null;
        }
    }
}

