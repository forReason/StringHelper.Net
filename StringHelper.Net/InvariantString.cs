using System.Text;
using Microsoft.VisualBasic.CompilerServices;

namespace StringHelper.Net
{
    /// <summary>
    /// Class for handling and cleaning strings to make them comparable. 
    /// Includes methods for lowercasing, trimming, replacing umlauts and removing specified characters.
    /// </summary>
    public class InvariantString
    {
        /// <summary>
        /// quickly gets rid of the most common user input pitfalls for comparison purposes. 
        /// </summary>
        /// <remarks> eg "Péter Petterson " vs "peter petterson"</remarks>
        /// <param name="input">the original string which should be edited</param>
        /// <param name="toLower">Should all characters be TrANsLaTEd to lowercase?</param>
        /// <param name="trim">trim empty spaces at start and end </param>
        /// <param name="cleanUmlaute">Clean special characters such as äöü é î</param>
        /// <param name="replaceWhitespace">when set, replaces any whitespaces with that char</param>
        /// <param name="deduplicateChars">when set, removes multiple occurrences of any of the contained chars, eg. ---, or oooo</param>
        /// <returns></returns>
        public static string InvaryString(
            string input, bool toLower = false, bool trim = true, 
            bool cleanUmlaute = true, char? replaceWhitespace = null, HashSet<char>? deduplicateChars = null)
        {
            if (trim) input = input.Trim();
            StringBuilder output = new StringBuilder();
            char lastChar = char.MaxValue;
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (toLower)
                {
                    c = char.ToLower(c);
                }
                if (replaceWhitespace is not null && char.IsWhiteSpace(c)) 
                {
                    c = replaceWhitespace.Value;
                }
                if (deduplicateChars is not null )
                {
                    if (c == lastChar && deduplicateChars.Contains(c)) continue;
                    lastChar = c;
                }
                if (cleanUmlaute)
                {
                    switch (c)
                    {
                        case 'ä':
                            output.Append("ae");
                            continue;
                        case 'ö':
                            output.Append("oe");
                            continue;
                        case 'ü':
                            output.Append("ue");
                            continue;
                    }
                }
                switch (c)
                {
                    case 'è':
                        output.Append('e');
                        break;
                    case 'ê':
                        output.Append('e');
                        break;
                    case 'é':
                        output.Append('e');
                        break;
                    case 'ì':
                        output.Append('i');
                        break;
                    case 'î':
                        output.Append('i');
                        break;
                    case 'í':
                        output.Append('i');
                        break;
                    case 'â':
                        output.Append('a');
                        break;
                    case 'à':
                        output.Append('a');
                        break;
                    case 'á':
                        output.Append('a');
                        break;
                    default:
                        output.Append(c);
                        break;
                }
            }
            return output.ToString();
        }
        /// <summary>
        /// removes the specified characterset quickly
        /// </summary>
        /// <param name="input"></param>
        /// <param name="charsToClean"></param>
        /// <returns></returns>
        public static string RemoveCharacters(string input, char[] charsToClean)
        {
            StringBuilder output = new StringBuilder();
            foreach (char c in input)
            {
                if (!charsToClean.Contains(c)) output.Append(c);
            }
            return output.ToString();
        }

        /// <summary>
        /// cleans a short string into a tag (replacing spaces and normalizing)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Tagize(string input)
        {
            StringBuilder output = new StringBuilder();
            char lastChar = char.MaxValue;
            foreach (char c in input)
            {
                char cleanedChar = char.ToLowerInvariant(c);
                if (char.IsWhiteSpace(cleanedChar)
                    || cleanedChar == '-'
                    || cleanedChar == '_')
                {
                    char nextChar = '-';
                    if (lastChar == nextChar) continue;
                    output.Append(nextChar);
                    lastChar = nextChar;
                }
                else if (char.IsLetterOrDigit(cleanedChar))
                {
                    output.Append(cleanedChar);
                    lastChar = cleanedChar;
                }
            }
            return output.ToString();
        }
    }
}