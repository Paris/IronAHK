using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace IronAHK.Rusty
{
    /// <summary>Provides some simple Color converter Methods.
    /// 
    /// Creation:       06.08.2010 - IsNull
    /// Last Changes:   
    /// </summary>
    public static class ColorFactory
    {
        public enum ColorType
        {
            ARGB,
            RGB,
            BGR
        }

        /// <summary>Cascade Function: Return Color from given Type.
        /// The Alpha Channel is assumed to be 0xFF which means that the Color is full visible.
        /// If you need more control over the Alpha chanel, you have to use the Explicit conversion Functions
        /// </summary>
        /// <param name="value">value to convert</param>
        /// <param name="Type">Source Type</param>
        /// <returns>Color (ARGB)</returns>
        public static Color ColorFrom(int value, ColorType Type) {

            switch(Type) {

                case ColorType.ARGB:
                    return Color.FromArgb(value);

                case ColorType.RGB:
                    return ColorFromRGB(0xFF, value);

                case ColorType.BGR:
                    return ColorFromBGR(0xFF, value);

                default:
                    throw new NotSupportedException();
            }
        }


        /// <summary>Returns a Color from BGR representation
        /// 
        /// </summary>
        /// <param name="BGR">BGR Color representation</param>
        /// <returns>Color (ARGB)</returns>
        public static Color ColorFromBGR(byte A, int BGR) {
            return Color.FromArgb(A, BGR & 0xFF, (BGR >> 8) & 0xFF, (BGR >> 16) & 0xFF);
        }

        /// <summary>Returns a Color from RGB representation
        /// 
        /// </summary>
        /// <param name="A">Alpha channel</param>
        /// <param name="RGB">RGB value</param>
        /// <returns>Color (ARGB)</returns>
        public static Color ColorFromRGB(byte A, int RGB) {
            //return Color.FromArgb(0xFF, (RGB >> 16) & 0xFF, (RGB >> 8) & 0xFF, RGB & 0xFF);
            return Color.FromArgb((int)((RGB | (A << 24)) & 0xFFFFFFFF));
        }
    }
}
