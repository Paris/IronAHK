using IronAHK.Rusty;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        internal class InternalMethods
        {
            #region Hotkeys

            public static MethodReference Hotkey
            {
                get { return new MethodReference(typeof(Core), "Hotkey", new[] { typeof(string), typeof(string), typeof(string) }); }
            }

            public static MethodReference Hotstring
            {
                get { return new MethodReference(typeof(Core), "Hotstring", new[] { typeof(string), typeof(string), typeof(string) }); }
            }

            public static MethodReference Send
            {
                get { return new MethodReference(typeof(Core), "Send", new[] { typeof(string) }); }
            }

            public static MethodReference LabelCall
            {
                get { return new MethodReference(typeof(Script), "LabelCall", new[] { typeof(string) }); }
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

            public static MethodReference LoopEach
            {
                get { return new MethodReference(typeof(Core), "LoopEach"); }
            }

            #endregion

            #region Operators

            public static MethodReference Operate
            {
                get { return new MethodReference(typeof(Script), "Operate"); }
            }

            public static MethodReference OperateTernary
            {
                get { return new MethodReference(typeof(Script), "OperateTernary"); }
            }

            public static MethodReference OperateUnary
            {
                get { return new MethodReference(typeof(Script), "OperateUnary"); }
            }

            public static MethodReference OperateZero
            {
                get { return new MethodReference(typeof(Script), "OperateZero"); }
            }

            public static MethodReference IfElse
            {
                get { return new MethodReference(typeof(Script), "IfTest"); }
            }

            public static MethodReference IfLegacy
            {
                get { return new MethodReference(typeof(Script), "IfLegacy"); }
            }

            public static MethodReference Parameters
            {
                get { return new MethodReference(typeof(Script), "Parameters"); }
            }

            public static MethodReference FunctionCall
            {
                get { return new MethodReference(typeof(Script), "FunctionCall"); }
            }

            public static MethodReference Invoke
            {
                get { return new MethodReference(typeof(Script), "Invoke"); }
            }

            #endregion

            #region Objects

            public static MethodReference Index
            {
                get { return new MethodReference(typeof(Script), "Index"); }
            }

            public static MethodReference Dictionary
            {
                get { return new MethodReference(typeof(Script), "Dictionary"); }
            }

            public static MethodReference SetObject
            {
                get { return new MethodReference(typeof(Script), "SetObject"); }
            }

            public static MethodReference ExtendArray
            {
                get { return new MethodReference(typeof(Script), "ExtendArray"); }
            }

            #endregion

            #region Misc

            public static MethodReference CreateTrayMenu
            {
                get { return new MethodReference(typeof(Script), "CreateTrayMenu"); }
            }

            public static MethodReference Run
            {
                get { return new MethodReference(typeof(Script), "Run"); }
            }

            public static MethodReference Exit
            {
                get { return new MethodReference(typeof(Core), "Exit", new[] { typeof(int) }); }
            }

            public static MethodReference Concat
            {
                get { return new MethodReference(typeof(string), "Concat", new[] { typeof(string[]) }); }
            }

            #endregion
        }
    }
}
