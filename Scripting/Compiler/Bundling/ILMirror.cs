using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace IronAHK.Scripting
{
    partial class ILMirror : List<MethodInfo>
    {
        static OpCode [] one_byte_opcodes;
        static OpCode [] two_bytes_opcodes;
        
        internal const string Prefix = ".builtin_";
        internal const string StatConPrefix = Prefix+"pseudostatic_";
        
        public List<Type> Sources;
        public TypeBuilder Target;
        public ModuleBuilder Module;
        
        Dictionary<MethodBase, MethodBuilder> MethodsDone;
        Dictionary<ConstructorInfo, ConstructorBuilder> ConstructorsDone;
        Dictionary<FieldInfo, FieldBuilder> FieldsDone;
        Dictionary<Type, TypeBuilder> TypesDone;

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
            Sources = new List<Type>();
            
            MethodsDone = new Dictionary<MethodBase, MethodBuilder>();
            ConstructorsDone = new Dictionary<ConstructorInfo, ConstructorBuilder>();
            FieldsDone = new Dictionary<FieldInfo, FieldBuilder>();
            TypesDone = new Dictionary<Type, TypeBuilder>();
        }
    }
}

