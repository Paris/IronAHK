using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace IronAHK.Scripting
{
    partial class ILMirror
    {
        void MineLabels(byte[] Bytes, ILGenerator Gen, Dictionary<int, Label[]> LabelOrigins, Dictionary<int, Label> LabelTargets)
        {
            for(int i = 0; i < Bytes.Length; i++)
            {
                OpCode Code = GetOpcode(Bytes, ref i);
                int Start = i;
                
                // Reference: http://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.switch(VS.80).aspx
                if(Code == OpCodes.Switch) 
                {
                    int Count = (int) BitHelper.ReadUnsignedInteger(Bytes, ref i);
                    
                    // Jumps are relative to the first byte of the instruction following the switchmap
                    int Zero = i+Count*4+1; 
                    List<Label> Labels = new List<Label>();
                    
                    for(int j = 0; j < Count; j++)
                    {
                        int Absolute = Zero+BitHelper.ReadInteger(Bytes, ref i);
                        
                        if(!LabelTargets.ContainsKey(Absolute))
                        {
                            Label At = Gen.DefineLabel();
                            LabelTargets.Add(Absolute, At);
                            Labels.Add(At);
                        }
                        // If there is a label already defined for this position, reuse that for this switchmap too
                        else Labels.Add(LabelTargets[Absolute]); 
                    }
                    
                    LabelOrigins.Add(Start, Labels.ToArray());
                }
                else if(Code.OperandType == OperandType.InlineBrTarget || 
                        Code.OperandType == OperandType.ShortInlineBrTarget)
                {
                    int Zero, Target;
                    
                    if(Code.OperandType == OperandType.InlineBrTarget)
                    {
                        Zero = i+5;
                        Target = Zero+BitHelper.ReadInteger(Bytes, ref i);
                    }
                    else 
                    {
                        Zero = i+2;
                        Target = Zero+((sbyte)Bytes[++i]);
                    }
                    
                    if(!LabelTargets.ContainsKey(Target))
                    {
                        Label At = Gen.DefineLabel();
                        LabelTargets.Add(Target, At);
                        LabelOrigins.Add(Start, new Label[] { At });
                    }
                    else LabelOrigins.Add(Start, new Label[] { LabelTargets[Target] });
                }
                else i += CodeArgumentSize(Code);
            }
        }
        
        void CopyLabels(ILGenerator Gen, int i, Dictionary<int, Label> LateLabels)
        {
            if(!LateLabels.ContainsKey(i)) return;
            
            Gen.MarkLabel(LateLabels[i]);
            LateLabels.Remove(i);
        }
        
        int CodeArgumentSize(OpCode Code)
        {
            switch(Code.OperandType)
            {
                case OperandType.InlineNone:
                    return 0;
                    
                // All tokens are 32-bit integers
                case OperandType.InlineMethod:
                case OperandType.InlineField:
                case OperandType.InlineType:
                case OperandType.InlineString:
                case OperandType.InlineTok:
                case OperandType.InlineBrTarget:
                case OperandType.InlineI:
                case OperandType.ShortInlineR: 
                    return 4;
                    
                // Variable length
                case OperandType.InlineSwitch:
                    throw new ArgumentException("InlineSwitch has variable argument length", "Code");
                    
                // Argument is a byte
                case OperandType.ShortInlineBrTarget:
                case OperandType.ShortInlineI:
                case OperandType.ShortInlineVar:
                    return 1;
                
                // Argument is a short
                case OperandType.InlineVar:
                    return 2;
                    
                // Argument is a 64-bit integer
                case OperandType.InlineI8:
                case OperandType.InlineR:
                    return 8;
                
                default:
                    throw new InvalidOperationException("Could not determine argument size for opcode "+Code);
            }
        }        
    }
}
    