using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace String_Helper_Project
{
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
    }
}
