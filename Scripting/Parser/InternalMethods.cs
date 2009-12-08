using System;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        internal class InternalMethods
        {
            #region Variables

            public static MethodReference GetEnv
            {
                get { return new MethodReference(typeof(Rusty.Core), "GetEnv"); }
            }

            public static MethodReference SetEnv
            {
                get { return new MethodReference(typeof(Rusty.Core), "SetEnv"); }
            }

            #endregion

            #region Loops

            public static MethodReference Loop
            {
                get { return new MethodReference(typeof(Rusty.Core), "Loop"); }
            }

            public static MethodReference LoopParse
            {
                get { return new MethodReference(typeof(Rusty.Core), "LoopParse"); }
            }

            public static MethodReference LoopRead
            {
                get { return new MethodReference(typeof(Rusty.Core), "LoopRead"); }
            }

            public static MethodReference LoopFile
            {
                get { return new MethodReference(typeof(Rusty.Core), "LoopFile"); }
            }

            public static MethodReference LoopRegistry
            {
                get { return new MethodReference(typeof(Rusty.Core), "LoopRegistry"); }
            }

            #endregion

            #region Operators

            public static MethodReference Operate
            {
                get { return new MethodReference(typeof(Script), "Operate"); }
            }

            public static MethodReference IfElse
            {
                get { return new MethodReference(typeof(Script), "IfTest"); }
            }

            public static MethodReference Parameters
            {
                get { return new MethodReference(typeof(Script), "Parameters"); }
            }

            #endregion

            #region Misc

            public static MethodReference Concat
            {
                get { return new MethodReference(typeof(string), "Concat", new Type[] { typeof(string[]) }); }
            }

            #endregion
        }
    }
}
