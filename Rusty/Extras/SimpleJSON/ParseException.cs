using System;

namespace IronAHK.Rusty.Extras.SimpleJSON
{
    /// <summary>
    /// Parse exception.
    /// </summary>
    sealed class ParseException : Exception
    {
        int position;

        /// <summary>
        /// The position in the string where the error occured.
        /// </summary>
        public int Position
        {
            get { return position; }
        }

        public ParseException(string message, int position)
            : base(message)
        {
            this.position = position;
        }
    }
}
