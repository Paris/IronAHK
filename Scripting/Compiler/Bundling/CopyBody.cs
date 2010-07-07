using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace IronAHK.Scripting
{
    partial class ILMirror
    {
        void CopyMethodBody(MethodBody Body, ILGenerator Gen, Module Origin)
        {
            CopyLocals(Gen, Body);
            
            byte[] Bytes = Body.GetILAsByteArray();
            var ExceptionTrinkets = new List<int>();
            var LateLabels = new Dictionary<int, Label>();
            var SwitchMaps = new Dictionary<int, Label[]>();
            
            MineExTrinkets(Body, ExceptionTrinkets);
            MineSwitchMaps(Bytes, Gen, SwitchMaps, LateLabels);
            
            for(int i = 0; i < Bytes.Length; i++)
            {
                CopyTryCatch(Gen, i, Body, ExceptionTrinkets);
                CopyLabels(Gen, i, LateLabels);
                CopyOpcode(Bytes, ref i, Gen, Origin, ExceptionTrinkets, SwitchMaps);
            }
        }
        
        // Initialise the variables. TODO: Obey InitLocals
        void CopyLocals(ILGenerator Gen, MethodBody Body)
        {
            foreach(LocalVariableInfo Info in Body.LocalVariables) 
                Gen.DeclareLocal(Info.LocalType, Info.IsPinned);
        }
        
        void CopyOpcode(byte[] Bytes, ref int i, ILGenerator Gen, Module Origin, List<int> ExceptionTrinkets, Dictionary<int, Label[]> SwitchMaps)
        {
            OpCode Code = GetOpcode(Bytes, ref i);
            
            // These are emitted by exception handling copier if an exception 
            // block is imminent. If not, copy them as usual.
            if(Code == OpCodes.Leave && ExceptionTrinkets.Contains(i + 5)) 
            {
                i += 4;
                return;
            }
            else if(Code == OpCodes.Endfinally && ExceptionTrinkets.Contains(i+1)) return; 
            
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
                        Gen.Emit(Code, GrabMethod(Base as MethodInfo));
                    else if(Base is ConstructorInfo)
                        Gen.Emit(Code, GrabConstructor(Base as ConstructorInfo));
                    else throw new InvalidOperationException("Inline method is neither method nor constructor.");
                    
                    break;
                }
                    
                // Argument is a field reference
                case OperandType.InlineField:
                {
                    int Token = BitHelper.ReadInteger(Bytes, ref i);
                    FieldInfo Field = Origin.ResolveField(Token);
                    
                    if(Sources.Contains(Field.DeclaringType.DeclaringType))
                        Field = GrabField(Field, GrabType(Field.DeclaringType) as TypeBuilder);
                    else if(Sources.Contains(Field.DeclaringType))
                        Field = GrabField(Field);
                    
                    Gen.Emit(Code, Field);
                    break;
                }
                    
                // Argument is a type reference
                case OperandType.InlineType:
                {
                    int Token = BitHelper.ReadInteger(Bytes, ref i);
                    Type Ref = Origin.ResolveType(Token);
                    Gen.Emit(Code, Ref);
                    break;
                }
                    
                // Argument is an inline string
                case OperandType.InlineString:
                {
                    int Token = BitHelper.ReadInteger(Bytes, ref i);
                    string Copy = Origin.ResolveString(Token);
                    Gen.Emit(Code, Copy);
                    break;
                }
                 
                // Argument is a metadata token
                case OperandType.InlineTok:
                {
                    int Token = BitHelper.ReadInteger(Bytes, ref i);
                    MemberInfo Info = Origin.ResolveMember(Token);
                    
                    if(Info.MemberType == MemberTypes.Field)
                    {
                        if(Code != OpCodes.Ldtoken || !TryReplaceBackingField(Bytes, i, Gen, Origin))
                            Gen.Emit(Code, GrabField(Info as FieldInfo));
                    }
                    else if(Info.MemberType == MemberTypes.Method)
                        Gen.Emit(Code, GrabMethod(Info as MethodInfo));
                    else if(Info.MemberType == MemberTypes.TypeInfo || Info.MemberType == MemberTypes.NestedType)
                        Gen.Emit(Code, GrabType(Info as Type));
                    else throw new InvalidOperationException("Inline token is neither field, nor method, nor type");
                    
                    break;
                }
                    
                // Argument is a switch map
                case OperandType.InlineSwitch:
                {
                    if(!SwitchMaps.ContainsKey(i))
                        throw new Exception("No switchmap found for RVA "+i.ToString("X"));
                        
                    Label[] Labels = SwitchMaps[i];
                    i += 4 + Labels.Length*4;
                    Gen.Emit(Code, Labels);
                    
                    break;
                }
                    
                // Argument is a byte
                case OperandType.ShortInlineBrTarget:
                case OperandType.ShortInlineI:
                case OperandType.ShortInlineVar:
                {
                    Gen.Emit(Code, Bytes[++i]);
                    break;
                }
                
                // Argument is a short
                case OperandType.InlineVar:
                {
                    Gen.Emit(Code, BitHelper.ReadShort(Bytes, ref i));
                    break;
                }
                    
                // Argument is a 32-bit integer
                case OperandType.InlineBrTarget:
                case OperandType.InlineI:
                case OperandType.ShortInlineR: // This is actually a 32-bit float, but we don't care
                {
                    Gen.Emit(Code, BitHelper.ReadInteger(Bytes, ref i));
                    break;
                }
                    
                // Argument is a 64-bit integer
                case OperandType.InlineI8:
                {
                    Gen.Emit(Code, BitHelper.ReadLong(Bytes, ref i));
                    break;
                }
                    
                // Argument is a 64-bit float
                case OperandType.InlineR:
                {
                    Gen.Emit(Code, BitHelper.ReadDouble(Bytes, ref i));
                    break;
                }     
                
                // If ever we run across OpCodes.Calli this'll probably happen
                default:
                    throw new InvalidOperationException("The method copier ran across an unknown opcode.");
            }
        }    
        
        bool TryReplaceBackingField(byte[] Bytes, int i, ILGenerator Gen, Module Origin)
        {
            // With a bit of clairvoyance we try to determine if we're dealing with
            // a specific type of array initializer that the C#-compiler uses.
            if(Bytes[i+1] != (byte) OpCodes.Call.Value)
                return false;
            
            if(Bytes[i+6] != (byte) OpCodes.Stsfld.Value)
                return false;
            
            i += 6;
            int Token = BitHelper.ReadInteger(Bytes, ref i);
            FieldInfo Info = Origin.ResolveField(Token);
            
            FieldInfo Ours = GrabField(Info);
            Gen.Emit(OpCodes.Ldtoken, BackingFields[Ours]);
            
            return true;
        }
    }
}

