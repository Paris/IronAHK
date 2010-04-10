using System;

[assembly: CLSCompliant(true)]

namespace IronAHK.Setup
{
    static partial class Program
    {
        static void Main(string[] args)
        {
            x64 = false;
            Build();
            x64 = true;
            Build();

#if DEBUG
            Console.Read();
#endif
        }
    }
}
