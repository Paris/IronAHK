using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace IronAHK.Rusty
{
    static class OptionParser
    {
        /// <summary>This function helps to parse AHK Option Strings. The options are returned in a string dictionary for easy handling.
        ///
        /// </summary>
        /// <param name="RegExDict">Dictionary of [Parameter/Option Name],[RegexNeedle to extract this Option]</param>
        /// <param name="StringToParse">The AHK Option String</param>
        /// <param name="RemoveFoundOptions">Every processed and found Option is removed from the String</param>
        /// <returns></returns>
        public static Dictionary<string, string> ParseOptionStringToDict(Dictionary<string, Regex> RegExDict, ref string StringToParse, bool RemoveFoundOptions) {
            var OptionDict = new Dictionary<string, string>();
            Match ThisMatch;

            foreach(var item in RegExDict) {
                if(item.Value.IsMatch(StringToParse)) {
                    ThisMatch = item.Value.Match(StringToParse);
                    OptionDict.Add(item.Key, ThisMatch.Groups[1].Captures[0].Value);
                    if(RemoveFoundOptions)
                        StringToParse = item.Value.Replace(StringToParse, "");
                }
            }
            return OptionDict;
        }
    }
}
