
namespace IronAHK.Rusty.Common
{
    /// <summary>
    /// Serialize JSON strings.
    /// </summary>
    static partial class SimpleJson
    {
        #region Tokens

        const char ObjectOpen = '{';
        const char ObjectClose = '}';
        const char MemberSeperator = ',';
        const char MemberAssign = ':';
        const char MemberAssignAlt = '=';
        const char ArrayOpen = '[';
        const char ArrayClose = ']';
        const char StringBoundary = '"';
        const char StringBoundaryAlt = '\'';
        const char Escape = '\\';
        const string True = "true";
        const string False = "false";
        const string Null = "null";
        const char Space = ' ';

        #endregion

        #region Exceptions

        const string ExUntermField = "Unterminated field";
        const string ExNoMemberVal = "Expected member value";
        const string ExNoKeyPair = "Expected key pair";
        const string ExUnexpectedToken = "Unexpected token";

        const string ExSeperator = " at position ";

        #endregion

        #region Helpers

        static string ErrorMessage(string text, int position)
        {
            return string.Concat(text, ExSeperator, position.ToString());
        }

        #endregion
    }
}
