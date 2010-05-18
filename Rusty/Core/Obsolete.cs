using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Core
    {
        #region Accessors

        /// <summary>
        /// Determines whether <c>Var1 = %Var2%</c> statements omit spaces and tabs from the beginning and end of Var2.
        /// </summary>
        [Obsolete]
        public static string A_AutoTrim
        {
            get;
            set;
        }

        #endregion

        #region Misc

        /// <summary>
        /// <seealso cref="FileCopy"/>
        /// </summary>
        [Obsolete, Conditional("LEGACY")]
        public static void FileInstall(string Source, string Dest, string Flag)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Performs miscellaneous math functions, bitwise operations, and tasks such as ASCII/Unicode conversion.
        /// This function is obsolete, please use the related newer syntax.
        /// <seealso cref="Asc"/>
        /// <seealso cref="Chr"/>
        /// <seealso cref="Mod"/>
        /// <seealso cref="Exp"/>
        /// <seealso cref="Sqrt"/>
        /// <seealso cref="Log"/>
        /// <seealso cref="Ln"/>
        /// <seealso cref="Round"/>
        /// <seealso cref="Ceil"/>
        /// <seealso cref="Floor"/>
        /// <seealso cref="Abs"/>
        /// <seealso cref="Sin"/>
        /// <seealso cref="Cos"/>
        /// <seealso cref="Tan"/>
        /// <seealso cref="ASin"/>
        /// <seealso cref="ACos"/>
        /// <seealso cref="ATan"/>
        /// <seealso cref="Floor"/>
        /// <seealso cref="Floor"/>
        /// </summary>
        [Obsolete, Conditional("LEGACY")]
        public static void Transform(ref string OutputVar, string Cmd, string Value1, string Value2)
        {
            OutputVar = string.Empty;
            switch (Cmd.Trim().ToLowerInvariant())
            {
                case Keyword_Unicode:
                    if (Value1 == null)
                        OutputVar = Clipboard.GetText();
                    else OutputVar = Value1;
                    break;
                case Keyword_Asc:
                    OutputVar = char.GetNumericValue(Value1, 0).ToString();
                    break;
                case Keyword_Chr:
                    OutputVar = char.ConvertFromUtf32(int.Parse(Value1));
                    break;
                case Keyword_Deref:
                    // TODO: dereference transform
                    break;
                case "html":
                    OutputVar = Value1
                        .Replace("\"", "&quot;")
                        .Replace("&", "&amp;")
                        .Replace("<", "&lt;")
                        .Replace(">", "&gt;")
                        .Replace("\n", "<br/>\n");
                    break;
                case Keyword_Mod:
                    OutputVar = (double.Parse(Value1) % double.Parse(Value2)).ToString();
                    break;
                case Keyword_Pow:
                    OutputVar = Math.Pow(double.Parse(Value1), double.Parse(Value2)).ToString();
                    break;
                case Keyword_Exp:
                    OutputVar = Math.Pow(double.Parse(Value1), Math.E).ToString();
                    break;
                case Keyword_Sqrt:
                    OutputVar = Math.Sqrt(double.Parse(Value1)).ToString();
                    break;
                case Keyword_Log:
                    OutputVar = Math.Log10(double.Parse(Value1)).ToString();
                    break;
                case Keyword_Ln:
                    OutputVar = Math.Log(double.Parse(Value1), Math.E).ToString();
                    break;
                case Keyword_Round:
                    int p = int.Parse(Value2);
                    OutputVar = Math.Round(double.Parse(Value1), p == 0 ? 1 : p).ToString();
                    break;
                case Keyword_Ceil:
                    OutputVar = Math.Ceiling(double.Parse(Value1)).ToString();
                    break;
                case Keyword_Floor:
                    OutputVar = Math.Floor(double.Parse(Value1)).ToString();
                    break;
                case Keyword_Abs:
                    double d = double.Parse(Value1);
                    OutputVar = (d < 0 ? d * -1 : d).ToString();
                    break;
                case Keyword_Sin:
                    OutputVar = Math.Sin(double.Parse(Value1)).ToString();
                    break;
                case Keyword_Cos:
                    OutputVar = Math.Cos(double.Parse(Value1)).ToString();
                    break;
                case Keyword_Tan:
                    OutputVar = Math.Tan(double.Parse(Value1)).ToString();
                    break;
                case Keyword_Asin:
                    OutputVar = Math.Asin(double.Parse(Value1)).ToString();
                    break;
                case Keyword_Acos:
                    OutputVar = Math.Acos(double.Parse(Value1)).ToString();
                    break;
                case Keyword_Atan:
                    OutputVar = Math.Atan(double.Parse(Value1)).ToString();
                    break;
                case Keyword_BitNot:
                    OutputVar = (~int.Parse(Value1)).ToString();
                    break;
                case Keyword_BitAnd:
                    OutputVar = (int.Parse(Value1) & int.Parse(Value2)).ToString();
                    break;
                case Keyword_BitOr:
                    OutputVar = (int.Parse(Value1) | int.Parse(Value2)).ToString();
                    break;
                case Keyword_BitXor:
                    OutputVar = (int.Parse(Value1) ^ int.Parse(Value2)).ToString();
                    break;
                case Keyword_BitShiftLeft:
                    OutputVar = (int.Parse(Value1) << int.Parse(Value2)).ToString();
                    break;
                case Keyword_BitShiftRight:
                    OutputVar = (int.Parse(Value1) >> int.Parse(Value2)).ToString();
                    break;
            }
        }

        #endregion

        #region Debugging

        /// <summary>
        /// Opens the current script for editing in the associated editor.
        /// </summary>
        [Obsolete, Conditional("LEGACY")]
        public static void Edit()
        {

        }

        /// <summary>
        /// Displays the script lines most recently executed.
        /// </summary>
        [Obsolete, Conditional("LEGACY")]
        public static void ListLines()
        {

        }

        /// <summary>
        /// Displays the script's variables: their names and current contents.
        /// </summary>
        [Obsolete, Conditional("LEGACY")]
        public static void ListVars()
        {

        }

        #endregion

        #region Mouse

        /// <summary>
        /// <seealso cref="Click"/>
        /// </summary>
        [Obsolete, Conditional("LEGACY")]
        public static void LeftClick(int X, int Y, int ClickCount, int Speed, string DU, bool R)
        {
            MouseClick(Keyword_Left, X, Y, ClickCount, Speed, DU, R);
        }

        /// <summary>
        /// 
        /// </summary>
        [Obsolete, Conditional("LEGACY")]
        public static void LeftClickDrag(int X1, int Y1, int X2, int Y2, int Speed, bool R)
        {
            MouseClickDrag(Keyword_Left, X1, Y1, X2, Y2, Speed, R);
        }

        /// <summary>
        /// <seealso cref="Click"/>
        /// </summary>
        [Obsolete, Conditional("LEGACY")]
        public static void MouseClick(string WhichButton, int X, int Y, int ClickCount, int Speed, string DU, bool R)
        {
            Click(new[] { WhichButton, X.ToString(), Y.ToString(), ClickCount.ToString(), Speed.ToString(), DU, R ? Keyword_Relative : string.Empty });
        }

        /// <summary>
        /// <seealso cref="Click"/>
        /// </summary>
        [Obsolete, Conditional("LEGACY")]
        public static void MouseClickDrag(string WhichButton, int X1, int Y1, int X2, int Y2, int Speed, bool R)
        {
            string[] options = { WhichButton, X1.ToString(), Y1.ToString(), "1", Speed.ToString(), Keyword_Down, R ? Keyword_Relative : string.Empty };
            
            Click(options);
            
            options[1] = X2.ToString();
            options[2] = Y2.ToString();
            options[6] = Keyword_Up;

            Click(options);
        }

        /// <summary>
        /// <seealso cref="Click"/>
        /// </summary>
        [Obsolete, Conditional("LEGACY")]
        public static void MouseMove(int X, int Y, int Speed, bool R)
        {
            Click(new[] { X.ToString(), Y.ToString(), "0", Speed.ToString(), R ? Keyword_Relative : string.Empty });
        }

        #endregion
    }
}
