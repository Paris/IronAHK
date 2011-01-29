
namespace IronAHK.Scripting
{
    partial class Parser
    {
        #region Generic

        const char CR = '\r';
        const char LF = '\n';
        internal const char SingleSpace = ' ';
        const char Reserved = '\0';
        readonly char[] Spaces = { CR, LF, SingleSpace, '\t', '\xA0' };

        public const string RawData = "raw";

        const string LibEnv = "AHKLIBPATH";
        const string LibDir = "lib";
        const string LibExt = "ahk";
        const char LibSeperator = '_';

        internal const char StringBound = '"';
        internal const char ParenOpen = '(';
        internal const char ParenClose = ')';
        internal const char BlockOpen = '{';
        internal const char BlockClose = '}';
        internal const char ArrayOpen = '[';
        internal const char ArrayClose = ']';
        internal const char MultiComA = '/';
        internal const char MultiComB = '*';
        internal const char TernaryA = '?';
        internal const char TernaryB = ':';
        internal const char HotkeyBound = ':';
        internal const string HotkeySignal = "::";
        internal const char Directive = '#';

#if !LEGACY
        internal const char LastVar = '$';
#endif

        internal const char DefaultEscpe = '`';
        internal const char DefaultComment = ';';
        internal const char DefaultResolve = '%';
        internal const char DefaultMulticast = ',';

#if !LEGACY
        const
#endif
        char Escape = DefaultEscpe;

#if !LEGACY
        const char Comment = DefaultComment;
#endif
        string Comment = DefaultComment.ToString();

#if !LEGACY
        const
#endif
        internal char Resolve = DefaultResolve;

#if !LEGACY
        const
#endif
        internal char Multicast = DefaultMulticast;

#if LEGACY
        internal const string VarExt = "#_@$?"; // []
#endif
#if !LEGACY
        internal const string VarExt = "#_$";
#endif

        #endregion

        #region Operators

        //internal const char Power = "**";
        internal const char Multiply = '*';
        internal const char Divide = '/';
        //internal const string FloorDivide = "//";
        internal const char Add = '+';
        internal const char Subtract = '-';
        internal const char BitAND = '&';
        //internal const string And = "&&";
        internal const char BitXOR = '^';
        internal const char BitOR = '|';
        //internal const string Or = "||";
        internal const char BitNOT = '~';
        internal const char Concatenate = '.';
        internal const char Greater = '>';
        internal const char Less = '<';
        //internal const string BitShiftLeft = "<<";
        //internal const string BitShiftRight = ">>";
        //internal const string GreaterOrEqual = ">=";
        //internal const string LessOrEqual = "<=";
        internal const char Equal = '=';
        internal const char AssignPre = ':';
        //internal const string CaseSensitiveEqual = "==";
        //internal const string NotEqual = "!=";

        internal const string AndTxt = "and";
        internal const string OrTxt = "or";
        internal const string NotTxt = "not";
        internal const string TrueTxt = "true";
        internal const string FalseTxt = "false";
        internal const string NullTxt = "null";

        internal const string BetweenTxt = "between";
        internal const string InTxt = "in";
        internal const string ContainsTxt = "contains";
        internal const string IsTxt = "is";

        internal const char Minus = '-';
        internal const char Not = '!';
        internal const char Address = '&';
        internal const char Dereference = '*';

        internal const string ErrorLevel = "ErrorLevel";

        #region Assignments

        //readonly string AssignEqual = ":" + Equal;
        //readonly string AssignAdd = "+" + Equal;
        //readonly string AssignSubtract = "-" + Equal;
        //readonly string AssignMultiply = "*=" + Equal;
        //readonly string AssignDivide = "/" + Equal;
        //readonly string AssignFloorDivide = "//" + Equal;
        //readonly string AssignConcatenate = "." + Equal;
        //readonly string AssignBitOR = "|" + Equal;
        //readonly string AssignBitAND = "&" + Equal;
        //readonly string AssignBitXOR = "^" + Equal;
        //readonly string AssignShiftLeft = "<<" + Equal;
        //readonly string AssignShiftRight = ">>" + Equal;

        #endregion

        #endregion

        #region Words

        const string MsgBox = "msgbox";

        #region Flow

        internal const string FlowBreak = "break";
        internal const string FlowContinue = "continue";
        internal const string FlowElse = "else";
        internal const string FlowGosub = "gosub";
        internal const string FlowGoto = "goto";
        internal const string FlowIf = "if";
        internal const string FlowLoop = "loop";
        internal const string FlowReturn = "return";
        internal const string FlowWhile = "while";

        #endregion

        #region Functions

        internal const string FunctionLocal = "local";
        internal const string FunctionGlobal = "global";
        internal const string FunctionStatic = "static";
        internal const string FunctionParamRef = "byref";

        #endregion

        #region Directives

        const string DirvCommentFlag = "commentflag";
        const string DirvEscapeChar = "escapechar";
        const string DirvInclude = "include";
        const string DirvIncludeAgain = "includeagain";

        #endregion

        #region Multiline string

        const string LTrim = "ltrim";
        const string RTrim = "rtrim";
        const string Join = "join";
        const string Comments0 = "comments";
        const string Comments1 = "comment";
        const string Comments2 = "com";
        const string Comments3 = "c";

        #endregion

        #endregion

        #region Exceptions

        const string ExGeneric = "Unexpected exception";
        const string ExUnexpected = "Unexpected symbol";
        const string ExMultiStr = "Unrecognized multiline string option";
        const string ExFlowArgReq = "Argument is required";
        const string ExFlowArgNotReq = "Argument not expected";
        const string ExUnbalancedParens = "Unbalanaced parentheses in expression";
        const string ExUntermStr = "Unterminated string";
        const string ExUnknownDirv = "Unrecognized directive";
        const string ExInvalidVarName = "Invalid variable name";
        const string ExInvalidVarToken = "Invalid character in variable name";
        const string ExEmptySource = "No code to parse";
        const string ExEmptyVarRef = "Empty variable reference";
        const string ExEnd = "Unexpected end of file";
        const string ExSymbolMismatch = "Parser returned incorrect token";
        const string ExFileNotFound = "File or directory not found";
        const string ExCommand = "Invalid command name";
        const string ExUnclosedBlock = "Unclosed block";
        const string ExInvalidExpression = "Invalid expression term";
        const string ExInvalidExponent = "Invalid exponent.";
        const string ExIntlLineMismatch = "Line and index counts mismatched";
        const string ExContJoinTooLong = "Join string for continuation section is too long";
        const string ExTooFewParams = "Too few parameters passed to function";
        const string ExIncludeNotFound = "Include file not found";
        const string ExNoDynamicVars = "Dynamic variables are not permitted";
        const string ExIllegalCommentFlag = "Illegal comment flag";

        #endregion
    }
}
