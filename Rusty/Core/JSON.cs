using System;
using System.Collections.Generic;

namespace IronAHK.Rusty
{
    partial class Core
    {
        /// <summary>
        /// Converts a JSON encoded string into an associative array.
        /// </summary>
        /// <param name="Source">The string to decode.</param>
        /// <returns>An associative array via a dictionary of string key and object value pairs.</returns>
        public static Dictionary<string, object> JsonDecode(string Source)
        {
            try
            {
                error = 0;
                return Extras.SimpleJSON.Decode.Parse(Source);
            }
            catch (Extras.SimpleJSON.ParseException) { error = 2; }
            catch (Exception) { error = 1; }
            return null;
        }

        /// <summary>
        /// Returns a string containing the JSON representation of <paramref name="Data"/>.
        /// </summary>
        /// <param name="Data">The associative array to encode.</param>
        /// <returns>A JSON encoded string.</returns>
        public static string JsonEncode(Dictionary<string, object> Data)
        {
            return Extras.SimpleJSON.Encode.Parse(Data);
        }
    }
}
