
namespace IronAHK.Scripting
{
    partial class Parser
    {
        #region Generic
        const char CR = '\r';
        const char LF = '\n';
        const char SingleSpace = ' ';
        const char Reserved = '\0';
        readonly char[] Spaces = { CR, LF, SingleSpace, '\t', '\xA0' };

        const char StringBound = '"';
        const char ParenOpen = '(';
        const char ParenClose = ')';
        const char BlockOpen = '{';
        const char BlockClose = '}';
        const char ArrayOpen = '[';
        const char ArrayClose = ']';
        const char MultiComA = '/';
        const char MultiComB = '*';
        const char TernaryA = '?';
        const char TernaryB = ':';
        const char HotkeyBound = ':';
        const string HotkeySignal = "::";
        const char Multicast = ',';
        const char Directive = '#';

#if !LEGACY
        const char LastVar = '$';
#endif

#if !LEGACY
        const
#endif
        char Escape = '`';

#if !LEGACY
        const char Comment = ';';
#endif
        string Comment = ";";

#if LEGACY
        const string VarExt = "#_@$?[]";
#endif
#if !LEGACY
        const string VarExt = "#_$";
#endif
        #endregion

        #region Operators
        //const char Power = "**";
        const char Multiply = '*';
        const char Divide = '/';
        //const string FloorDivide = "//";
        const char Add = '+';
        const char Subtract = '-';
        const char BitAND = '&';
        //const string And = "&&";
        const char BitXOR = '^';
        const char BitOR = '|';
        //const string Or = "||";
        const char BitNOT = '~';
        const char Concatenate = '.';
        const char Greater = '>';
        const char Less = '<';
        //const string BitShiftLeft = "<<";
        //const string BitShiftRight = ">>";
        //const string GreaterOrEqual = ">=";
        //const string LessOrEqual = "<=";
        const char Equal = '=';
        const char AssignPre = ':';
        //const string CaseSensitiveEqual = "==";
        //const string NotEqual = "!=";

        const string AndTxt = "and";
        const string OrTxt = "or";
        const string NotTxt = "not";
        const string TrueTxt = "true";
        const string FalseTxt = "false";
        const string NullTxt = "null";

        const string BetweenTxt = "between";
        const string InTxt = "in";
        const string ContainsTxt = "contains";
        const string IsTxt = "is";

        const char Resolve = '%';
        const char Minus = '-';
        const char Not = '!';
        const char Address = '&';
        const char Dereference = '*';

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
        #region Flow
        const string FlowBreak = "break";
        const string FlowContinue = "continue";
        const string FlowElse = "else";
        const string FlowGosub = "gosub";
        const string FlowGoto = "goto";
        const string FlowIf = "if";
        const string FlowLoop = "loop";
        const string FlowReturn = "return";
        const string FlowWhile = "while";
        #endregion

        #region Functions
        const string FunctionLocal = "local";
        const string FunctionGlobal = "global";
        const string FunctionStatic = "static";
        const string FunctionParamRef = "byref";
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

        #region Internal
        const string ExIntlLineMismatch = "Line and index counts mismatched";
        #endregion

#if LEGACY
        const string ExContJoinTooLong = "Join string for continuation section is too long";
#endif
        #endregion
    }
}
