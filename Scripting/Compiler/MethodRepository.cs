using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace IronAHK.Scripting
{
    partial class MethodCollection : List<MethodInfo>
    {
        static OpCode [] one_byte_opcodes;
        static OpCode [] two_bytes_opcodes;
        
        public List<Type> Sources;
        public TypeBuilder Target;
        public ModuleBuilder Module;
        
        Dictionary<MethodBase, MethodBuilder> Done;

        static MethodCollection ()
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
            
            MethodBody Body = Original.GetMethodBody();

            CopyLocals(Gen, Body);
            
            byte[] Bytes = Original.GetMethodBody().GetILAsByteArray();
            
            for(int i = 0; i < Bytes.Length; i++)
                CopyOpcode(Bytes, ref i, Gen, Original.Module);
            
            Done.Add(Original, Builder);
            return Builder; 
        }
        
        void CopyLocals(ILGenerator Gen, MethodBody Body)
        {
            foreach(LocalVariableInfo Info in Body.LocalVariables)
                Gen.DeclareLocal(Info.LocalType, Info.IsPinned);
        }
        
        void CopyOpcode(byte[] Bytes, ref int i, ILGenerator Gen, Module Origin)
        {
            OpCode Code;
            if(Bytes[i] == 0xFE) Code = two_bytes_opcodes[Bytes[++i]];
            else Code = one_byte_opcodes[Bytes[i]];
                
            switch(Code.OperandType)
            {
                // If no argument, then re-emit the opcode
                case OperandType.InlineNone:
                {
                    Gen.Emit(Code);
                    break;
                }
                    
                // If argument is a method, re-emit the method reference
                case OperandType.InlineMethod:
                {
                    int Token = BitHelper.ReadInteger(Bytes, ref i);
                    MethodBase Base = Origin.ResolveMethod(Token);
                    
                    if(Base is MethodInfo)
                        Gen.Emit(Code, Base as MethodInfo);
                    else if(Base is ConstructorInfo)
                        Gen.Emit(Code, Base as ConstructorInfo);
                    else throw new InvalidOperationException("Inline method is neither method nor constructor.");
                    
                    break;
                }
                    
                case OperandType.InlineField:
                {
                    int Token = BitHelper.ReadInteger(Bytes, ref i);
                    FieldInfo Field = Origin.ResolveField(Token);
                    
                    Gen.Emit(Code, Field);
                    break;
                }
                    
                case OperandType.InlineType:
                {
                    int Token = BitHelper.ReadInteger(Bytes, ref i);
                    Type Ref = Origin.ResolveType(Token);
                    Gen.Emit(Code, Ref);
                    break;
                }
                    
                case OperandType.InlineString:
                {
                    int Token = BitHelper.ReadInteger(Bytes, ref i);
                    string Copy = Origin.ResolveString(Token);
                    Gen.Emit(Code, Copy);
                    break;
                }
                    
                case OperandType.InlineSig:
                {
                    int Token = BitHelper.ReadInteger(Bytes, ref i);
                    byte[] Sig = Module.ResolveSignature(Token);
                    break;
                }
                    
                case OperandType.InlineTok:
                {
                    int Token = BitHelper.ReadInteger(Bytes, ref i);
                    MemberInfo Info = Origin.ResolveMember(Token);
                    
                    if(Info.MemberType == MemberTypes.Field)
                        Gen.Emit(OpCodes.Ldtoken, Info as FieldInfo);
                    else if(Info.MemberType == MemberTypes.Method)
                        Gen.Emit(OpCodes.Ldtoken, Info as MethodInfo);
                    else if(Info.MemberType == MemberTypes.TypeInfo)
                        Gen.Emit(OpCodes.Ldtoken, Info as Type);
                    else throw new InvalidOperationException("Inline token is neither field, nor method, nor type");
                    
                    break;
                }
                    
                case OperandType.ShortInlineBrTarget:
                case OperandType.ShortInlineI:
                case OperandType.ShortInlineVar:
                {
                    Gen.Emit(Code, Bytes[++i]);
                    break;
                }
                    
                case OperandType.InlineVar:
                {
                    Gen.Emit(Code, BitHelper.ReadShort(Bytes, ref i));
                    break;
                }
                    
                case OperandType.InlineSwitch:
                case OperandType.InlineBrTarget:
                case OperandType.InlineI:
                case OperandType.ShortInlineR: // This is actualy a float, but we don't care
                {
                    Gen.Emit(Code, BitHelper.ReadInteger(Bytes, ref i));
                    break;
                }
                    
                case OperandType.InlineI8:
                {
                    Gen.Emit(Code, BitHelper.ReadLong(Bytes, ref i));
                    break;
                }
                    
                case OperandType.InlineR:
                {
                    Gen.Emit(Code, BitHelper.ReadDouble(Bytes, ref i));
                    break;
                }     
                    
                // TODO: OperandType.InlineTok, OperandType.InlineSig
                
                default:
                    throw new InvalidOperationException("The method copier ran across an unknown opcode.");
            }
        }
        
        static Type[] ParameterTypes(MethodInfo Original)
        {
            ParameterInfo[] Params = Original.GetParameters();
            Type[] Ret = new Type[Params.Length];
            
            for(int i = 0; i < Params.Length; i++)
                Ret[i] = Params[i].ParameterType;
            
            return Ret;
        }

        static class BitHelper
        {
            public static int ReadInteger(byte[] Bytes, ref int i)
            {
                int Ret = BitConverter.ToInt32(Bytes, ++i);
                i += 3;
                return Ret;
            }
            
            public static long ReadLong(byte[] Bytes, ref int i)
            {
                long Ret = BitConverter.ToInt64(Bytes, ++i);
                i += 7;
                return Ret;
            }
            
            public static float ReadFloat(byte[] Bytes, ref int i)
            {
                float Ret = BitConverter.ToSingle(Bytes, ++i);
                i += 3;
                return Ret;
            }
            
            public static double ReadDouble(byte[] Bytes, ref int i)
            {
                double Ret = BitConverter.ToDouble(Bytes, ++i);
                i += 7;
                return Ret;
            }
            
            public static short ReadShort(byte[] Bytes, ref int i)
            {
                short Ret = BitConverter.ToInt16(Bytes, ++i);
                i++;
                return Ret;
            }
        }        
    }
}

