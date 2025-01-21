using System.Text;
using StringHelper.Net.Filtering.WordFilters;

namespace StringHelper.Net.StringFunctionsNS;

public class CalculateVariance
{
    /// <summary>
    ///  returns a percentage based simmilarity score by checking which words are used how often.
    /// </summary>
    /// <param name="inputA"></param>
    /// <param name="inputB"></param>
    /// <returns></returns>
    public static double GetVariance(string inputA, string inputB)
    {
        Dictionary<string, int> wordset1 = BuildWordSet(inputA);
        Dictionary<string, int> wordset2 = BuildWordSet(inputB);

        int simmilarities = 0;
        int total = 0;
        foreach (KeyValuePair<string, int> set in wordset1)
        {
            total += set.Value;
            if (wordset2.TryGetValue(set.Key, out int value))
            {
                total += value;
                simmilarities += Math.Min(set.Value, value) * 2;
                wordset2.Remove(set.Key);
            }
        }
        foreach (KeyValuePair<string, int> set in wordset2)
        {
            total += set.Value;
        }
        return (double)simmilarities / (double)total;
    }
    private static Dictionary<string, int> BuildWordSet(string input)
    {
        Dictionary<string, int> wordset = new Dictionary<string, int>();
        StringBuilder builder = new StringBuilder();
        foreach (char c in input)
        {
            if (builder.Length > 0 && WordFilter.IsDelimiter(c))
            {
                string word = builder.ToString();
                builder.Clear();

                if (wordset.ContainsKey(word))
                    wordset[word]++;
                else
                    wordset[word] = 1;

                continue;
            }

            if (char.IsLetter(c)) builder.Append(char.ToUpperInvariant(c));
        }
        string lastWord = builder.ToString();
        if (wordset.ContainsKey(lastWord))
            wordset[lastWord]++;
        else
            wordset[lastWord] = 1;

        return wordset;
    }
}