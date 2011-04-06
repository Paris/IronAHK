using System;
using System.Collections.Generic;
using System.Text;

namespace IronAHK.Rusty.Common
{
    partial class Mapper
    {
        internal class MapperBase<T> where T : struct, IConvertible
        {
            static protected Dictionary<T, string> clrMappingTable = new Dictionary<T, string>();

            public MapperBase()
            {
                SetUpMappingTable();
            }

            public virtual T? LookUpCLRType(string keyword)
            {
                T? res = null;
                foreach (var kv in clrMappingTable)
                {
                    if (kv.Value == keyword)
                    {
                        res = kv.Key;
                        break;
                    }
                }
                return res;
            }

            public virtual void SetUpMappingTable()
            {
                //
            }

            public virtual string LookUpIAType(T clrType)
            {
                if (clrMappingTable.ContainsKey(clrType))
                    return clrMappingTable[clrType];
                else
                    return "";
            }
        }
    }
}
