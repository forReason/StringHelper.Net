
using System.Text.Json;
using System.Text.RegularExpressions;

namespace StringHelper.Net
{
    /// <summary>
    /// Class containing methods for manipulating and analyzing strings. 
    /// Includes methods for trimming a string, 
    /// quickly comparing if two strings are equal or match, 
    /// and counting the number of lines in a string.
    /// </summary>
    public class StringFunctions
    {
        /// <summary>
        /// takes a reference in memory to a string and calculates its start and end index for use in for loops (quick .Trim());
        /// </summary>
        /// <param name="input">a string which should be trimmed. Max length = int.Max value or string max length. Whatever is smaller</param>
        /// <returns></returns>
        public (int StartIndex, int EndIndex) TrimString(ref string input)
        {
            (int StartIndex, int EndIndex) trimIndex = (0, input.Length - 1);
            // calculate end index (trim end)
            for (int i = trimIndex.EndIndex; i >= 0; i--)
            {
                if (input[i] != ' ' && input[i] != '\t' && input[i] != '\n' && input[i] != '\r')
                {
                    trimIndex.EndIndex = i;
                    break;
                }
            }
            // it is an empty string!
            if (trimIndex.EndIndex == 0) return (0, 0);
            // calculate start index
            for (int i = 0; i <= trimIndex.EndIndex; i++)
            {
                if (input[i] != ' ' && input[i] != '\t' && input[i] != '\n' && input[i] != '\r')
                {
                    trimIndex.StartIndex = i;
                    return trimIndex;
                }
            }
            // this point should never be reached
            return (0, 0);
        }
        /// <summary>
        /// quickly compare if two strings equal or match each other
        /// </summary>
        /// <param name="input"></param>
        /// <param name="comparators"></param>
        /// <returns></returns>
        public bool QuickEqualsOrMatch(string input, string[] comparators)
        {
            bool comparatorsAvailable = true;
            for (int i = 0; i < input.Length; i++)
            {
                comparatorsAvailable = false;
                for (int c = 0; c < comparators.Length; c++)
                {
                    if (comparators[c].Length == 0) continue;
                    if (comparators[c].Length != input.Length || input[i] != comparators[c][i])
                    {
                        comparators[c] = "";
                        continue;
                    }
                    comparatorsAvailable = true;
                }
                if (!comparatorsAvailable)
                {
                    break;
                }
            }
            return comparatorsAvailable;
        }
        /// <summary>
        /// quickly count the amount of lines in a string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public int GetAmountOfLines(ref string input)
        {
            int lines = 1;
            char lastChar = 'a';
            foreach (char c in input)
            {
                if (c == '\r' || (lastChar != '\r' && c == '\n')) lines++;
                lastChar = c;
            }
            return lines;
        }
        /// <summary>
        /// finds and returns the first json element within a text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string? FindJsonInText(ref string input)
        {
            // Regular expression pattern to match JSON objects
            var jsonPattern = new Regex(@"\{.*?\}", RegexOptions.Singleline);
            var matches = jsonPattern.Matches(input);

            foreach (Match match in matches)
            {
                try
                {
                    return match.Value;
                }
                catch (JsonException)
                {
                    continue;
                }
            }
            return null;
        }
    }
}
