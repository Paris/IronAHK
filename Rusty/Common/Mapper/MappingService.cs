using System;
using System.Collections.Generic;
using System.Text;

namespace IronAHK.Rusty.Common
{
    partial class Mapper
    {
        public sealed class MappingService
        {
            static readonly MappingService instance = new MappingService();

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static MappingService() { }

            MappingService()
            {
                // add here all mapping providers here
                DriveType = new DriveTypeMapper();
            }


            internal readonly DriveTypeMapper DriveType;

            public static MappingService Instance
            {
                get { return instance; }
            }
        }
    }
}
