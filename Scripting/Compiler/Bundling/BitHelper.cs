using System;
using System.Reflection;
using System.Collections.Generic;

namespace IronAHK.Scripting
{
    partial class ILMirror
    {
        static class BitHelper
        {
            public static uint ReadUnsignedInteger(byte[] Bytes, ref int i)
            {
                uint Ret = BitConverter.ToUInt32(Bytes, ++i);
                i += 3;
                return Ret;
            }
            
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

