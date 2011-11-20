using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace IronAHK.Scripting
{
    partial class ILMirror
    {
        bool HasBody(MethodBase Base)
        {
            if(Base.IsAbstract)
                return false;
            
            MethodImplAttributes Attr = Base.GetMethodImplementationFlags();
            if((Attr & MethodImplAttributes.Runtime) == MethodImplAttributes.Runtime &&
               (Attr & MethodImplAttributes.Managed) == MethodImplAttributes.Managed)
                return false;
            
            return true;
        }
        
        void CopyMethodBody(MethodBase Base, MethodBuilder Builder)
        {
            if(HasBody(Base))
                CopyMethodBody(Base, Builder.GetILGenerator());
        }
        
        void CopyMethodBody(MethodBase Base, ConstructorBuilder Builder)
        {
            if(HasBody(Base))
                CopyMethodBody(Base, Builder.GetILGenerator());
        }
        
        void CopyMethodBody(MethodBase Base, ILGenerator Gen)
        {
            MethodBody Body = Base.GetMethodBody();
            
            CopyLocals(Gen, Body);
            
            byte[] Bytes = Body.GetILAsByteArray();
            var ExceptionTrinkets = new List<int>();
            var LabelTargets = new Dictionary<int, Label>();
            var LabelOrigins = new Dictionary<int, Label[]>();
            
            MineExTrinkets(Body, ExceptionTrinkets);
            
            // There's a reason we mine these labels. First of all, we need to get the switchmaps. Because
            // there is no way to bang a number of bytes in the IL, we mine the targets of a switch operation
            // and save them to be marked when we walk through the method again to copy the opcodes. Secondly, 
            // the microsoft C# compiler sometimes uses the leave.s opcode instead of the leave opcode at the
            // end of a try block. This is all fine, but System.Reflection.Emit forces a leave opcode on the
            // IL when we call BeginCatchBlock() and friends, offering no way to use the leave.s opcode instead.
            // The simple result is that we are left with putting the leave instruction with its 4-byte 
            // argument in the IL against our will. This screws over all branch targets with an offset of +3
            // bytes. Consequently, we have to mine *all* branch targets and re-mark them to prevent segfaults
            // and the like in the runtime. This overhead could all have been avoided, had SRE given us just 
            // a tiny bit more of control over the IL that was to be emitted.
            MineLabels(Bytes, Gen, LabelOrigins, LabelTargets);
            
            for(int i = 0; i < Bytes.Length; i++)
            {
                CopyTryCatch(Gen, i, Body, ExceptionTrinkets);
                CopyLabels(Gen, i, LabelTargets);
                CopyOpcode(Bytes, ref i, Gen, Base.Module, ExceptionTrinkets, LabelOrigins);
            }
            
            // If we do not throw this exception, SRE will do it, but with much less debugging information.
            foreach(int i in LabelTargets.Keys)
                throw new Exception("Unmarked label destined for RVA "+i.ToString("X"));
        }
        
        // Initialise the variables. TODO: Obey InitLocals
        void CopyLocals(ILGenerator Gen, MethodBody Body)
        {
            foreach(LocalVariableInfo Info in Body.LocalVariables) 
                Gen.DeclareLocal(GrabType(Info.LocalType), Info.IsPinned);
        }
        
        void CopyOpcode(byte[] Bytes, ref int i, ILGenerator Gen, Module Origin, List<int> ExceptionTrinkets, Dictionary<int, Label[]> LabelOrigins)
        {
            OpCode Code = GetOpcode(Bytes, ref i);
            
            // These are emitted by exception handling copier if an exception 
            // block is imminent. If not, copy them as usual.
            if(Code == OpCodes.Leave && ExceptionTrinkets.Contains(i + 5)) 
            {
                i += 4;
                return;
            }
            else if(Code == OpCodes.Leave_S && ExceptionTrinkets.Contains(i + 2))
            {
                // This is a rather tricky one. See the comment preceding the call to MineLabels above.
                i++;
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
                    Gen.Emit(Code, GrabField(Field));
                    break;
                }
                    
                // Argument is a type reference
                case OperandType.InlineType:
                {
                    int Token = BitHelper.ReadInteger(Bytes, ref i);
                    Type Ref = Origin.ResolveType(Token);
                    Gen.Emit(Code, GrabType(Ref));
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
                    if(!LabelOrigins.ContainsKey(i))
                        throw new Exception("No switchmap found for RVA "+i.ToString("X"));
                        
                    Label[] Labels = LabelOrigins[i];
                    i += 4 + Labels.Length*4;
                    Gen.Emit(Code, Labels);
                    
                    break;
                }
                    
                // Argument is a single-byte branch target
                case OperandType.ShortInlineBrTarget:
                {
                    if(!LabelOrigins.ContainsKey(i))
                        throw new Exception("No label origin found for RVA "+i.ToString("X"));

                    // messy fix to convert short branch targets to normal ones, since there's no easy way to calculate offsets via reflection
                    const string s = ".s";
                    string name = Code.Name;
                    if (name.EndsWith(s))
                    {
                        name = name.Substring(0, name.Length - s.Length);
                        foreach (var field in typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static))
                        {
                            var opcode = (OpCode)field.GetValue(null);
                            if (opcode.Name.Equals(name))
                            {
                                Code = opcode;
                                break;
                            }
                        }
                    }

                    Gen.Emit(Code, LabelOrigins[i][0]);
                    i++;
                    
                    break;
                }
                    
                // Argument is a byte
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
                    
                case OperandType.InlineBrTarget:
                {
                    if(!LabelOrigins.ContainsKey(i))
                        throw new Exception("No label origin found for RVA "+i.ToString("X"));
                    
                    Gen.Emit(Code, LabelOrigins[i][0]);
                    i += 4;
                    
                    break;
                }
                    
                // Argument is a 32-bit integer
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

