using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace IronAHK.Rusty.Common
{
    partial class Mapper
    {
        /// <summary>
        /// Maps DriveType's
        /// </summary>
        internal class DriveTypeMapper : MapperBase<DriveType>
        {

            public override void SetUpMappingTable()
            {
                clrMappingTable.Add(DriveType.CDRom, Core.Keyword_CDROM);
                clrMappingTable.Add(DriveType.Fixed, Core.Keyword_FIXED);
                clrMappingTable.Add(DriveType.Network, Core.Keyword_NETWORK);
                clrMappingTable.Add(DriveType.Ram, Core.Keyword_RAMDISK);
                clrMappingTable.Add(DriveType.Removable, Core.Keyword_REMOVABLE);
                clrMappingTable.Add(DriveType.Unknown, Core.Keyword_UNKNOWN);
                clrMappingTable.Add(DriveType.NoRootDirectory, Core.Keyword_UNKNOWN);
            }

            public override string LookUpIAType(DriveType clrType)
            {
                var str = base.LookUpIAType(clrType);
                if (string.IsNullOrEmpty(str))
                    str = Core.Keyword_UNKNOWN;
                return str;
            }

            public override DriveType? LookUpCLRType(string keyword)
            {
                return base.LookUpCLRType(keyword);
            }
        }
    }
}
