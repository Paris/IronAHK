using System;
using System.Collections.Generic;
using IronAHK.Rusty.Common;

namespace IronAHK.Rusty
{
    partial class Core
    {
        /// <summary>
        /// Converts a JSON encoded string into an associative array.
        /// </summary>
        /// <param name="source">The string to decode.</param>
        /// <returns>An associative array.</returns>
        public static Dictionary<string, object> JsonDecode(string source)
        {
            try
            {
                ErrorLevel = 0;
                return SimpleJson.Decode(source);
            }
            catch (Exception)
            {
                ErrorLevel = 1;
                return null;
            }
        }

        /// <summary>
        /// Returns a string containing the JSON representation of <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The associative array to encode.</param>
        /// <returns>A JSON encoded string.</returns>
        public static string JsonEncode(Dictionary<string, object> data)
        {
            return SimpleJson.Encode(data);
        }
    }
}
