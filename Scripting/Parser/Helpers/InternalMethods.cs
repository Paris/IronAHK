using System;
using IronAHK.Rusty;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        internal class InternalMethods
        {
            #region Variables

            public static MethodReference GetEnv
            {
                get { return new MethodReference(typeof(Core), "GetEnv"); }
            }

            public static MethodReference SetEnv
            {
                get { return new MethodReference(typeof(Core), "SetEnv"); }
            }

            #endregion

            #region Hotkeys

            public static MethodReference Hotkey
            {
                get { return new MethodReference(typeof(Core), "Hotkey", new Type[] { typeof(string), typeof(string), typeof(string) }); }
            }

            public static MethodReference Hotstring
            {
                get { return new MethodReference(typeof(Core), "Hotstring", new Type[] { typeof(string), typeof(string) }); }
            }

            public static MethodReference Send
            {
                get { return new MethodReference(typeof(Core), "Send", new Type[] { typeof(string) }); }
            }

            public static MethodReference LabelCall
            {
                get { return new MethodReference(typeof(Script), "LabelCall", new Type[] { typeof(string) }); }
            }

            #endregion

            #region Loops

            public static MethodReference Loop
            {
                get { return new MethodReference(typeof(Core), "Loop"); }
            }

            public static MethodReference LoopParse
            {
                get { return new MethodReference(typeof(Core), "LoopParse"); }
            }

            public static MethodReference LoopRead
            {
                get { return new MethodReference(typeof(Core), "LoopRead"); }
            }

            public static MethodReference LoopFile
            {
                get { return new MethodReference(typeof(Core), "LoopFile"); }
            }

            public static MethodReference LoopRegistry
            {
                get { return new MethodReference(typeof(Core), "LoopRegistry"); }
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

            public static MethodReference FunctionCall
            {
                get { return new MethodReference(typeof(Script), "FunctionCall"); }
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
