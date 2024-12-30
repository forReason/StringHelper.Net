
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

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
        /// <exception cref="JsonException">the json is likely malformed</exception>
        /// <returns>the json string</returns>
        public string? FindJsonInText(ref string input)
        {
            var jsonPattern = new Regex(@"\{(?:[^{}]|(?<open>\{)|(?<-open>\}))+(?(open)(?!))\}", RegexOptions.Singleline);
            var matches = jsonPattern.Matches(input);
            var exceptions = new List<JsonException>();

            foreach (Match match in matches)
            {
                string jsonCandidate = match.Value;

                // Fix formatting issues in JSON string
                jsonCandidate = FixJsonFormatting(jsonCandidate);
                try
                {
                    // Attempt to parse to ensure it's valid JSON
                    var jsonObject = JsonSerializer.Deserialize<JsonElement>(jsonCandidate);
                    return jsonCandidate; // If parsing is successful, return the JSON string
                }
                catch (JsonException ex)
                {
                    exceptions.Add(ex);
                    continue; // If parsing fails, continue to the next match
                }
            }

            if (exceptions.Count > 0)
            {
                throw exceptions[0];
            }

            return null; // Return null if no valid JSON was found
        }


        private string FixJsonFormatting(string json)
        {
            // Correct JSON formatting issues by handling double backslashes
            // Replace double backslashes followed by `n`, `r`, `t` with single escape sequences
            json = Regex.Replace(json, @"\\(\\n|\\r|\\t)", @"\1");

            // Correct any cases where single backslashes are used incorrectly
            json = Regex.Replace(json, @"(?<!\\)\\([nrt])", @"\\\1");

            return json;
        }
        
        /// <summary>
        /// this function extracts a DateTime object from string.
        /// </summary>
        /// <param name="input">the string which to scan for the dateTime</param>
        /// <param name="pattern">the regex pattern to match. Defaults to american format MM/DD/YYYY</param>
        /// <returns></returns>
        public static DateTime? ExtractDate(string input, string pattern = @"\d{2}/\d{2}/\d{4}")
        {
            // Regular expression pattern to capture date in MM/DD/YYYY format
            var datePattern = pattern;
        
            // Search for the date pattern in the input string
            var match = Regex.Match(input, datePattern);
        
            // If a match is found, try to parse the date
            if (match.Success && DateTime.TryParse(match.Value, out DateTime releaseDate))
            {
                return releaseDate;
            }

            return null;
        }
    }
}
