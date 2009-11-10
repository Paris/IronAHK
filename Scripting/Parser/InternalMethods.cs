using System;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        internal class InternalMethods
        {
            public static MethodReference GetEnv
            {
                get { return new MethodReference(typeof(Rusty.Core), "GetEnv"); }
            }

            public static MethodReference SetEnv
            {
                get { return new MethodReference(typeof(Rusty.Core), "SetEnv"); }
            }

            public static MethodReference Operate
            {
                get { return new MethodReference(typeof(Script), "Operate"); }
            }

            public static MethodReference Concat
            {
                get { return new MethodReference(typeof(string), "Concat", new Type[] { typeof(string[]) }); }
            }
        }
    }
}
